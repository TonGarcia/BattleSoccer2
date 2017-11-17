using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerAttribExtensions
{
    public static float GetCurrentStamin(this PlayerController player)
    {
        PlayerAttributes attrib = player.GetComponent<PlayerAttributes>();
        return attrib.Stamina.CurrentValue;
    }
}
public class PlayerAttributes : MonoBehaviour
{
    [System.Serializable]
    public class Attribute
    {
        [SerializeField]
        private float maxValue = 100;
        public float MaxValue { get { return maxValue; } }

        [SerializeField]
        private float currentValue = 0;
        public float CurrentValue { get { return currentValue; } }

        [SerializeField]
        private float dampTime = 1.5f;

        private float elapsedValue = 0;
        public float ElapsedValue { get { return elapsedValue; } }


        public void AddCurrentStamina(float v)
        {
            if (currentValue + v > maxValue)
            {
                currentValue = maxValue;
            }
            else
            {
                currentValue += v;
            }

        }
        public void SubCurrentStamina(float v)
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

        public void Update()
        {
            elapsedValue = Mathf.Lerp(elapsedValue, currentValue, dampTime * Time.deltaTime);
        }
    }

    public Attribute Stamina;


    public void Update()
    {
        Stamina.Update();
    }

}
