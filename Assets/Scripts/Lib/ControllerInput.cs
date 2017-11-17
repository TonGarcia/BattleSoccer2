using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoccerGame
{
    /// <summary>
    /// Interface de captura de entrada de dados para controladores. Esta infertace utiliza a 
    /// <seealso cref="Input"/> Genérica da unity, trabalhando como uma expansão, facilitando 
    /// a utilização das entradas para multiplos usuários, veja também <seealso cref="ControllerInputType"/>
    /// para entender como utilizar as entradas para multiplos controladores.
    /// <para>
    /// Tenha em mente que as configurações de Input precisam ser modificadas no editor da unity da seguinte maneira:
    /// Use P1_ e P2_ antes do nome de cada input para diferir a qual controlador ele pertence. Esta interface levara esta 
    /// abreviatura em considerãção para saber se a entrada de dados veio do controlador para jogador 1 ou 2.
    /// </para>
    /// </summary>
    public static class ControllerInput
    {
        public static bool GetButton(ControllerInputType userType, string bt)
        {
            if (userType == ControllerInputType.ControllerCPU)
                return false;

            string str_input = userType.ToShortCutInput() + bt;
            return Input.GetButton(str_input);
        }
        public static bool GetButton(ControllerInputType userType, ButtomInputType bt)
        {
            if (userType == ControllerInputType.ControllerCPU)
                return false;

            string str_input = userType.ToShortCutInput() + bt.ToString();
            return Input.GetButton(str_input);
        }
        public static bool GetButtonDown(ControllerInputType userType, string bt)
        {
            if (userType == ControllerInputType.ControllerCPU)
                return false;
            string str_input = userType.ToShortCutInput() + bt;
            return Input.GetButtonDown(str_input);
        }
        public static bool GetButtonDown(ControllerInputType userType, ButtomInputType bt)
        {
            if (userType == ControllerInputType.ControllerCPU)
                return false;
            string str_input = userType.ToShortCutInput() + bt.ToString();
            return Input.GetButtonDown(str_input);
        }
        public static bool GetButtonUp(ControllerInputType userType, string bt)
        {
            if (userType == ControllerInputType.ControllerCPU)
                return false;
            string str_input = userType.ToShortCutInput() + bt;
            return Input.GetButtonUp(str_input);
        }
        public static bool GetButtonUp(ControllerInputType userType, ButtomInputType bt)
        {
            if (userType == ControllerInputType.ControllerCPU)
                return false;
            string str_input = userType.ToShortCutInput() + bt.ToString();
            return Input.GetButtonUp(str_input);
        }
        public static float GetAxisHorizontal(ControllerInputType userType)
        {
            if (userType == ControllerInputType.ControllerCPU)
                return 0;
            string str_input_h = userType.ToShortCutInput() + "Horizontal";
            return Input.GetAxis(str_input_h);

        }
        public static float GetAxisVertical(ControllerInputType userType)
        {
            if (userType == ControllerInputType.ControllerCPU)
                return 0;
            string str_input_v = userType.ToShortCutInput() + "Vertical";
            return Input.GetAxis(str_input_v);
        }
        public static Vector2 GetAxis(ControllerInputType userType)
        {
            if (userType == ControllerInputType.ControllerCPU)
                return Vector2.zero;
            string str_input_h = userType.ToShortCutInput() + "Horizontal";
            string str_input_v = userType.ToShortCutInput() + "Vertical";
            float h = Input.GetAxis(str_input_h);
            float v = Input.GetAxis(str_input_v);

            return new Vector2(h, v);
        }
        public static string ToShortCutInput(this ControllerInputType userType)
        {
            string result = "";

            if (userType == ControllerInputType.Controller1)
            {
                result = "P1_";
            }
            else if (userType == ControllerInputType.Controller2)
            {
                result = "P2_";
            }

            return result;
        }
    }
}