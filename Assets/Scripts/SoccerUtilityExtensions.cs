using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoccerGame
{
    static class SoccerUtilityExtensions
    {

        public static float Distance(this Transform transform, Transform other)
        {
            return Vector3.Distance(transform.position, other.position);
        }
        public static float Distance(this PlayerController owner, PlayerController other)
        {
            return Vector3.Distance(owner.transform.position, other.transform.position);
        }
    }
}
