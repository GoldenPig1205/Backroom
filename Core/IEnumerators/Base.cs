using Exiled.API.Extensions;
using Exiled.API.Features;
using MEC;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Backroom.Core.Variables.Base;
using static Backroom.Core.Functions.Base;
using InventorySystem.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Items;
using PluginAPI.Events;
using PlayerRoles.FirstPersonControl;
using PlayerRoles;
using System.Threading;

namespace Backroom.Core.IEnumerators
{
    public static class Base
    {
        public static IEnumerator<float> BGM()
        {
            while (true)
            {
                foreach (var player in Player.List.Where(x => x.IsAlive && !x.IsNPC && x.IsHuman))
                {
                    try
                    {
                        if (SoundTrackMutePlayers.Contains(player))
                        {
                            if (AudioPlayers[player].ClipsById.Count() > 0)
                                AudioPlayers[player].RemoveAllClips();
                        }
                        else if (Physics.Raycast(player.Position, Vector3.down, out RaycastHit hit, 10, (LayerMask)1))
                        {
                            if (hit.transform.name.StartsWith("BGM"))
                            {
                                string name = hit.transform.name.Split('/')[1];
                                bool flag = AudioPlayers[player].ClipsById.TryGetValue(0, out AudioClipPlayback value);

                                if (flag)
                                {
                                    if (value.Clip != name)
                                    {
                                        AudioPlayers[player].RemoveAllClips();
                                        AudioPlayers[player].AddClip(name, 0.2f);
                                    }
                                }
                                else
                                {
                                    AudioPlayers[player].AddClip(name, 0.2f);
                                }
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        Log.Error($"BGM Error: {e}");
                    }
                }

                yield return Timing.WaitForOneFrame;
            }
        }

        public static IEnumerator<float> ClearDecals()
        {
            Map.CleanAllRagdolls();
            Map.CleanAllItems();

            while (true)
            {
                List<string> list = new List<string> 
                {
                    "bulletholes",
                    "blood",
                    "ragdolls",
                    "items"
                };

                foreach (var s in list)
                {
                    Server.ExecuteCommand($"/cleanup {s}");
                }

                yield return Timing.WaitForSeconds(300);
            }
        }

        public static IEnumerator<float> InputCooldown()
        {
            while (true)
            {
                ChatCooldowns.Clear();
                EmotionCooldowns.Clear();

                yield return Timing.WaitForSeconds(2f);
            }
        }

        public static IEnumerator<float> IsFallDown()
        {
            while (true)
            {
                foreach (var player in Player.List.Where(x => x.IsAlive))
                {
                    if (OnGround.ContainsKey(player) && !player.IsNoclipPermitted && player.Role.Type != RoleTypeId.Scp079)
                    {
                        if (FpcExtensionMethods.IsGrounded(player.ReferenceHub))
                            OnGround[player] = 5;
                        else
                        {
                            OnGround[player] -= 0.1f;

                            if (OnGround[player] <= 0)
                            {
                                player.Kill("공허에 빨려들어갔습니다. (5초 이상 낙하)");

                                OnGround[player] = 5;
                            }
                        }
                    }
                }

                yield return Timing.WaitForSeconds(0.1f);
            }
        }

        public static IEnumerator<float> ItemSpawner()
        {
            while (true)
            {
                try
                {
                    switch (Random.Range(1, 21))
                    {
                        case 1:
                            Item medical = Item.Create(EnumToList<ItemType>().GetRandomValue(x => new List<ItemType>() { ItemType.Medkit, ItemType.Painkillers, ItemType.Adrenaline }.Contains(x)));

                            medical.CreatePickup(GetRandomLocation(), new Quaternion(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180)));
                            break;

                        case 2:
                            Item coin = Item.Create(ItemType.Coin);

                            coin.CreatePickup(GetRandomLocation(), new Quaternion(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180)));
                            break;
                    }
                }
                catch { }

                yield return Timing.WaitForSeconds(1);
            }
        }

        public static IEnumerator<float> HumanLoop()
        {
            while (!Round.IsEnded)
            {
                foreach (var player in Player.List)
                {
                    if (player.IsHuman)
                    {
                        if (!JumpScareCooldown.Contains(player))
                        {
                            if (Physics.Raycast(player.ReferenceHub.PlayerCameraReference.position + player.ReferenceHub.PlayerCameraReference.forward * 0.2f, player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 25) &&
                                hit.collider.TryGetComponent<IDestructible>(out IDestructible destructible))
                            {
                                if (Player.TryGet(hit.collider.GetComponentInParent<ReferenceHub>(), out Player t) && player != t && t.IsScp)
                                {
                                    JumpScareCooldown.Add(player);

                                    Timing.CallDelayed(60, () =>
                                    {
                                        JumpScareCooldown.Remove(player);
                                    });

                                    AudioPlayer audioPlayer = AudioPlayer.CreateOrGet($"Player {player.Nickname}", condition: (hub) =>
                                    {
                                        return hub == player.ReferenceHub;
                                    }, onIntialCreation: (p) =>
                                    {
                                        Speaker speaker = p.AddSpeaker("Main", isSpatial: false, maxDistance: 12050);
                                    });

                                    audioPlayer.AddClip($"facingScp-{UnityEngine.Random.Range(1, 7)}", volume: 2);

                                    Timing.CallDelayed(3, () =>
                                    {
                                        audioPlayer.AddClip("chase", volume: 2);
                                    });
                                }
                            }
                        }
                    }
                }

                yield return Timing.WaitForOneFrame;
            }
        }
    }
}
