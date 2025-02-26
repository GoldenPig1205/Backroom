using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Backroom.Core.Variables.Base;
using static Backroom.Core.Functions.Base;
using System.Diagnostics.Eventing.Reader;
using InventorySystem.Items;
using MultiBroadcast.API;
using PlayerRoles.FirstPersonControl.Thirdperson.Subcontrollers;
using Exiled.API.Features.DamageHandlers;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Toys;
using InventorySystem.Items.Firearms.Attachments;
using MapEditorReborn.API.Enums;
using MapEditorReborn.API;
using Exiled.API.Features.Items;
using RemoteAdmin;
using static System.Net.Mime.MediaTypeNames;
using Backroom.Core.Classes;

namespace Backroom.Core.EventArgs
{
    public static class PlayerEvents
    {
        public static void OnVerified(VerifiedEventArgs ev)
        {
            ev.Player.Role.Set(RoleTypeId.Tutorial);

            if (!AudioPlayers.ContainsKey(ev.Player))
            {
                AudioPlayer audioPlayer = AudioPlayer.CreateOrGet($"Player - {ev.Player.UserId}", condition: (hub) =>
                {
                    return hub == ev.Player.ReferenceHub;
                }
                , onIntialCreation: (p) =>
                {
                    p.transform.parent = ev.Player.GameObject.transform;

                    Speaker speaker = p.AddSpeaker("Main", isSpatial: false, minDistance: 0, maxDistance: 5000);

                    speaker.transform.parent = ev.Player.GameObject.transform;
                    speaker.transform.localPosition = Vector3.zero;
                });

                AudioPlayers.Add(ev.Player, audioPlayer);
            }
        }

        public static void OnLeft(LeftEventArgs ev)
        {
        }

        public static void OnSpawned(SpawnedEventArgs ev)
        {
            if (ev.Player.IsAlive)
            {
                GodModePlayers.Add(ev.Player);

                Timing.CallDelayed(5, () =>
                {
                    if (GodModePlayers.Contains(ev.Player))
                        GodModePlayers.Remove(ev.Player);
                });

                ev.Player.Position = FirstSpawnPoint.position;
                ev.Player.EnableEffect(EffectType.FogControl, 1);
                ev.Player.EnableEffect(EffectType.SoundtrackMute, 1);
                ev.Player.EnableEffect(EffectType.SilentWalk, 3);
            }
        }

        public static void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker == null)
                return;

            if (!ev.Attacker.IsNPC && GodModePlayers.Contains(ev.Player))
                ev.IsAllowed = false;
        }

        public static IEnumerator<float> OnDied(DiedEventArgs ev)
        {
            if (ev.Attacker != null)
            {
            }

            for (int i=0; i<5; i++)
            {
                ev.Player.ShowHint($"{5 - i}초 뒤 부활합니다.");

                yield return Timing.WaitForSeconds(1);
            }

            ev.Player.Role.Set(RoleTypeId.Tutorial);

            Timing.CallDelayed(Timing.WaitForOneFrame, () =>
            {
                ev.Player.Position = FirstSpawnPoint.position;
            });
        }

        public static void OnTogglingNoClip(TogglingNoClipEventArgs ev)
        {
            if (ev.Player.IsHuman && !ev.Player.IsCuffed)
            {
                if (TryGetLookPlayer(ev.Player, 2f, out Player player, out RaycastHit? hit1))
                {
                    if (ev.Player != player && !MeleeCooldowns.Contains(ev.Player) && !GodModePlayers.Contains(player))
                    {
                        float damageCalcu(string pos)
                        {
                            switch (pos)
                            {
                                case "Head":
                                    return 24.1f;

                                case "Chest":
                                    return 14f;

                                default:
                                    return 12.5f;
                            }
                        }

                        float damage = damageCalcu(hit1.Value.transform.name);

                        ev.Player.ShowHitMarker(damage / 14);
                        player.Hurt(ev.Player, damage, DamageType.Custom, new DamageHandlerBase.CassieAnnouncement("") { Announcement = null, SubtitleParts = null }, "무지성으로 뚜드려 맞았습니다.");

                        MeleeCooldowns.Add(ev.Player);

                        Timing.CallDelayed(1, () =>
                        {
                            MeleeCooldowns.Remove(ev.Player);
                        });
                    }
                }
            }
        }

        public static void OnChangedEmotion(ChangedEmotionEventArgs ev)
        {
            if (!EmotionCooldowns.Contains(ev.Player))
            {
                EmotionCooldowns.Add(ev.Player);

                EmotionPresetType type = ev.EmotionPresetType;

                if (type == EmotionPresetType.Neutral)
                    return;

                string emotion()
                {
                    if (type == EmotionPresetType.Happy)
                        return "행복한 표정을 짓고 있습니다";

                    else if (type == EmotionPresetType.AwkwardSmile)
                        return "뒤틀린 미소를 짓고 있습니다";

                    else if (type == EmotionPresetType.Scared)
                        return "두려운 표정을 짓고 있습니다";

                    else if (type == EmotionPresetType.Angry)
                        return "화가난 표정을 짓고 있습니다";

                    else if (type == EmotionPresetType.Chad)
                        return "꼭 채드처럼 보이는군요";

                    else
                        return "꼭 오우거같이 보이는군요";
                }

                foreach (var player in Player.List.Where(x => x.IsDead || Vector3.Distance(x.Position, ev.Player.Position) < 11))
                    player.AddBroadcast(5, $"<size=20>{BadgeFormat(ev.Player)}<color={ev.Player.Role.Color.ToHex()}>{ev.Player.DisplayNickname}</color>(은)는 {emotion()}.</size>");
            }
        }

        public static void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
        }
    }
}
