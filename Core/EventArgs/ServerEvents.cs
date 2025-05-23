﻿using Exiled.API.Features;
using InventorySystem.Configs;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Backroom.Core.Variables.Base;
using static Backroom.Core.Functions.Base;
using static Backroom.Core.IEnumerators.Base;
using static Backroom.Core.Extensions.Base;
using MultiBroadcast.API;
using Exiled.API.Extensions;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Serializable;
using Exiled.API.Enums;
using Interactables.Interobjects.DoorUtils;
using MapEditorReborn.API.Enums;
using PlayerRoles;
using AiScp;

namespace Backroom.Core.EventArgs
{
    public static class ServerEvents
    {
        public static IEnumerator<float> OnWaitingForPlayers()
        {
            yield return Timing.WaitForSeconds(1);

            Map.IsDecontaminationEnabled = false;
            Respawn.PauseWaves();
            Round.IsLocked = true;
            Round.Start();
            Server.FriendlyFire = true;
            Server.ExecuteCommand($"/mp load Backroom");

            foreach (var _audioClip in System.IO.Directory.GetFiles(Paths.Plugins + "/audio/"))
            {
                string name = _audioClip.Replace(Paths.Plugins + "/audio/", "").Replace(".ogg", "");

                Audios["Audios"].Add(name);

                AudioClipStorage.LoadClip(_audioClip, name);
            }

            foreach (var _audioClip in System.IO.Directory.GetFiles(Paths.Configs + "/Backroom/BGMs/"))
            {
                string name = _audioClip.Replace(Paths.Configs + "/Backroom/BGMs/", "").Replace(".ogg", "");

                Audios["BGMs"].Add(name);

                AudioClipStorage.LoadClip(_audioClip, name);
            }

            foreach (var _audioClip in System.IO.Directory.GetFiles(Paths.Configs + "/Backroom/SEs/"))
            {
                string name = _audioClip.Replace(Paths.Configs + "/Backroom/SEs/", "").Replace(".ogg", "");

                Audios["SEs"].Add(name);

                AudioClipStorage.LoadClip(_audioClip, name);
            }

            GlobalPlayer = AudioPlayer.CreateOrGet($"Global AudioPlayer", onIntialCreation: (p) =>
            {
                Speaker speaker = p.AddSpeaker("Main", isSpatial: false, maxDistance: 5000);
            });

            FirstSpawnPoint = GameObject.FindObjectsOfType<Transform>().Where(t => t.name == "[SP] First").FirstOrDefault();
            BackroomSchematic = (SchematicObject)MapEditorReborn.API.API.SpawnedObjects.Where(x => x is SchematicObject).FirstOrDefault();

            InventoryLimits.StandardCategoryLimits[ItemCategory.SpecialWeapon] = 8;
            InventoryLimits.StandardCategoryLimits[ItemCategory.SCPItem] = 8;
            InventoryLimits.Config.RefreshCategoryLimits();

            Timing.RunCoroutine(BGM());
            Timing.RunCoroutine(ClearDecals());
            Timing.RunCoroutine(InputCooldown());
            Timing.RunCoroutine(IsFallDown());
            Timing.RunCoroutine(HumanLoop());
            Timing.RunCoroutine(ItemSpawner());

            foreach (var pos in GenerateUniformPoints(-1000, -1000, 1000, 1000, 1000, 100))
            {
                try
                {
                    DoorSerializable door = new DoorSerializable();
                    door.DoorType = DoorType.EntranceDoor;

                    ObjectSpawner.SpawnDoor(door, pos, null, new Vector3(15, 15, 15));
                }
                catch (Exception e)
                {
                    Log.Error($"Error while spawning door: {e}");
                }
            }

            //AiPlayerUtils.CreateAi(RoleTypeId.Scp106, FirstSpawnPoint.position, true);
            //AiPlayerUtils.CreateAi(RoleTypeId.Scp049, new Vector3(50.39742f, 1042.284f, -49.08986f), true);
        }
    }
}
