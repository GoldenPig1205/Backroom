using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Backroom.Core.Configs;
using static Backroom.Core.EventArgs.ServerEvents;
using static Backroom.Core.EventArgs.MapEvents;
using static Backroom.Core.EventArgs.PlayerEvents;

namespace Backroom
{
    public class Main : Plugin<Config>
    {
        public static Main Instance;

        public override string Name => "Backroom";
        public override string Author => "GoldenPig1205";
        public override Version Version { get; } = new(1, 0, 0);
        public override Version RequiredExiledVersion { get; } = new(1, 2, 0, 5);

        public override void OnEnabled()
        {
            base.OnEnabled();
            Instance = this;

            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;

            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Player.Left += OnLeft;
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;
            Exiled.Events.Handlers.Player.ChangedEmotion += OnChangedEmotion;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpItem;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;

            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Player.Left -= OnLeft;
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Exiled.Events.Handlers.Player.Died -= OnDied;
            Exiled.Events.Handlers.Player.TogglingNoClip -= OnTogglingNoClip;
            Exiled.Events.Handlers.Player.ChangedEmotion -= OnChangedEmotion;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpItem;

            Instance = null;
            base.OnDisabled();
        }
    }
}
