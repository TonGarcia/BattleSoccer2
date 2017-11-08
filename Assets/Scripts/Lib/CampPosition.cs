using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoccerGame
{
    public static class CampPositionExtensions
    {
        public static string ToKey(this CampPosition scgp)
        {
            string key = scgp.side.ToString() + scgp.position.ToString() + scgp.camptype.ToString();
            return key;
        }
    }
    /// <summary>
    /// Representa o lado do campo específico. Direito ou Esquerdo. 
    /// Muito usado para representar o time
    /// </summary>
    public enum CampPlaceSide
    {
        left,
        right
    }
    /// <summary>
    /// Posição em campo utilizado para os jogadores conhecerem quais suas posições de
    /// origem em uma partida. As posições possuem marcações para defesa e ataque, veja também
    /// <seealso cref="CampPlaceType"/>
    /// </summary>
    public enum CampPlacePosition
    {
        position_0,
        position_1,
        position_2,
        position_3,
        position_4,
        position_5,
        position_6,
        position_7,
        position_8,
        position_9,
        position_10


    }
    /// <summary>
    /// Campo ou grupo de posições <see cref="CampPlacePosition"/>, utilizado para
    /// saber se a posição origem é uma marcação de ataque ou de defesa
    /// </summary>
    public enum CampPlaceType
    {
        defense,
        attack
    }

    public class CampPosition : MonoBehaviour
    {

        public CampPlaceSide side;
        public CampPlacePosition position;
        public CampPlaceType camptype;

        public static string GetKey(CampPlaceSide side, CampPosition cpos, CampPlaceType ctype)
        {
            return (side.ToString() + cpos.ToString() + ctype.ToString());
        }
        public CampPositionsManager mananger;


        private void Start()
        {
            mananger.AddPosition(this);
        }

    }
}
