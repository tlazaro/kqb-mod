using System;
using System.Collections;
using System.Collections.Generic;
using Discord;
using GameLogic.Utility;
using LiquidBit.KillerQueenX;
using LiquidBit.KillerQueenX.Utility;
using Newtonsoft.Json;
using Steamworks;
using UnityEngine;
using UnityEngine.Networking;

namespace KQBMod.SteamClientOverride
{
	// Class for overriding LB's GameSparks-based Steam Client
	public class SteamClientOverride: GameSparksBasePlatformClient
	{
		public SteamClientOverride(GameObject steamManagerPrefab)
		{
			this.steamManagerPrefab = steamManagerPrefab;
		}

		// Unchanged from SteamClient
		public override void Init()
		{
			Debug.Log("--- STEAM CLIENT INIT CALLED");
			bool flag = SteamAPI.Init();
			if (!this.initialized)
			{
				if (!flag)
				{
					Debug.Log("--- STEAM CLIENT INIT STEAM NOT RUNNING");
					return;
				}
				UnityEngine.Object.Instantiate<GameObject>(this.steamManagerPrefab);
				Debug.Log("---- STEAM CLIENT INITIALIZED");
				this.initialized = true;
				this.discordUtility = new DiscordUtility(CreateFlags.NoRequireDiscord);
				this.m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(new Callback<GameOverlayActivated_t>.DispatchDelegate(this.OnGameOverlayActivated));
				if (SteamUtils.IsSteamInBigPictureMode())
				{
					GameManager.GMInstance.ForceFullScreen();
					return;
				}
			}
			else
			{
				Debug.Log("--- STEAM CLIENT INIT ALREADY INITIALIZED");
			}
		}

		// Unchanged from SteamClient
		public override void Update()
		{
			if (this.initialized)
			{
				this.discordUtility.Update();
			}
		}

		// Unchanged from SteamClient
		public override IEnumerator ValidateOwnership(Action valid, Action invalid)
		{
			bool flag = false;
			if (this.initialized)
			{
				flag = true;
			}
			if (flag)
			{
				valid();
			}
			else
			{
				invalid();
			}
			yield break;
		}

		// Unchanged from SteamClient, except for a useless extra yield break statement after the yield return null
		public override IEnumerator DoLoginFlow(bool isStartup, Action onSuccess, Action onFailure, bool onlineServicePrompt)
		{
			if (this.initialized && SteamAPI.IsSteamRunning())
			{
				this.SteamAuth(onSuccess, onFailure);
			}
			yield return null;
		}

		// Unchanged from SteamClient
		private void SteamAuth(Action onSuccess, Action onFailure)
		{
			base.UpdateAuthStatus(AuthStatus.WaitingForAuth);
			this.appId = SteamUtils.GetAppID().ToString();
			this.discordUtility.RegisterSteam(this.appId);
			if (this.steamAuthTicket == null)
			{
				byte[] array = new byte[1024];
                SteamUser.GetAuthSessionTicket(array, 1024, out uint num);
                this.steamAuthTicket = "";
				int num2 = 0;
				while ((long)num2 < (long)((ulong)num))
				{
					this.steamAuthTicket += string.Format("{0:X2}", array[num2]);
					num2++;
				}
			}
			Debug.Log(string.Concat(new object[]
			{
				"---- steamAuthTicket: ",
				this.steamAuthTicket,
				" length: ",
				this.steamAuthTicket.Length
			}));
			GameManager.GMInstance.StartCoroutine(this.GetAvatarAndLogin(onSuccess, onFailure));
		}

		// Updated to bypass GameSpark startup and API Calls
		private IEnumerator GetAvatarAndLogin(Action onSuccess, Action onFailure)
		{
			Debug.Log("--- START GET AVATAR AND LOGIN");

			// Get the Steam ID from Steam process running in the background
			string steamId = SteamUser.GetSteamID().ToString();
			Debug.Log("--- STEAM USER ID: " + steamId);

			// Send the Steam ID to the Steam API to get other info like Username and Avatar URL
			UnityWebRequestAsyncOperation async = new UnityWebRequest("https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=1D04E36F7E09998864E2966531960BD1&steamids=" + steamId)
			{
				method = "GET",
				downloadHandler = new DownloadHandlerBuffer()
			}.SendWebRequest();
			yield return async;
			UnityWebRequest webRequest = async.webRequest;
			if (!webRequest.isNetworkError && !webRequest.isHttpError)
			{
				// Convert the JSON response into an object that can be used to create a KQB user profile
				SteamJsonResponse steamJson = JsonConvert.DeserializeObject<SteamJsonResponse>(webRequest.downloadHandler.text);
				if (steamJson.response.players.Count > 0)
				{
					// The following block was adapted from GameSparksBasePlatformClient.CreatePlaceholderAccount()
					// Init a new game profile with ID, Username and Avatar URL from Steam API
					GameLogic.Profile profile = new GameLogic.Profile
                    {
                        playerId = steamJson.response.players[0].steamid, // TODO: Ideally would be GS Player ID, want to map Steam ID to GS ID
                        displayName = steamJson.response.players[0].personaname,
                        avatarUrl = steamJson.response.players[0].avatarfull
                    };
                    // Assign the profile to the account field, otherwise KQB will use a placeholder account
					// placeholder account assigns 'P1' username, uses a default avatar and generates a random player ID using GUID function
                    this.account = profile;
					// Not 100% sure what these do, they were copied from the CreatePlaceholderAccount() method
					// AddProfileToCache() looks like it runs the displayName through the GameManager's stringSanitizer and then adds it to the GameManager's Cache
					GameManager.GMInstance.profileCache.AddProfileToCache(profile);
					// InitializeLocalParty() looks like it assigns this profile as the party leader, then adds any local players to that player's party
					GameManager.GMInstance.partyManager.InitializeLocalParty();
					// When called by DoLoginFlow() is called by GameManager default onSuccess() method is GameManager.LoginSuccess()
					// LoginSuccess() just logs that the login was successful, then tells game to move onto next step
					onSuccess();
					yield break;
				}
			}
			// When called by DoLoginFlow() is called by GameManager default onFailure() method is GameManager.LoginFailure()
			// LoginFailure() just logs that the login failed, then tells game to move onto next step
			onFailure();
			yield break;

		}

		// Unchanged from SteamClient
		public override void RefreshFriendsList(Action successCallback, Action errorCallback)
		{
			int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
			List<string> list = new List<string>();
			for (int i = 0; i < friendCount; i++)
			{
				list.Add(SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate).ToString());
			}
			base.FetchProfiles("steamIds", list, true, true, delegate (GameLogic.Profile profile)
			{
				this.UpdateFriendFromProfile(profile);
			}, delegate (List<GameLogic.Profile> profiles)
			{
				this.friends = profiles;
				this.FriendsListUpdated(this.friends);
				if (successCallback != null)
				{
					successCallback();
				}
			}, errorCallback);
		}

		// Unchanged from SteamClient
		public override void Dispose()
		{
			this.discordUtility.Dispose();
		}

		// Unchanged from SteamClient
		private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
		{
			if (pCallback.m_bActive != 0)
			{
				Debug.Log("Steam Overlay has been activated");
				GameManager.GMInstance.inputManager.DisableAllInput();
				return;
			}
			Debug.Log("Steam Overlay has been closed");
			GameManager.GMInstance.StartCoroutine(this.DelayedSetLastControllerScheme());
		}

		// Unchanged from SteamClient
		private IEnumerator DelayedSetLastControllerScheme()
		{
			yield return new WaitForSecondsRealtime(0.1f);
			GameManager.GMInstance.inputManager.SetLastControllerScheme();
			yield break;
		}

		// Unchanged from SteamClient
		public override Platform GetPlatformID()
		{
			return Platform.Steam;
		}

		// Unchanged from SteamClient
		protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;

		// Unchanged from SteamClient
		private bool initialized;

		// Unchanged from SteamClient
		private GameObject steamManagerPrefab;

		// Unchanged from SteamClient
		private string steamAuthTicket;

		// Classes for deserializing JSON Response from steam API
		public class SteamJsonResponse
		{
			public SteamPlayers response;
		}

		public class SteamPlayers
		{
			public List<SteamPlayer> players;
		}

		public class SteamPlayer
		{
			public string steamid;
			public string personaname;
			public string avatarfull;
		}
	}
}