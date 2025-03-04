using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MultiBroadcast.API;
using PlayerRoles;
using UnityEngine;
using static Backroom.Core.Variables.Base;

namespace Backroom.Core.Commands.ClientCommands.basicfeatures
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class SoundTrackMute : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (SoundTrackMutePlayers.Contains(player))
            {
                SoundTrackMutePlayers.Remove(player);
                response = "배경 음악이 켜졌습니다.";
            }
            else
            {
                SoundTrackMutePlayers.Add(player);
                response = "배경 음악이 꺼졌습니다.";
            }

            return true;
        }

        public string Command { get; } = "bgm";

        public string[] Aliases { get; } = { "사운드트랙", "배경음악" };

        public string Description { get; } = "[Backroom] 배경 음악을 끄고 켤 수 있습니다.";

        public bool SanitizeResponse { get; } = true;
    }
}