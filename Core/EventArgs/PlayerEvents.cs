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
using Exiled.API.Features.Roles;
using Respawning;
using PlayerRoles.FirstPersonControl;
using Exiled.Events.Commands.Hub;
using RelativePositioning;
using Exiled.API.Extensions;

namespace Backroom.Core.EventArgs
{
    public static class PlayerEvents
    {
        public static IEnumerator<float> OnVerified(VerifiedEventArgs ev)
        {
            ev.Player.Role.Set(RoleTypeId.NtfPrivate);

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

            PlayerStatuses.Add(ev.Player, new PlayerStatus
            {
                IsSitDown = false,
                IsChangingSitDownState = false
            });

            AudioPlayers[ev.Player].RemoveAllClips();
            ev.Player.ClearInventory();

            foreach (var item in new List<ItemType>()
                {
                    ItemType.Flashlight,
                    ItemType.Radio,
                    ItemType.Coin,
                    ItemType.Coin,
                    ItemType.Medkit,
                })
            {
                ev.Player.AddItem(item);
            }

            AudioPlayers[ev.Player].AddClip("military-radio-communication-222904");

            ev.Player.Teleport(Exiled.API.Features.Doors.Door.List.Select(x => x.Type).GetRandomValue());
            ev.Player.CurrentItem = ev.Player.Items.FirstOrDefault(x => x.Type == ItemType.Flashlight);

            ev.Player.ShowHint("\n\n\n\n\n\n<size=20><color=#0080FF>모리스 사령관</color>: 아, 아, Site-02 기지에는 도착했나?</size>", 3);

            yield return Timing.WaitForSeconds(4);

            ev.Player.ShowHint("\n\n\n\n\n\n<size=20><color=#0080FF>모리스 사령관</color>: (잡음) 교신 상태가 좋지 않은 것 같은데</size>", 4);

            yield return Timing.WaitForSeconds(4.5f);

            ev.Player.ShowHint("\n\n\n\n\n\n<size=20><color=#00BFFF>제임스 중대장</color>: 여기는 Alpha-1,</size>", 1);

            yield return Timing.WaitForSeconds(1.5f);

            ev.Player.ShowHint("\n\n\n\n\n\n<size=20><color=#0080FF>모리스 사령관</color>: 어, 그래. 중대장, 보고할 것이라도 발견했나?</size>", 3);

            yield return Timing.WaitForSeconds(3.5f);

            ev.Player.ShowHint("\n\n\n\n\n\n<size=20><color=#00BFFF>제임스 중대장</color>: 그게.. 유기체가 아예 보이지 않습니다. 과학자와 SCP가..</size>", 2);

            yield return Timing.WaitForSeconds(2.5f);

            ev.Player.ShowHint("\n\n\n\n\n\n<size=20><color=#0080FF>모리스 사령관</color>: 그게 무ㅅ(!#($*^$!^&(</size>", 1);

            yield return Timing.WaitForSeconds(1);

            
            
            for (int i = 0; i < 100; i++)
            {
                Vector3 pos = ev.Player.Position;

                ev.Player.Position = new Vector3(pos.x, pos.y - 0.01f, pos.z);

                yield return Timing.WaitForSeconds(0.01f);
            }
            ev.Player.Kill("ㅋ");
        }

        public static void OnLeft(LeftEventArgs ev)
        {
        }

        public static void OnSpawned(SpawnedEventArgs ev)
        {
        }

        public static void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker == null)
                return;

            if (!ev.Attacker.IsNPC && GodModePlayers.Contains(ev.Player))
                ev.IsAllowed = false;

            if (ev.DamageHandler.Type == DamageType.Falldown)
                ev.DamageHandler.Damage /= 3;
        }

        public static void OnDying(DyingEventArgs ev)
        {
            ev.IsAllowed = false;
            
            ev.Player.DisableAllEffects();
            ev.Player.EnableEffect(EffectType.SilentWalk, 7);
            ev.Player.EnableEffect(EffectType.SoundtrackMute, 1);
            ev.Player.EnableEffect(EffectType.Blinded, 60);
            ev.Player.EnableEffect(EffectType.FogControl, 1);
            ev.Player.ClearInventory();

            FirstPersonMovementModule fpcModule = (ev.Player.ReferenceHub.roleManager.CurrentRole as FpcStandardRoleBase).FpcModule;
            fpcModule.Noclip.IsActive = true;

            foreach (var item in new List<ItemType>()
                {
                    ItemType.Flashlight,
                    ItemType.Radio,
                    ItemType.Coin,
                    ItemType.Coin,
                    ItemType.Medkit,
                })
            {
                ev.Player.AddItem(item);
            }

            fpcModule.Noclip.IsActive = false;

            ev.Player.Health = 100;
            ev.Player.Position = FirstSpawnPoint.position;
        }

        public static IEnumerator<float> OnTogglingNoClip(TogglingNoClipEventArgs ev)
        {
            if (!PlayerStatuses[ev.Player].IsChangingSitDownState && !ev.Player.IsJumping && !ev.Player.IsNoclipPermitted && ev.Player.IsHuman)
            {
                PlayerStatuses[ev.Player].IsChangingSitDownState = true;

                AudioPlayer audioPlayer = AudioPlayer.CreateOrGet($"Player {ev.Player.Nickname}", onIntialCreation: (p) =>
                {
                    Speaker speaker = p.AddSpeaker("Main", maxDistance: 3);

                    p.transform.parent = ev.Player.GameObject.transform;

                    speaker.transform.parent = ev.Player.GameObject.transform;

                    speaker.transform.localPosition = Vector3.zero;
                });

                if (PlayerStatuses[ev.Player].IsSitDown)
                {
                    audioPlayer.AddClip($"standing");

                    while (ev.Player.Scale.y >= 0.65f)
                    {
                        ev.Player.Scale = new Vector3(1, ev.Player.Scale.y - 0.01f, 1);

                        yield return Timing.WaitForOneFrame;
                    }

                    ev.Player.EnableEffect(EffectType.Slowness, 50);
                    ev.Player.EnableEffect(EffectType.SilentWalk, 10);

                    PlayerStatuses[ev.Player].IsSitDown = false;
                }
                else
                {
                    audioPlayer.AddClip($"sitting");

                    while (ev.Player.Scale.y <= 1)
                    {
                        ev.Player.Scale = new Vector3(1, ev.Player.Scale.y + 0.01f, 1);

                        yield return Timing.WaitForOneFrame;
                    }

                    ev.Player.DisableEffect(EffectType.Slowness);
                    ev.Player.EnableEffect(EffectType.SilentWalk, 7);

                    PlayerStatuses[ev.Player].IsSitDown = true;
                }

                PlayerStatuses[ev.Player].IsChangingSitDownState = false;
            }

            /*
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
            */
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
