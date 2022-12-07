using UnityEngine;
using UnityModManagerNet;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Reflection;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HarmonyLib;
using System.Linq;
using LiquidBit.KillerQueenX;
using I2.Loc;
using System.Runtime.Remoting.Messaging;
using GameLogic;
using GameLogic.ServerCommands;
using System.Runtime.CompilerServices;
using BumbleBear;
using GameLogic.ClientCommands;

namespace KQBMod
{
#if DEBUG
    [EnableReloading]
#endif

    public class Settings : UnityModManager.ModSettings
    {
        public enum ShotSorting
        {
            Default = 0,
            Shuffle,
            ByType,
            ByDeposit
        }

        public static ShotSorting[] ShotSortingValues = KQBMod.Utils.GetEnumValues<ShotSorting>();

        public enum TrainingLevel
        {
            Helix = 0,
            Tally,
        }

        public static TrainingLevel[] TrainingLevelValues = KQBMod.Utils.GetEnumValues<TrainingLevel>();

        public static Training.ShotType[] ShotTypesValues = KQBMod.Utils.GetEnumValues<Training.ShotType>();

        public string ip = "127.0.0.1";
        public ushort port = 5000;
        public bool remote = false;
        public bool customTrainingInterleaved = false;
        public bool moveToNextOnSucceess = true;
        public ShotSorting shotSorting = ShotSorting.Default;
        public TrainingLevel trainingLevel = TrainingLevel.Helix;
        public bool[] activeShotTypes = new bool[ShotTypesValues.Length];
        public bool showHitboxes = false;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }

    public static class Main
    {
        public static UnityModManager.ModEntry.ModLogger Logger;
        public static Harmony MainHarmony = new Harmony("KQBMod");

        public static Settings settings = null;
        public static ModGameManager manager = null;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            Logger = modEntry.Logger;
            Logger.Log("Starting KQBMod");

            settings = Settings.Load<Settings>(modEntry);
            manager = new ModGameManager();

            MainHarmony.PatchAll();

            // Patch CustomMatchLobbyGameMode which is internal
            var ass = Assembly.GetAssembly(typeof(GameLogic.Actor));
            var type = ass.GetType("GameLogic.CustomMatchLobbyGameMode", true);
            var original = type.GetMethod("SetLevelsForMatch", BindingFlags.NonPublic | BindingFlags.Static);
            var prefix = typeof(SetLevelsForMatch).GetMethod("Prefix", BindingFlags.Static | BindingFlags.Public);
            MainHarmony.Patch(original, prefix: new HarmonyMethod(prefix));

#if DEBUG
            modEntry.OnUnload = Unload;
#endif
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;

            return true;
        }

#if DEBUG
        static bool Unload(UnityModManager.ModEntry modEntry)
        {
            Logger.Log("Unloading KQBMod");

            MainHarmony.UnpatchAll();

            return true;
        }
#endif

        public static void RemoveOnline(NavMenu navMenu)
        {
            List<NavItem> mainNavStructure = Traverse.Create(navMenu).Field<List<NavItem>>("mainNavStructure").Value;

            // Remove online play to avoid accidentally playing online with mod enabled
            var onlinePlayItem = mainNavStructure.Find(item => item.mainText.mTerm == NavItem.Online.mainText.mTerm);
            if (onlinePlayItem != null)
            {
                mainNavStructure.Remove(onlinePlayItem);
            }

            // Remove KQB TV to avoid accidentally disturbing online with mod enabled
            var kqbTV = mainNavStructure.Find(item => item.mainText.mTerm == NavItem.Spectate.mainText.mTerm);
            if (kqbTV != null)
            {
                mainNavStructure.Remove(kqbTV);
            }
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUIStyle h2 = new GUIStyle();
            h2.name = "umm h2";
            h2.normal.textColor = new Color(0.6f, 0.91f, 1f);
            h2.fontStyle = FontStyle.Bold;
            h2.fontSize = 13;
            h2.margin = new RectOffset(0, 0, 3, 3);

#if DEBUG
            GUILayout.Label("Free Play Mode", h2);

            settings.showHitboxes = GUILayout.Toggle(settings.showHitboxes, "Show hitboxes", GUILayout.ExpandWidth(false));
            
            GUILayout.Label("Custom Training Mode", h2);

            int selectedLevel = GUILayout.SelectionGrid((int)settings.trainingLevel, Settings.TrainingLevelValues.Select(v => v.ToString()).ToArray(), Settings.TrainingLevelValues.Length);
            settings.trainingLevel = Settings.TrainingLevelValues[selectedLevel];

            int selected = GUILayout.SelectionGrid((int)settings.shotSorting, Settings.ShotSortingValues.Select(v => v.ToString()).ToArray(), Settings.ShotSortingValues.Length);
            settings.shotSorting = Settings.ShotSortingValues[selected];

            settings.customTrainingInterleaved = GUILayout.Toggle(settings.customTrainingInterleaved, "Alternate between Blue and Gold", GUILayout.ExpandWidth(false));
            settings.moveToNextOnSucceess = GUILayout.Toggle(settings.moveToNextOnSucceess, "Move to next on success", GUILayout.ExpandWidth(false));

            foreach(Training.ShotType type in Settings.ShotTypesValues)
            {
                settings.activeShotTypes[(int)type] = GUILayout.Toggle(settings.activeShotTypes[(int)type], type.ToString(), GUILayout.ExpandWidth(false));
            }
#endif
            GUILayout.Label("Networking", h2);

            GUILayout.Label("Remote Host");
            settings.ip = GUILayout.TextField(settings.ip, GUILayout.Width(200));

            var portUpdate = GUILayout.TextField(settings.port.ToString(), GUILayout.Width(50));
            if (portUpdate != settings.port.ToString() && ushort.TryParse(portUpdate, out var p))
            {
                settings.port = p;
            }
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }

        public static Game GetServerGame()
        {
            if (!manager.inCustomMode()) return null;

            // The Match Manager runs locally but we need to modify the state of the server, the source of truth.
            // All entities used must come from the server.
            GameServer server = Traverse.Create(GameManager.GMInstance).Field("localGameServer").GetValue<GameServer>();

            return server.game;
        }

        public static Player GetPlayer(MatchClient client)
        {
            if (!manager.inCustomMode()) return null;

            int actorNr = client.mainPlayerId.actorNr;
            int inputID = client.mainPlayerId.inputID;

            Player player = GetServerGame().gameState.GetPlayer(actorNr, inputID);

            return player;
        }

        public static Entity GetPlayerEntity(Player player)
        {
            if (!manager.inCustomMode()) return null;

            Entity playerEntity = GetServerGame().gameState.GetEntityByActorNr(player.actorNr, player.inputID);
            return playerEntity;
        }

        // Gets the player for the local client
        public static Player GetPlayer()
        {
            return GetPlayer(GameManager.GMInstance.currentClient);
        }

        // Gets the player entity for the local client
        public static Entity GetPlayerEntity()
        {
            return GetPlayerEntity(GetPlayer());
        }
    }

    // We don't want user of the mod to join online matches
    [HarmonyPatch(typeof(UIManager))]
    [HarmonyPatch("InitMainMenu")]
    static class RemoveOnlinePlayUiManager
    {
        static void Postfix(UIManager __instance)
        {
            Main.Logger.Log("UIManager InitMainMenu");

            Main.RemoveOnline(Traverse.Create(__instance).Field<NavMenu>("navMenu").Value);
        }
    }

    [HarmonyPatch(typeof(NavMenu))]
    [HarmonyPatch("Awake")]
    static class RemoveOnlinePlay
    {
        static void Postfix(NavMenu __instance)
        {
            Main.Logger.Log("NavMenu InitMainMenu");

            Main.RemoveOnline(__instance);
        }
    }

    [HarmonyPatch(typeof(GameManager))]
    [HarmonyPatch("CreatePlatformClient")]
    static class GameSparksSteamOverride
    {
        static bool Prefix()
        {
            Main.Logger.Log("Skipping Base GameManager.CreatePlatformClient Method");
            return false;
        }

        static void Postfix(ref IPlatformClient __result)
        {
            GameObject steamManagerPrefab = Traverse.Create(GameManager.GMInstance).Field("steamManagerPrefab").GetValue<GameObject>();
            SteamClientOverride.SteamClientOverride steamClient = new SteamClientOverride.SteamClientOverride(steamManagerPrefab);
            ((IPlatformClient)steamClient).Init();
            __result = steamClient;
        }
    }
}