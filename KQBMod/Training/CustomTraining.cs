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
using GameLogic.Math;
using GameLogic.Animation;
using System.Security.Cryptography;
using UnityEngine.Assertions.Comparers;

namespace KQBMod.Training
{
    [HarmonyPatch(typeof(CustomMatchLobbyGameMode))]
    [HarmonyPatch("Start")]
    static class CustomMatchLobbyGameModeStart
    {
        static bool Prefix(MatchManager matchManager)
        {
            if (Main.manager.inCustomMode())
            {
                Main.Logger.Log("Starting CustomMatchLobbyGameMode");


                //foreach (GameLogic.Level level in Game.gameConfiguration.levels)
                //{
                //    Main.Logger.Log($"Level id {level.id}:{level.name} playable {level.playable} pool {level.availableForPool}");
                //}
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(MatchGameMode))]
    [HarmonyPatch("StartMatch")]
    static class GetBerryDeposits
    {
        static bool Prefix(MatchManager matchManager)
        {
            if (Main.manager.inCustomMode())
            {
                Main.Logger.Log("Getting deposit info");

                List<Entity> blueDeposits = FreePlay.GetBerryDeposits(Team.Blue);
                Main.Logger.Log($"Blue deposits");
                foreach (Entity deposit in blueDeposits)
                {
                    Main.Logger.Log($"Deposit id {deposit.id} pos {deposit.pos}");
                }

                List<Entity> goldDeposits = FreePlay.GetBerryDeposits(Team.Red);
                Main.Logger.Log($"Gold deposits");
                foreach (Entity deposit in goldDeposits)
                {
                    Main.Logger.Log($"Deposit id {deposit.id} pos {deposit.pos}");
                }
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(GameLogic.MatchGameMode))]
    [HarmonyPatch("MarkBerryDepositOccupied")]
    static class MarkBerryDepositOccupied
    {
        static bool Prefix(Game game, Entity depositingEntity, Entity berryDeposit, Entity.BerryDepositInteractionType berryDepositInteractionType)
        {
            if (Main.manager.inCustomMode())
            {
                return Main.manager.BerryOnDeposit(game, depositingEntity, berryDeposit, berryDepositInteractionType);
            }
            return true;
        }
    }

    public class CustomTraining : ModGameMode
    {
        public TrainingPack trainingPack = null;

        public LevelInfo levelInfo = null;
        MatchManager matchManager = null;

        public int currentShotIndex = 0;
        public Team.Color currentTeam = Team.Blue;
        public Shot currentShot = null;
        public volatile int _event = 0;

        NavItem.Type ModGameMode.getMenuItemType()
        {
            return NavItem.Type.Local;
        }

        string ModGameMode.getMenuItemText()
        {
            return "Custom Training";
        }

        ModGameModeType ModGameMode.getModeType()
        {
            return ModGameModeType.CustomTraining;
        }

        bool ModGameMode.StartingLobby(CustomMatchLobbyState state)
        {
            switch (Main.settings.trainingLevel)
            {
                case Settings.TrainingLevel.Tally:
                    trainingPack = TrainingPack.TallyShots();
                    break;
                default:
                case Settings.TrainingLevel.Helix:
                    trainingPack = TrainingPack.BasicHelixShots();
                    break;
            }

            currentShotIndex = 0;
            levelInfo = null; // Can't load yet until level starts

            foreach (GameLogic.Level level in Game.gameConfiguration.levels)
            {
                if (level.availableForPool && level.id != (int)trainingPack.levelId)
                {
                    level.availableForPool = false;
                }
            }

            return true;
        }

        bool ModGameMode.StartingMatch(MatchManager matchManager)
        {
            this.matchManager = matchManager;
            currentShotIndex = 0;
            currentTeam = Team.Blue;

            switch (Main.settings.shotSorting)
            {
                case Settings.ShotSorting.Shuffle:
                    trainingPack.shots.ShuffleInPlace();
                    break;
                case Settings.ShotSorting.ByType:
                    trainingPack.shots.Sort((s1, s2) =>
                    {
                        if (s1.type != s2.type)
                        {
                            return s1.type - s2.type;
                        }
                        else if (s1.slot != s2.slot)
                        {
                            return s1.slot - s2.slot;
                        }
                        else
                        {
                            // Start from inside out, top to bottom
                            var x = s2.pos.x - s1.pos.x;
                            var y = s1.pos.y - s2.pos.y;

                            return (x < 0) ? -1 : ((x > 0) ? 1 : ((y < 0) ? -1 : ((y > 0) ? 1 : 0)));
                        }
                    });

                    break;
                case Settings.ShotSorting.ByDeposit:
                    trainingPack.shots.Sort((s1, s2) =>
                    {
                        if (s1.slot != s2.slot)
                        {
                            return s1.slot - s2.slot;
                        }
                        else if (s1.type != s2.type)
                        {
                            return s1.type - s2.type;
                        }
                        else
                        {
                            // Start from inside out, top to bottom
                            var x = s2.pos.x - s1.pos.x;
                            var y = s1.pos.y - s2.pos.y;
                        
                            return (x < 0) ? -1 : ((x > 0) ? 1 : ((y < 0) ? -1 : ((y > 0) ? 1 : 0)));
                        }
                    });
                    break;
                case Settings.ShotSorting.Default:
                default:
                    break;
            }

            // Filter selected types
            trainingPack.shots = trainingPack.shots.FindAll(s => Main.settings.activeShotTypes[(int)s.type]);

            levelInfo = new LevelInfo(trainingPack.levelId);

            Main.GetPlayer().team = Team.Blue;
            Main.GetPlayerEntity().teamColor = Team.Blue;

            PrepareShot(this._event);

            return true;
        }

        private static Color DefaultColor = Color.white;

        private void SetDepositColor(Entity d, Color color)
        {
            var depositSprite = matchManager.gameObjects[d.id].entityState.gameObject.GetComponent<SpriteRenderer>();
            depositSprite.material.color = depositSprite.color = color;
        }

        public void PrepareShot(int _event)
        {
            if (_event < this._event)
            {
                return;
            }
            this._event++;

            Game game = Main.GetServerGame();
            //game.gameState.teams[currentTeam.Index()].blackTeam = true;
            //game.gameState.teams[currentTeam.OtherTeamIndex()].blackTeam = false;

            currentShot = trainingPack.shots[currentShotIndex];
            Main.Logger.Log($"Preparing shot {currentShotIndex}/{trainingPack.shots.Count} for team {currentTeam}: {currentShot.description}");

            // Remove previous entity
            Entity playerEntity = Main.GetPlayerEntity();
            FreePlay.MatchGameMode_EndEntityInteractions(game, playerEntity);
            playerEntity.collides = false;
            playerEntity.died = true;
            FreePlay.Game_RemoveEntity(game, playerEntity);

            // Update player
            Player player = Main.GetPlayer();
            player.team = currentTeam;

            // Create new player which will spawn new entity
            player = FreePlay.MatchGameMode_SpawnPlayer(game, player.actorNr, player.inputID, true, false);
            playerEntity = Main.GetPlayerEntity();

            // Reset berries
            FreePlay.ResetBerryDeposits(game.gameState.blueTeam);
            FreePlay.ResetBerryDeposits(game.gameState.redTeam);
            FreePlay.ReestBerryStacks();

            foreach (var occupied in currentShot.occupiedSlots)
            {
                Entity occupiedDeposit = levelInfo.getDeposit(currentTeam, occupied);
                Entity serverDeposit = game.gameState.GetEntityByEntityID(occupiedDeposit.id);

                serverDeposit.isActivating = false;
                serverDeposit.isOccupied = true;
                serverDeposit.interactionLayer = Entity.InteractionLayer.None;
            }

            var pos = (currentTeam == Team.Blue) ? currentShot.pos : new GameLogic.Math.Vector2(-currentShot.pos.x, currentShot.pos.y);
            playerEntity.dP = new GameLogic.Math.Vector2(0f, 0f);
            playerEntity.ddP = new GameLogic.Math.Vector2(0f, 0f);
            GameLogic.Simulation.ChangeEntityPosition(game, playerEntity, pos, true);
            Simulation.GroundEntity(game, playerEntity, playerEntity.pos);
            playerEntity.isHoldingBerry = currentShot.holdingBerry;
            playerEntity.isSpawning = false;
            playerEntity.hasShield = false;
            playerEntity.hasSpeedBoost = false;
            playerEntity.facingRight = (currentTeam == Team.Blue) ? currentShot.facingRight : !currentShot.facingRight;
            playerEntity.isActivating = false;
            playerEntity.didJump = false;
            playerEntity.isGrounded = true;

            int _eventForClosure = this._event;
            // Writing player position
            GameManager.GMInstance.Chain(new IEnumerator[] {
                CoroutineUtility.DelayForSeconds(1.0f, delegate
                {
                    if (_eventForClosure >= this._event)
                    {
                        Main.Logger.Log($"Player position #1 {playerEntity.pos}");
                    }
                }),
                CoroutineUtility.DelayForSeconds(3.0f, delegate
                {
                    if (_eventForClosure >= this._event)
                    {
                        Main.Logger.Log($"Player position #2 {playerEntity.pos}");
                    }
                }),
                CoroutineUtility.DelayForSeconds(6.0f, delegate
                {
                    if (_eventForClosure >= this._event)
                    {
                        Main.Logger.Log($"Player position #3 {playerEntity.pos}");
                    }
                }),
            });

            foreach (var d in levelInfo.getDeposits(currentTeam))
            {
                SetDepositColor(d, DefaultColor);
            }
            foreach (var d in levelInfo.getDeposits(currentTeam.OtherTeam()))
            {
                SetDepositColor(d, DefaultColor);
            }

            Entity deposit = levelInfo.getDeposit(currentTeam, currentShot.slot);
            SetDepositColor(deposit, Shot.ShotColor[currentShot.type]);
        }

        public void PreviousShot(int _event)
        {
            if (_event < this._event)
            {
                return;
            }
            this._event++;

            Game game = Main.GetServerGame();

            if (Main.settings.customTrainingInterleaved)
            {
                Main.Logger.Log("Previous interleaved");
                if (currentTeam == Team.Red)
                {
                    currentTeam = Team.Blue;
                }
                else
                {
                    currentShotIndex--;
                    if (currentShotIndex < 0)
                    {
                        currentShotIndex = 0;
                    }
                    else
                    {
                        currentTeam = Team.Red;
                    }

                }
            }
            else
            {
                Main.Logger.Log("Previous non-interleaved");
                currentShotIndex--;
                if (currentShotIndex < 0)
                {
                    currentShotIndex = 0;
                    if (currentTeam == Team.Red)
                    {
                        currentTeam = Team.Blue;
                        currentShotIndex = trainingPack.shots.Count() - 1;
                    }
                }
            }

            PrepareShot(this._event);
        }

        public void NextShot(int _event, bool allowWin)
        {
            if (_event < this._event)
            {
                return;
            }

            Game game = Main.GetServerGame();

            if (Main.settings.customTrainingInterleaved)
            {
                Main.Logger.Log("NextShot Interleaved");
                if (currentTeam == Team.Blue)
                {
                    currentTeam = Team.Red;
                    PrepareShot(_event);
                }
                else
                {
                    if (currentShotIndex + 1 < trainingPack.shots.Count())
                    {
                        currentShotIndex++;
                        currentTeam = Team.Blue;
                        PrepareShot(_event);
                    }
                    else if (!allowWin)
                    {
                        // Reset shot without going to next
                        PrepareShot(_event);
                    }
                    else
                    {
                        currentShotIndex = 0;
                        currentShot = null;

                        // Trigger economic win condition
                        Team team = currentTeam == Team.Blue ? game.gameState.blueTeam : game.gameState.redTeam;

                        currentTeam = Team.Blue;
                        team.berryCount = team.berryDepositIDs.Count;
                    }
                }
            }
            else
            {
                Main.Logger.Log("NextShot Non interleaved");
                if (currentShotIndex + 1 < trainingPack.shots.Count())
                {
                    currentShotIndex++;
                    PrepareShot(_event);
                }
                else if (currentTeam == Team.Blue)
                {
                    currentTeam = Team.Red;
                    currentShotIndex = 0;
                    PrepareShot(_event);
                }
                else if (!allowWin)
                {
                    // Reset shot without going to next
                    PrepareShot(_event);
                }
                else
                {
                    currentShotIndex = 0;
                    currentShot = null;

                    // Trigger economic win condition
                    Team team = currentTeam == Team.Blue ? game.gameState.blueTeam : game.gameState.redTeam;

                    team.berryCount = team.berryDepositIDs.Count;
                }
            }

        }

        private Entity getCurrentDeposit()
        {
            return levelInfo.getDeposit(currentTeam, currentShot.slot);
        }

        bool ModGameMode.BerryOnDeposit(Game game, Entity depositingEntity, Entity berryDeposit, Entity.BerryDepositInteractionType berryDepositInteractionType)
        {
            double startTime = game.gameState.totalTime;

            int _event = this._event;

            if (berryDeposit.id == getCurrentDeposit().id && berryDepositInteractionType == Entity.BerryDepositInteractionType.LooseDeposit)
            {
                Main.Logger.Log("Accepting deposit");
                // Introduce delay to make more pleasant
                GameManager.GMInstance.Chain(new IEnumerator[] {
                    CoroutineUtility.DelayForSeconds(0.5f, delegate
                    {
                        // Ignore event if update occured already
                        if (Main.settings.moveToNextOnSucceess)
                        {
                            NextShot(_event, true);
                        }
                        else
                        {
                            PrepareShot(_event);
                        }
                    }),
                });
                return true;
            }
            else
            {
                Main.Logger.Log("Rejecting deposit");

                // Introduce delay to make more pleasant
                GameManager.GMInstance.Chain(new IEnumerator[] {
                    CoroutineUtility.DelayForSeconds(0.5f, delegate
                    {
                        PrepareShot(_event);
                    }),
                });
            }

            return true;
        }
    }
}
