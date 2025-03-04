using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Backroom.Core.Classes;

namespace Backroom.Core.Variables
{
    public static class Base
    {
        public static AudioPlayer GlobalPlayer;
        public static Transform FirstSpawnPoint;
        public static SchematicObject BackroomSchematic;

        public static List<Player> MeleeCooldowns = new List<Player>();
        public static List<Player> EmotionCooldowns = new List<Player>();
        public static List<Player> ChatCooldowns = new List<Player>();
        public static List<Player> IntercomPlayers = new List<Player>();
        public static List<Player> GodModePlayers = new List<Player>();
        public static List<Player> JumpScareCooldown = new List<Player>();
        public static List<Player> SoundTrackMutePlayers = new List<Player>();

        public static Dictionary<Player, AudioPlayer> AudioPlayers = new Dictionary<Player, AudioPlayer> { };
        public static Dictionary<string, List<string>> Audios = new Dictionary<string, List<string>> 
        {
            { "BGMs", new List<string> { } },
            { "SEs", new List<string> { } },
            { "Audios", new List<string> { } }
        };
        public static Dictionary<Player, float> OnGround = new Dictionary<Player, float>();
        public static Dictionary<Player, PlayerStatus> PlayerStatuses = new Dictionary<Player, PlayerStatus>();
    }
}
