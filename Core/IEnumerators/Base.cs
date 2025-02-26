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

namespace Backroom.Core.IEnumerators
{
    public static class Base
    {
        public static IEnumerator<float> BGM()
        {
            while (true)
            {
                AudioClipPlayback clip = GlobalPlayer.AddClip(Audios["BGMs"].GetRandomValue(), 0.2f, false);

                yield return Timing.WaitForSeconds((int)clip.Duration.TotalSeconds + Random.Range(1, 21));
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
    }
}
