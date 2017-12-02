using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillAbility : MonoBehaviour {

    public Image img_icon;
    public Image img_cooldown;
    public Image img_value;

    private Sprite defaultSprite;
    private Color colorValue;
    private Color colorCooldown;

    private void Awake()
    {
        defaultSprite = img_icon.sprite;
        colorValue = img_value.color;
        colorCooldown = img_cooldown.color;
    }
  

    public void SetIcon(Sprite sprite)
    {
        if (img_icon)
            img_icon.sprite = sprite;
    }
    public void ResetIcon()
    {
        if (img_icon)
            img_icon.sprite = defaultSprite;
    }
    public void SetCooldown(float v)
    {
        if (img_cooldown)
            img_cooldown.fillAmount = v;
    }
    public void SetValue(float v)
    {
        if (img_value)
            img_value.fillAmount = v;
    }

    public void SetCooldownColor(Color color)
    {
        if (img_cooldown != null)
            img_cooldown.color = color;
    }
    public void ResetCooldownColor()
    {
        if (img_cooldown != null)
            img_cooldown.color = colorCooldown;
    }
    public void ResetValueColor()
    {
        if (img_value != null)
            img_value.color = colorValue;
    }
    public void SetValueColor(Color color)
    {
        if (img_value != null)
            img_value.color = color;
    }

    public void EnableValue()
    {
        if (img_value)
            img_value.enabled = true;
    }
    public void DisableValue()
    {
        if (img_value)
            img_value.enabled = false;
    }
    public void EnableCooldown()
    {
        if (img_cooldown)
            img_cooldown.enabled = true;
    }
    public void DisableCooldown()
    {
        if (img_cooldown)
            img_cooldown.enabled = false;
    }
}
