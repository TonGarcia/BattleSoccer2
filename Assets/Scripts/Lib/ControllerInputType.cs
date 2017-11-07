using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SoccerGame
{
    /// <summary>
    /// Tipos de controladores aceitos. 
    /// Utilizados para identificar quem esta controlando o objeto.
    /// As abreviações seguidas nas configurações de Input do editor serão as seguintes:
    /// P1_ para Controller1, P2_ para Controller2 e CPU será ignorado
    /// </summary>
    public enum ControllerInputType
    {
        Controller1,
        Controller2,
        ControllerCPU
    }

    public enum ButtomInputType
    {
        X, B, A, Y, RB, RT, LB, LT, Stick1, Stick2, Up, Right, Down, Left, Back, Start

    }
}