using UnityEngine;
using UnityModManagerNet;
using UnityEngine.UI;
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
using System.Security.Policy;
using LiquidBit.KillerQueenX.Extensions;

namespace KQBMod
{

    [HarmonyPatch(typeof(GameLogic.Utility.GameStateSerializer))]
    [HarmonyPatch("DeltaCompressGameState")]
    [HarmonyPatch(new Type[] { typeof(GameLogic.Utility.BitStreamWriter), typeof(int), typeof(Game), typeof(GameState), typeof(GameState), typeof(bool) })]
    static class BeforeSendingState
    {
        static bool Prefix(GameLogic.Utility.BitStreamWriter stream, int actorNr, Game game, GameState from, GameState to, bool full)
        {
            if (Main.manager.isCustomMode(ModGameModeType.FreePlay) && Main.settings.showHitboxes)
            {
                Game.gameConfiguration.debugParams.alwaysShowHitBoxes = Main.settings.showHitboxes;
                Game.gameConfiguration.debugParams.alwaysShowHurtBoxes = Main.settings.showHitboxes;
                //Game.gameConfiguration.debugParams.alwaysShowCollisionBoxes = Main.settings.showHitboxes;
                // Game.gameConfiguration.debugParams.alwaysShowJoustHurtBoxes = Main.settings.showHitboxes;

                foreach (var e in Main.GetServerGame().gameState.entities)
                {
                    if (e != null)
                    {
                        switch (e.entityType)
                        {
                            case Entity.EntityType.HitBox:
                            case Entity.EntityType.AttackVolume:
                                e.noSerialize = false;
                                break;
                            default:
                                // do nothing
                                break;
                        }
                    }
                }
            }
            else if (!Main.manager.isRemote())
            {
                //Game.gameConfiguration.debugParams.alwaysShowHitBoxes = false;
                //Game.gameConfiguration.debugParams.alwaysShowHurtBoxes = false;
                //Game.gameConfiguration.debugParams.alwaysShowCollisionBoxes = false;
                //Game.gameConfiguration.debugParams.alwaysShowJoustHurtBoxes = false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(MatchManager))]
    [HarmonyPatch("UpdateLocalGameState")]
    static class AfterUpdateLocalGameState
    {
        static void Postfix(GameState newGameState)
        {
            if (Main.manager.isCustomMode(ModGameModeType.FreePlay) && Main.settings.showHitboxes)
            {
                foreach (var objs in MatchManager.Instance.gameObjects.Values)
                {
                    var e = objs.entityState.GetCurrentEntity();

                    if (e.entityType == Entity.EntityType.AttackVolume)
                    {
                        KQBMod.Training.AttackVolumeVisual vis = null;
                        if (!objs.entityState.gameObject.TryGetComponent<KQBMod.Training.AttackVolumeVisual>(out vis))
                        {
                            objs.entityState.gameObject.AddComponent<KQBMod.Training.AttackVolumeVisual>();
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(TransformGate))]
    [HarmonyPatch("Update")]
    static class UntagGates
    {
        static bool Prefix(TransformGate __instance, ref Team.Color ___lastColor)
        {
            if (Main.manager.isCustomMode(ModGameModeType.FreePlay))
            {
                Entity entityStateForVisuals = __instance.entityState.entityStateForVisuals;
                if (___lastColor != entityStateForVisuals.teamColor)
                {
                    Entity gateEntity = Main.GetServerGame().gameState.GetEntityByEntityID(entityStateForVisuals.id);
                    // Gate got tagged, so queue untagging
                    GameManager.GMInstance.Chain(new IEnumerator[] {
                        CoroutineUtility.DelayForSeconds(3.0f, delegate
                        {
                            gateEntity.teamColor = Team.Color.None;
                        })
                    });
                }
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(GameLogic.MatchGameMode))]
    [HarmonyPatch("RemoveBerryFromStack")]
    static class InfiniteBerries
    {
        static void Postfix(Game game, Entity berryStack)
        {
            if (Main.manager.isCustomMode(ModGameModeType.FreePlay) && game.isServer)
            {
                berryStack.berryCount++;
            }
        }
    }

    [HarmonyPatch(typeof(GameLogic.MatchGameMode))]
    [HarmonyPatch("CheckToStartGame")]
    static class StartImmediately
    {
        static bool Prefix(Game game)
        {
            if ((Main.manager.isCustomMode(ModGameModeType.FreePlay) || Main.manager.isCustomMode(ModGameModeType.CustomTraining)) && game.isServer)
            {
                // Time to start is now!
                game.gameState.serverTimeToStartGame = game.gameState.serverTime;
            }

            return true;
        }
    }


    [HarmonyPatch(typeof(MatchManager))]
    [HarmonyPatch("OnExitMatchClicked")]
    static class FreePlayExit
    {
        static bool Prefix(MatchManager __instance)
        {
            if (Main.manager.isCustomMode(ModGameModeType.FreePlay))
            {
                Main.Logger.Log("Exiting Free Play");
                GameManager.GMInstance.Quit();
                return false;
            }

            return true;
        }
    }

    public class FreePlay : ModGameMode
    {
        public MatchManager matchManager;

        // provides access for internal method via reflection
        public static Player MatchGameMode_SpawnPlayer(Game game, int actorNr, int inputID, bool skipSpawn = false, bool makeInvincible = true)
        {
            Type type = typeof(GameLogic.MatchGameMode);
            object result = type.GetMethod("SpawnPlayer", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { game, actorNr, inputID, skipSpawn, makeInvincible });
            return result as Player;
        }

        // provides access for internal method via reflection
        public static void MatchGameMode_EndEntityInteractions(Game game, Entity entity)
        {
            Type type = typeof(GameLogic.MatchGameMode);
            type.GetMethod("EndEntityInteractions", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { game, entity });
        }

        // provides access for internal method via reflection
        public static void Game_RemoveEntity(Game game, Entity entity)
        {
            Traverse.Create(game).Method("RemoveEntity", new object[] { entity }).GetValue();
        }

        // provides access for internal method via reflection
        public static void MatchGameMode_RespawnPlayer(Game game, Entity entity, float startSpawnTimeDelay, bool skipSpawn, bool makeInvincible)
        {
            Type type = typeof(GameLogic.MatchGameMode);
            type.GetMethod("RespawnPlayer", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { game, entity, startSpawnTimeDelay, skipSpawn, makeInvincible });
        }

        // provides access for internal method via reflection
        internal static Entity Game_AddRunnerEntity(Game game, int actorNr, int inputID, Team.Color teamColor)
        {
            Type type = typeof(GameLogic.Game);
            object result = type.GetMethod("AddRunnerEntity", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(game, new object[] { actorNr, inputID, teamColor });
            return result as Entity;
        }

        // provides access for internal method via reflection
        public static void MatchGameMode_ChangeEntityType(Game game, Entity entity, Entity.EntityType entityType, Attack.Type attackType = Attack.Type.None)
        {
            Type type = typeof(GameLogic.MatchGameMode);
            type.GetMethod("ChangeEntityType", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { game, entity, entityType, attackType });
        }

        // provides access for internal method via reflection
        public static void MatchGameMode_DisengageSnail(Game game, Entity ridingEntity)
        {
            Type type = typeof(GameLogic.MatchGameMode);
            type.GetMethod("DisengageSnail", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { game, ridingEntity });
        }

        public static void ConvertPlayerTo(MatchManager matchManager, Player player, Entity.EntityType entityType, Attack.Type attackType)
        {
            if (Main.manager.inCustomMode())
            {
                Game game = Main.GetServerGame();

                Entity entityByActorNr = Main.GetPlayerEntity(player);
                FreePlay.MatchGameMode_EndEntityInteractions(game, entityByActorNr);
                entityByActorNr.collides = false;
                FreePlay.Game_RemoveEntity(game, entityByActorNr);

                // Create new player which will spawn new entity
                player = FreePlay.MatchGameMode_SpawnPlayer(game, player.actorNr, player.inputID, true, false);
                entityByActorNr = Main.GetPlayerEntity(player);

                // Disengage from Snail if neccessary
                if (entityByActorNr.isRidingSnail)
                {
                    MatchGameMode_DisengageSnail(game, entityByActorNr);
                }

                // Swapping by holding shield or speed stays on Queen.
                if (entityType == Entity.EntityType.Queen)
                {
                    SetShield(entityByActorNr, false);
                    entityByActorNr.hasSpeedBoost = false;
                }
                MatchGameMode_ChangeEntityType(game, entityByActorNr, entityType, attackType);
                player.type = entityType;
                // Swapping from Queen to Worker or Warrior leaves skin as None.
                if ((entityType == Entity.EntityType.Runner) || (entityType == Entity.EntityType.Warrior))
                {
                    // Rotate skin to force reloading of graphics
                    Main.Logger.Log($"Skin was {player.entitySkin}");
                    player.entitySkin = player.entitySkin + 1;
                    if (player.entitySkin == Entity.EntitySkin.End)
                    {
                        player.entitySkin = Entity.EntitySkin.Stripes;
                    }
                    Main.Logger.Log($"Switching sking to {player.entitySkin}");
                    entityByActorNr.isRidingSnail = false;
                }

                entityByActorNr.AssignPlayerInteractionLayer();
                Main.GetServerGame().world.MarkEntityDirty(entityByActorNr);
            }
        }

        public static List<Entity> GetBerryDeposits(Team.Color team)
        {
            List<Entity> deposits = new List<Entity>();

            if (Main.manager.inCustomMode())
            {
                Game game = Main.GetServerGame();

                for (int i = 0; i < game.gameState.entities.Length; i++)
                {
                    Entity entity = game.gameState.entities[i];

                    if (entity.inUse && entity.entityType == Entity.EntityType.BerryDeposit && (entity.teamColor == team))
                    {
                        deposits.Add(entity);
                    }
                }
            }

            return deposits;
        }

        public static void ReestBerryStacks()
        {
            if (Main.manager.inCustomMode())
            {
                Game game = Main.GetServerGame();
                List<Entity> stacks = new List<Entity>();
                for (int i = 0; i < game.gameState.entities.Length; i++)
                {
                    Entity entity = game.gameState.entities[i];
                    if (entity.inUse && entity.entityType == Entity.EntityType.BerryStack)
                    {
                        entity.berryCount = 6;
                    }
                }
            }
        }

        public static void ResetBerryDeposits(Team team)
        {
            if (Main.manager.inCustomMode())
            {
                Game game = Main.GetServerGame();

                List<Entity> deposits = new List<Entity>();
                for (int i = 0; i < game.gameState.entities.Length; i++)
                {
                    Entity entity = game.gameState.entities[i];
                    if (entity.inUse && entity.entityType == Entity.EntityType.BerryDeposit && (team.color == Team.Color.None || entity.teamColor == team.color))
                    {
                        deposits.Add(entity);
                    }
                    else if (entity.inUse && entity.entityType == Entity.EntityType.Berry)
                    {
                        game.gameState.RemoveEntity(entity.id);
                    }
                }

                foreach (Entity deposit in deposits)
                {
                    if (deposit.isOccupied)
                    {
                        team.berryCount--;
                        deposit.isOccupied = false;
                        deposit.isActivating = false;
                    }

                    if (team.color == Team.Color.Blue)
                    {
                        deposit.interactionLayer = Entity.InteractionLayer.BlueBerryDeposit;
                    }
                    else
                    {
                        deposit.interactionLayer = Entity.InteractionLayer.GoldBerryDeposit;
                    }
                }
                team.lastBerryDepositEntityId = 0U;
            }
        }

        public static void ResetGates()
        {
            if (Main.manager.inCustomMode())
            {
                Game game = Main.GetServerGame();

                List<Entity> gates = new List<Entity>();
                for (int i = 0; i < game.gameState.entities.Length; i++)
                {
                    Entity entity = game.gameState.entities[i];
                    if (entity.inUse && entity.entityType == Entity.EntityType.TransformGate)
                    {
                        entity.teamColor = Team.Color.None;
                    }
                }
            }
        }

        public static void SetShield(Entity playerEntity, bool hasShield)
        {
            if (Main.manager.inCustomMode())
            {
                if (playerEntity.hasShield != hasShield)
                {
                    Game game = Main.GetServerGame();

                    if (playerEntity.hasShield)
                    {
                        Entity attackVolume = game.GetAttackVolume(playerEntity, 0);
                        attackVolume.animationData[0].Initialize();
                    }
                    playerEntity.hasShield = hasShield;
                }
            }
        }

        public static void SwitchTeam(Player player, Entity playerEntity)
        {
            if (Main.manager.inCustomMode())
            {
                Team.Color otherTeam = playerEntity.teamColor.OtherTeam();
                GameLogic.Math.Vector2 pos = new GameLogic.Math.Vector2(playerEntity.pos.x, playerEntity.pos.y);

                Game game = Main.GetServerGame();
                FreePlay.MatchGameMode_EndEntityInteractions(game, playerEntity);
                if (playerEntity.isRidingSnail)
                {
                    MatchGameMode_DisengageSnail(game, playerEntity);
                }
                FreePlay.Game_RemoveEntity(game, playerEntity);
                playerEntity.died = false;

                // Create new player which will spawn new entity
                player = FreePlay.MatchGameMode_SpawnPlayer(game, player.actorNr, player.inputID, true, false);
                playerEntity = Main.GetPlayerEntity(player);

                player.team = player.team.OtherTeam();
                playerEntity.teamColor = otherTeam;
                GameLogic.Simulation.ChangeEntityPosition(game, playerEntity, pos, true);

                Main.Logger.Log($"Skin was {player.entitySkin}");

                // Rotate skin to force reloading graphics
                if (playerEntity.entityType == Entity.EntityType.Queen)
                {
                    player.entitySkin = Entity.EntitySkin.None;
                    Main.Logger.Log($"Setting skin to {player.entitySkin}");
                }
                else
                {
                    player.entitySkin = player.entitySkin + 1;
                    if (player.entitySkin >= Entity.EntitySkin.End)
                    {
                        player.entitySkin = Entity.EntitySkin.Stripes;
                    }
                    Main.Logger.Log($"Setting skin to {player.entitySkin}");
                }
            }
        }

        NavItem.Type ModGameMode.getMenuItemType()
        {
            return NavItem.Type.Local;
        }

        string ModGameMode.getMenuItemText()
        {
            return "Free Play";
        }

        ModGameModeType ModGameMode.getModeType()
        {
            return ModGameModeType.FreePlay;
        }

        bool ModGameMode.StartingLobby(CustomMatchLobbyState state)
        {
            return true;
        }

        bool ModGameMode.StartingMatch(MatchManager matchManager)
        {
            this.matchManager = matchManager;

            return true;
        }

        bool ModGameMode.BerryOnDeposit(Game game, Entity depositingEntity, Entity berryDeposit, Entity.BerryDepositInteractionType berryDepositInteractionType)
        {
            return true;
        }
    }
}
