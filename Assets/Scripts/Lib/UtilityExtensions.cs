using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoccerGame
{
    static class UtilityExtensions
    {

        public static float Distance(this Transform transform, Transform other)
        {
            Vector3 origem = transform.position;
            Vector3 destino = other.position;
            destino.y = origem.y;
            return Vector3.Distance(origem, destino);
        }
        public static float Distance(this PlayerController owner, PlayerController other)
        {
            Vector3 origem = owner.transform.position;
            Vector3 destino = other.transform.position;
            destino.y = origem.y;

            return Vector3.Distance(origem, destino);
        }
    }
}
