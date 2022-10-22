using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using KQBMod.Training;
using KQBMod.Remote;
using UnityEngine;
using UnityModManagerNet;
using UnityEngine.UI;
using System.Reflection;
using System.Globalization;
using System.Collections;
using System.Runtime.InteropServices;
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
    public interface ModGameMode
    {
        NavItem.Type getMenuItemType();
        string getMenuItemText();
        ModGameModeType getModeType();
        bool StartingLobby(CustomMatchLobbyState state);
        bool StartingMatch(MatchManager matchManager);
        bool BerryOnDeposit(Game game, Entity depositingEntity, Entity berryDeposit, Entity.BerryDepositInteractionType berryDepositInteractionType);
    }

    public enum ModGameModeType
    {
        FreePlay,
        CustomTraining,
        HostRemotePlay,
        JoinRemotePlay
    }

    public class ModGameManager
    {
        private ModGameMode _currentMode = null;
        public ModGameMode CurrentMode
        {
            get { return _currentMode; }
            set
            {
                string text = value == null ? "null" : value.getMenuItemText();
                Main.Logger.Log("Setting mode to " + text);
                _currentMode = value;
            }
        }
        public ModGameMode _goingIntoMode = null;
        public ModGameMode GoingIntoMode
        {
            get { return _goingIntoMode; }
            set
            {
                string text = value == null ? "null" : value.getMenuItemText();
                Main.Logger.Log("Going into mode to " + text);
                _goingIntoMode = value;
            }
        }

        public List<ModGameMode> modes = new List<ModGameMode>{
            //new FreePlay(),
            //new CustomTraining(),
            new JoinRemotePlay(),
            new HostRemotePlay(),
        };

        public Dictionary<ModGameMode, NavItem> menuItems = new Dictionary<ModGameMode, NavItem>();

        public ModGameManager()
        {

        }

        public bool inCustomMode()
        {
            return CurrentMode != null;
        }

        public bool isCustomMode(ModGameModeType type)
        {
            return CurrentMode != null && CurrentMode.getModeType() == type;
        }

        public bool isRemote()
        {
            return isAnyOf(ModGameModeType.HostRemotePlay, ModGameModeType.JoinRemotePlay);
        }

        public bool isAnyOf(params ModGameModeType[] types)
        {
            return CurrentMode != null && types.Contains(CurrentMode.getModeType());
        }

        public bool StartingLobby(CustomMatchLobbyState state)
        {
            return CurrentMode.StartingLobby(state);
        }

        public bool StartMatchClicked(CustomMatchUI customMatchUI)
        {
            if (Main.manager.isCustomMode(ModGameModeType.FreePlay) || Main.manager.isCustomMode(ModGameModeType.CustomTraining))
            {
                Main.Logger.Log("SkipReadyState");
                Game.gameConfiguration.gameParams.skinSelectScreenTime = 3;
                customMatchUI.ProcessStartSkinSelect();

                return false;
            }
            
            return true;
        }

        public bool StartingMatch(MatchManager matchManager)
        {
            if (Main.manager.isCustomMode(ModGameModeType.FreePlay) || Main.manager.isCustomMode(ModGameModeType.CustomTraining))
            {
                Main.Logger.Log("Trying to escape the 3 Ways");
                Main.GetServerGame().gameState.startGameCountdown = 0;
                matchManager.currentClient.gameLogic.gameState.startGameCountdown = 0;

                return CurrentMode.StartingMatch(matchManager);
            }

            return true;
        }

        public bool BerryOnDeposit(Game game, Entity depositingEntity, Entity berryDeposit, Entity.BerryDepositInteractionType berryDepositInteractionType)
        {
            return CurrentMode.BerryOnDeposit(game, depositingEntity, berryDeposit, berryDepositInteractionType);
        }
    }

    [HarmonyPatch(typeof(CustomMatchLobbyGameMode))]
    [HarmonyPatch("Start")]
    static class CustomMatchLobbyGameModeStart
    {
        static bool Prefix(CustomMatchLobbyGameMode __instance, MatchManager matchManager)
        {
            if (Main.manager.inCustomMode())
            {

            }

            return true;
        }
    }

    // Patched manually in Main because CustomMatchLobbyGameMode is internal
    //[HarmonyPatch(Type.GetType("GameLogic.CustomMatchLobbyGameMode"))]
    //[HarmonyPatch(typeof(String), "SetLevelsForMatch")]
    public static class SetLevelsForMatch
    {
        public static bool Prefix(Game game, CustomMatchLobbyState state, ref List<int> activeLevels)
        {
            if (Main.manager.inCustomMode())
            {
                Main.manager.StartingLobby(state);

                //foreach (GameLogic.Level level in Game.gameConfiguration.levels)
                //{
                //    Main.Logger.Log($"Level id {level.id}:{level.name} playable {level.playable} pool {level.availableForPool}");
                //}

                Main.Logger.Log($"Active levels before {String.Join(", ", activeLevels)}");

                if (Main.manager.isCustomMode(ModGameModeType.CustomTraining))
                {
                    var mode = (CustomTraining)Main.manager.CurrentMode;

                    activeLevels = activeLevels.Where(levelId => levelId == (int)mode.trainingPack.levelId).ToList();
                    Main.Logger.Log($"Active levels after {String.Join(", ", activeLevels)}");
                }
            }

            return true;
        }
    }


    [HarmonyPatch(typeof(CustomMatchUI))]
    [HarmonyPatch("StartMatchClicked")]
    static class SkipReadyState
    {
        static bool Prefix(CustomMatchUI __instance)
        {
            if (Main.manager.inCustomMode())
            {
                return Main.manager.StartMatchClicked(__instance);
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(NavMenu))]
    [HarmonyPatch("Awake")]
    static class BackToMainMenu
    {
        static bool Prefix(NavMenu __instance)
        {
            Main.Logger.Log("NavMenu Awake");
            if (Main.manager.inCustomMode())
            {
                Main.manager.CurrentMode = null;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(NavMenu))]
    [HarmonyPatch("SetupMenu")]
    static class InjectCustomModesMenuButtons
    {
        static NavItem MkNavItem(NavItem.Type type, string mainText, string mainTextTranslation, bool partyLeaderOnly, bool disableForRemoteParty)
        {
            Type navItemType = typeof(NavItem);
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;

            // TODO cleanup the language resources after removal
            var langSource = new LanguageSourceData();
            LocalizationManager.Sources.Add(langSource);
            langSource.AddTerm(mainText).SetTranslation(0, mainTextTranslation);

            NavItem navItem = new NavItem();
            navItemType.GetField("_type", flags).SetValue(navItem, type);
            navItemType.GetField("_mainText", flags).SetValue(navItem, new I2.Loc.LocalizedString(mainText));
            navItemType.GetField("_partyLeaderOnly", flags).SetValue(navItem, partyLeaderOnly);
            navItemType.GetField("_disableForRemoteParty", flags).SetValue(navItem, disableForRemoteParty);

            return navItem;
        }

        static bool Prefix(NavMenu __instance, List<NavItem> menuTypes, ref List<NavItem> ___localNavStructure, ref List<NavItem> ___mainNavStructure)
        {
            if (Main.manager.inCustomMode())
            {
                Main.manager.CurrentMode = null;
            }

            Main.Logger.Log($"nav structure has: {String.Join(", ", ___localNavStructure.Select(x => x.mainText.mTerm))}");

            // Only patch localNavStructure if about to be used
            if (menuTypes.Equals(___localNavStructure))
            {
                Main.Logger.Log("local nav");
                var insertIndex = 1; // Leaves an empty line at 0
                foreach (var mode in Main.manager.modes)
                {
                    Main.Logger.Log("adding mode: " + mode.getMenuItemText());
                    NavItem menuItem = null;
                    Main.manager.menuItems.TryGetValue(mode, out menuItem);

                    if (menuItem == null)
                    {
                        Main.Logger.Log("item was null");
                        var mterm = "MainMenu/" + mode.getMenuItemText();

                        // Plugin was reloaded, cleanup previous NavItem
                        // TODO maintain a list of side-effect and undo them?
                        var previousItem = ___localNavStructure.Find(item => item.mainText.mTerm == mterm);
                        if (previousItem != null)
                        {
                            Main.Logger.Log("removing old menu item");
                            ___localNavStructure.Remove(previousItem);
                        }
                        menuItem = MkNavItem(NavItem.Type.LocalPlay, mterm, mode.getMenuItemText(), true, true);

                        Main.manager.menuItems.Add(mode, menuItem);
                    }

                    if (menuItem != null && !___localNavStructure.Contains(menuItem))
                    {
                        Main.Logger.Log("inserting menu item");
                        ___localNavStructure.Insert(insertIndex, menuItem);
                        insertIndex++;
                    }

                    Main.Logger.Log($"nav structure now has: {String.Join(", ", ___localNavStructure.Select(x => x.mainText.mTerm))}");
                }
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(NavMenu))]
    [HarmonyPatch("NavButtonClicked")]
    static class ModGameModeButtonClicked
    {
        static bool Prefix(NavItem item)
        {
            // Set global flag indicating attempt to enter custom mode
            foreach (var entry in Main.manager.menuItems)
            {
                if (item.Equals(entry.Value))
                {
                    Main.manager.GoingIntoMode = entry.Key;
                    return true;
                }
            }

            Main.manager.GoingIntoMode = null;
            return true;
        }
    }

    [HarmonyPatch(typeof(GameManager))]
    [HarmonyPatch("StartLocalServerMatch")]
    static class StartLocalServerMatchOverrideRemote
    {
        static bool Prefix(GameManager __instance)
        {
            if (Main.manager.GoingIntoMode != null)
            {
                Main.Logger.Log($"Starting {Main.manager.GoingIntoMode.getModeType()} Mode");

                Main.manager.CurrentMode = Main.manager.GoingIntoMode;

                if (Main.manager.CurrentMode.getModeType() == ModGameModeType.JoinRemotePlay)
                {
                    Main.Logger.Log($"Connecting to {Main.settings.ip}:{Main.settings.port}");
                    UIManager.Instance.DirectConnectToServer(Main.settings.ip, Main.settings.port, false);
                }
                else // if (Main.manager.GoingIntoMode.getModeType() == ModGameModeType.HostRemotePlay)
                {
                    Main.Logger.Log($"Hosting game at 127.0.0.1:5000 with bind address 0.0.0.0");
                    __instance.StartLocalServer("127.0.0.1", "0.0.0.0", MatchType.Custom);
                    UIManager.Instance.DirectConnectToServer("127.0.0.1", 5000, false);
                }

                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(MatchGameMode))]
    [HarmonyPatch("StartMatch")]
    static class StartingMatch
    {
        static bool Prefix(MatchManager matchManager)
        {
            if (Main.manager.inCustomMode())
            {
                return Main.manager.StartingMatch(matchManager);
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(MatchClient))]
    [HarmonyPatch("ProcessNonVerbalCommunicationCommand")]
    static class FreePlayModeActions
    {
        static bool Prefix(MatchClient __instance, ClientCommand command)
        {
            // Remote play should use default behavior of emotes
            if (Main.manager.isRemote())
            {
                return true;
            }

            if (Main.manager.inCustomMode() && command.type == ClientCommand.Type.NonVerbalCommunication)
            {
                Main.Logger.Log("Custom Training action " + command.type);

                NonVerbalCommunication communication = (NonVerbalCommunication)command.command;
                NonVerbalCommunication.Type type = (NonVerbalCommunication.Type)communication.type;

                Game game = Main.GetServerGame();

                Player player = Main.GetPlayer();
                Entity playerEntity = Main.GetPlayerEntity();

                // Removes emote throttling allowing quick switch
                game.gameState.blueTeam.lastAlertTime = 0;
                game.gameState.redTeam.lastAlertTime = 0;
                foreach (var p in game.gameState.players)
                {
                    p.lastEmoteTime = 0f;
                    p.emoteThrottleNumberSent = 0;
                    p.emoteThrottleStartTime = 0;
                }

                if (Main.manager.isCustomMode(ModGameModeType.CustomTraining))
                {
                    var mode = (CustomTraining)Main.manager.CurrentMode;

                    switch (type)
                    {
                        case NonVerbalCommunication.Type.Alert:
                            switch ((NonVerbalCommunication.Alert)communication.value)
                            {
                                case NonVerbalCommunication.Alert.BlueBase:
                                    mode.PreviousShot(mode._event);
                                    break;
                                case NonVerbalCommunication.Alert.GoldBase:
                                    // Avoid triggering win condition
                                    mode.NextShot(mode._event, false);
                                    break;
                                case NonVerbalCommunication.Alert.Snail:
                                    mode.PrepareShot(mode._event);
                                    break;
                                case NonVerbalCommunication.Alert.Gate:
                                    mode.currentShotIndex = 0;
                                    mode.currentTeam = Team.Blue;
                                    mode.PrepareShot(mode._event);
                                    break;
                            }

                            break;
                        default:
                            // do nothing
                            break;
                    }
                }
                else if (Main.manager.isCustomMode(ModGameModeType.FreePlay))
                {
                    var mode = (FreePlay)Main.manager.CurrentMode;

                    switch (type)
                    {
                        case NonVerbalCommunication.Type.Alert:
                            switch ((NonVerbalCommunication.Alert)communication.value)
                            {
                                case NonVerbalCommunication.Alert.BlueBase:
                                    FreePlay.ResetBerryDeposits(game.gameState.blueTeam);
                                    break;
                                case NonVerbalCommunication.Alert.GoldBase:
                                    FreePlay.ResetBerryDeposits(game.gameState.redTeam);
                                    break;
                                case NonVerbalCommunication.Alert.Snail:
                                    FreePlay.SwitchTeam(player, playerEntity);
                                    break;
                                case NonVerbalCommunication.Alert.Gate:
                                    FreePlay.ResetGates();
                                    break;
                            }

                            break;
                        case NonVerbalCommunication.Type.Emote:
                            switch ((NonVerbalCommunication.Emote)communication.value)
                            {
                                case NonVerbalCommunication.Emote.Anger:
                                    // Switch between Queen and Worker/Warrior
                                    if (player.type == Entity.EntityType.Queen)
                                    {
                                        FreePlay.ConvertPlayerTo(mode.matchManager, player, Entity.EntityType.Runner, RunnerBrain.attackType);
                                    }
                                    else if (player.type == Entity.EntityType.Runner || player.type == Entity.EntityType.Warrior)
                                    {
                                        FreePlay.ConvertPlayerTo(mode.matchManager, player, Entity.EntityType.Queen, QueenBrain.attackType);
                                    }
                                    break;
                                case NonVerbalCommunication.Emote.Love:
                                    // Toggle speed boost
                                    if (player.type == Entity.EntityType.Runner || player.type == Entity.EntityType.Warrior)
                                    {
                                        playerEntity.hasSpeedBoost = !playerEntity.hasSpeedBoost;
                                    }
                                    break;
                                case NonVerbalCommunication.Emote.Confused:
                                    // Toggle shield
                                    if (player.type == Entity.EntityType.Runner || player.type == Entity.EntityType.Warrior)
                                    {
                                        FreePlay.SetShield(playerEntity, !playerEntity.hasShield);
                                    }
                                    break;
                                case NonVerbalCommunication.Emote.Taunt:
                                    var cycle = new Pair<Entity.EntityType, Attack.Type>[] {
                                        new Pair<Entity.EntityType, Attack.Type>(Entity.EntityType.Warrior, Attack.Type.Sword),
                                        new Pair<Entity.EntityType, Attack.Type>(Entity.EntityType.Warrior, Attack.Type.MorningStar),
                                        new Pair<Entity.EntityType, Attack.Type>(Entity.EntityType.Warrior, Attack.Type.Laser),
                                        // new Pair<Entity.EntityType, Attack.Type>(Entity.EntityType.Warrior, Attack.Type.KFP3), // warrior when mace is spinning ?
                                        // new Pair<Entity.EntityType, Attack.Type>(Entity.EntityType.Warrior, Attack.Type.Lance), // graphics missing?
                                        new Pair<Entity.EntityType, Attack.Type>(Entity.EntityType.Runner, RunnerBrain.attackType),
                                    };

                                    var found = cycle.ToList().FindIndex(x => x._1 == player.type && x._2 == playerEntity.attackType);
                                    var selected = found < 0 ? cycle[0] : cycle[(found + 1) % cycle.Length];

                                    FreePlay.ConvertPlayerTo(mode.matchManager, player, selected._1, selected._2);

                                    break;
                            }

                            break;
                    }
                }

                return false;
            }
            return true;
        }
    }
}
