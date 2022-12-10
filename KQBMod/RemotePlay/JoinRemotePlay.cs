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

namespace KQBMod.Remote
{
    public class JoinRemotePlay : ModGameMode
    {
        NavItem.Type ModGameMode.getMenuItemType()
        {
            return NavItem.Type.Local;
        }

        string ModGameMode.getMenuItemText()
        {
            return "Join Custom Match";
        }

        ModGameModeType ModGameMode.getModeType()
        {
            return ModGameModeType.JoinRemotePlay;
        }

        bool ModGameMode.StartingLobby(CustomMatchLobbyState state)
        {
            return true;
        }

        bool ModGameMode.StartingMatch(MatchManager matchManager)
        {
            return true;
        }

        bool ModGameMode.IsSpectator()
        {
            return false;
        }

        public bool BerryOnDeposit(Game game, Entity depositingEntity, Entity berryDeposit, Entity.BerryDepositInteractionType berryDepositInteractionType)
        {
            return true;
        }
    }
}
