using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoccerGame
{

    public static class CampPositionExtensions
    {
        public static string ToKey(this CampPosition scgp)
        {
            string key = scgp.Side.ToString() + scgp.Marcation.ToString() + scgp.CampType.ToString();
            return key;
        }
        public static CampActionAttribute Next(this CampActionAttribute campAction)
        {
            CampActionAttribute result = CampActionAttribute.deffender;

            if (campAction == CampActionAttribute.deffender)
                result = CampActionAttribute.middle;
            else if (campAction == CampActionAttribute.middle)
                result = CampActionAttribute.attack;
            else
                result = CampActionAttribute.attack;

            return result;
        }
        public static CampActionAttribute Prev(this CampActionAttribute campAction)
        {
            CampActionAttribute result = CampActionAttribute.deffender;

            if (campAction == CampActionAttribute.attack)
                result = CampActionAttribute.middle;
            else if (campAction == CampActionAttribute.middle)
                result = CampActionAttribute.deffender;
            else
                result = CampActionAttribute.deffender;

            return result;
        }

        public static CampTeam Enemy(this CampTeam team)
        {
            if (team == CampTeam.Team_A)
                return CampTeam.Team_B;
            else
                return CampTeam.Team_A;

        }

    }
    /// <summary>
    /// Atributo de ação que o jogador tem no campo.
    /// </summary>
    public enum CampActionAttribute
    {
        deffender,
        middle,
        attack
    }
    
    public enum CampTeam
    {
        Team_A,
        Team_B
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
    public enum CampPlaceMarcation
    {
        marcation_0,
        marcation_1,
        marcation_2,
        marcation_3,
        marcation_4,
        marcation_5,
        marcation_6,
        marcation_7,
        marcation_8,
        marcation_9,
        marcation_10


    }
    /// <summary>
    /// Campo ou grupo de posições <see cref="CampPlaceMarcation"/>, utilizado para
    /// saber se a posição origem é uma marcação de ataque ou de defesa
    /// </summary>
    public enum CampPlaceType
    {
        defense,
        attack
    }

    public class CampPosition : MonoBehaviour
    {
        [SerializeField]
        private CampPlaceSide side;
        public CampPlaceSide Side { get { return side; } }
        [SerializeField]
        private CampPlaceMarcation marcation;
        public CampPlaceMarcation Marcation { get { return marcation; } }
        [SerializeField]
        private CampPlaceType camptype;
        public CampPlaceType CampType { get { return camptype; } }

        public static string GetKey(CampPlaceSide side, CampPlaceMarcation cpos, CampPlaceType ctype)
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
