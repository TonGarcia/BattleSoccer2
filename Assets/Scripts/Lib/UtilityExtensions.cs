using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace SoccerGame
{
    static class UtilityExtensions
    {

        public static float Distance(this Transform transform, Transform other)
        {
            Vector3 origem = transform.position;
            Vector3 destino = other.position;
            destino.y = 0;
            return Vector3.Distance(origem, destino);
        }
        public static float Distance(this PlayerController owner, PlayerController other)
        {
            Vector3 origem = owner.transform.position;
            Vector3 destino = other.transform.position;
            destino.y = origem.y;

            return Vector3.Distance(origem, destino);
        }
        public static float Distance(this PlayerController owner, Vector3 other)
        {
            Vector3 origem = owner.transform.position;
            Vector3 destino = other;
            destino.y = origem.y;

            return Vector3.Distance(origem, destino);
        }


        public static float Angle(this Vector3 from, Vector3 to)
        {
            return Vector3.Angle(from, to);
        }
        public static PlayerController MinAngle(this List<PlayerController> list, PlayerController player, Vector3 dirOrigim)
        {
            float min = list.Min(r => r.Direction(player).angle(dirOrigim));
            return list.FirstOrDefault(r => r.Direction(player).angle(dirOrigim) == min);
        }

        public static Vector3 Direction(this PlayerController from, PlayerController to)
        {
            return from.transform.position - to.transform.position;
        }


    }
}
