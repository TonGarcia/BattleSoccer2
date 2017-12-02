using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;

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
        public static LocomotionAbility ToLocomotion(this Ability ability)
        {
            return (LocomotionAbility)ability;
        }
    }

    public enum SkillVarMode
    {
        nothing,
        autoRegen,
        autoSubtract,
    }

    [System.Serializable]
    public class SkillVar: ICloneable
    {
        [HideInInspector]
        public Ability ability;

        [SerializeField]
        private float maxValue = 100;
        public float MaxValue { get { return maxValue; } }

        [SerializeField]
        private float currentValue = 0;
        public float CurrentValue { get { return currentValue; } }

        [Range(0.0f, 1.0f)]
        public float CriticalValue = 0;
        /// <summary>
        /// Tempo total em segundos que leva para o cooldown ser finalizado quando acionado via <see cref="TriggerCooldown()"/>.
        /// Utilize <seealso cref="FillAmountCooldown"/> para capturar a percentagem de 0 a 1 do cooldown.
        /// </summary>
        public float coolDownDuration = 1.0f;
        /// <summary>
        /// Tempo total em segundos necessário para regeneração automatica até se igual ao maxvalue. Utilize
        /// <seealso cref="mode"/> para setar regeneração automática
        /// </summary>
        public float regenDuration = 10.0f;
        /// <summary>
        /// Tempo total em segundos necessário para subtração total do valor até se igual a 0.
        /// Utilize <seealso cref="mode"/> para setar a subtração automática.
        /// </summary>
        public float subtractDuration = 5.0f;

        public float dampTime = 3.5f;

        public SkillVarMode mode = SkillVarMode.nothing;

        public Image imageValueFillAmount;
        public Image imageCooldownFillAmout;

        public float ElapsedValue { get { return elapsedValue; } }
        public float FillAmounValue { get { return elapsedValue / MaxValue; } }
        public float FillAmountCooldown { get { return coolDownTimeLeft / coolDownDuration; } }
        /// <summary>
        /// Verdadeiro se não estiver em Cooldown
        /// </summary>
        public bool IsReady { get { return isReady; } }
        public bool IsMax { get { return currentValue == maxValue; } }
        public bool IsMin { get { return currentValue <= 0; } }
        /// <summary>
        /// Informa se o valor atual esta abaixo do critico permitido.
        /// Se o valor atual da skill for menor que o setado pelo <see cref="CriticalValue"/>, este metodo será verdadeiro.
        /// </summary>
        public bool IsCritical { get { return currentValue < (maxValue * CriticalValue); } }

        private float elapsedValue = 0;
        private float coolDownTimeLeft = 0;
        private float nextReadyTime = 0;
        private bool isReady = true;

        public void SetCurrentValue(float v)
        {
            if (v >= 0 && v <= maxValue)
                currentValue = v;
        }
        public void SetMaxValue(float v)
        {
            if (v < 0)
                return;

            maxValue = v;
            if (CurrentValue > maxValue)
                currentValue = maxValue;

        }
        public void AddCurrentValue(float v)
        {
            if (v <= 0)
                return;

            if (currentValue + v > maxValue)
            {
                currentValue = maxValue;
            }
            else
            {
                currentValue += v;
            }

        }
        public void SubCurrentValue(float v)
        {
            if (currentValue - v < 0)
            {
                currentValue = 0;
            }
            else
            {
                currentValue -= v;
            }
        }
        public void SetCoolDownImagemask(Image image)
        {
            imageCooldownFillAmout = image;
        }


        public void Update()
        {
            if (mode == SkillVarMode.autoRegen)
            {
                float valued = currentValue;

                valued += (maxValue / regenDuration) * Time.deltaTime;
                if (valued > maxValue)
                    valued = maxValue;

                currentValue = valued;

            }
            else if (mode == SkillVarMode.autoSubtract)
            {
                float valued = currentValue;

                valued -= (maxValue / subtractDuration) * Time.deltaTime;
                if (valued < 0)
                    valued = 0;

                currentValue = valued;
            }

            elapsedValue = Mathf.Lerp(elapsedValue, currentValue, dampTime * Time.deltaTime);

            if (imageValueFillAmount != null)
            {
                imageValueFillAmount.fillAmount = FillAmounValue;
            }

            bool isCoolDownComplete = (Time.time > nextReadyTime);
            if (isCoolDownComplete)
            {
                SkillReady();
            }
            else
            {
                CoolDown();
            }
        }
        public void TriggerCooldown()
        {
            if (isReady == false)
                return;

            nextReadyTime = coolDownDuration + Time.time;
            coolDownTimeLeft = coolDownDuration;
            if (imageCooldownFillAmout != null)
                imageCooldownFillAmout.enabled = true;
        }
        private void SkillReady()
        {
            isReady = true;
            if (imageCooldownFillAmout != null)
                imageCooldownFillAmout.enabled = false;
        }
        private void CoolDown()
        {
            isReady = false;
            coolDownTimeLeft -= Time.deltaTime;
            if (imageCooldownFillAmout != null)
            {
                imageCooldownFillAmout.enabled = true;
                imageCooldownFillAmout.fillAmount = FillAmountCooldown;
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
