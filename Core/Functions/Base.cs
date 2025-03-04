using AdminToys;
using Backroom.Core.Extensions;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Toys;
using Hints;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features.Serializable;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utf8Json.Internal.DoubleConversion;
using Utf8Json.Resolvers.Internal;
using static Backroom.Core.Variables.Base;

namespace Backroom.Core.Functions
{
    public static class Base
    {
        public static List<T> EnumToList<T>()
        {
            Array items = Enum.GetValues(typeof(T));
            List<T> itemList = new List<T>();

            foreach (T item in items)
            {
                if (!item.ToString().Contains("None"))
                    itemList.Add(item);
            }

            return itemList;
        }

        public static bool TryGetLookPlayer(Player player, float Distance, out Player target, out RaycastHit? raycastHit)
        {
            target = null;
            raycastHit = null;

            if (Physics.Raycast(player.ReferenceHub.PlayerCameraReference.position + player.ReferenceHub.PlayerCameraReference.forward * 0.2f, player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, Distance) &&
                    hit.collider.TryGetComponent<IDestructible>(out IDestructible destructible))
            {
                if (Player.TryGet(hit.collider.GetComponentInParent<ReferenceHub>(), out Player t) && player != t)
                {
                    target = t;
                    raycastHit = hit;

                    return true;
                }
            }

            return false;
        }

        public static string ColorFormat(string cn)
        {
            if (ColorUtility.TryParseHtmlString(cn, out Color color))
                return color.ToHex();

            else
            {
                Dictionary<string, string> Colors = new Dictionary<string, string>
                {
                    // {"gold", "#EFC01A"},
                    // {"teal", "#008080"},
                    // {"blue", "#005EBC"},
                    // {"purple", "#8137CE"},
                    // {"light_red", "#FD8272"},
                    {"pink", "#FF96DE"},
                    {"red", "#C50000"},
                    {"default", "#FFFFFF"},
                    {"brown", "#944710"},
                    {"silver", "#A0A0A0"},
                    {"light_green", "#32CD32"},
                    {"crimson", "#DC143C"},
                    {"cyan", "#00B7EB"},
                    {"aqua", "#00FFFF"},
                    {"deep_pink", "#FF1493"},
                    {"tomato", "#FF6448"},
                    {"yellow", "#FAFF86"},
                    {"magenta", "#FF0090"},
                    {"blue_green", "#4DFFB8"},
                    // {"silver_blue", "#666699"},
                    {"orange", "#FF9966"},
                    // {"police_blue", "#002DB3"},
                    {"lime", "#BFFF00"},
                    {"green", "#228B22"},
                    {"emerald", "#50C878"},
                    {"carmine", "#960018"},
                    {"nickel", "#727472"},
                    {"mint", "#98FB98"},
                    {"army_green", "#4B5320"},
                    {"pumpkin", "#EE7600"}
                };

                if (Colors.ContainsKey(cn))
                    return Colors[cn];

                else
                    return "#FFFFFF";
            }
        }

        public static string BadgeFormat(Player player)
        {
            if (player.RankName != null && !player.BadgeHidden)
                return $"[<color={ColorFormat(player.RankColor)}>{player.RankName}</color>] ";

            else
                return "";
        }

        public static List<Vector3> GenerateUniformPoints(float startX, float startZ, float endX, float endZ, float y, int totalPoints)
        {
            List<Vector3> points = new List<Vector3>();

            int sqrtPoints = (int)Math.Ceiling(Math.Sqrt(totalPoints));
            float xInterval = (endX - startX) / (sqrtPoints - 1);
            float zInterval = (endZ - startZ) / (sqrtPoints - 1);

            for (int i = 0; i < sqrtPoints; i++)
            {
                for (int j = 0; j < sqrtPoints; j++)
                {
                    float x = startX + i * xInterval;
                    float z = startZ + j * zInterval;
                    points.Add(new Vector3(x, y, z));
                }
            }

            return points;
        }

        public static Vector3 GetRandomLocation()
        {
            switch (UnityEngine.Random.Range(1, 3)) 
            {
                case 1:
                    return new Vector3(UnityEngine.Random.Range(-45.46655f, 107.9961f), 1046.391f, UnityEngine.Random.Range(-139.3259f, 38.08987f));

                default:
                    return new Vector3(UnityEngine.Random.Range(-33.98999f, 64.64844f), 1042.289f, UnityEngine.Random.Range(-98.98218f, -1.117188f));
            }
        }
    }
}
