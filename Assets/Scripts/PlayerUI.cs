using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerAttributes))]
public class PlayerUI : MonoBehaviour
{
    public PlayerAttributes playerAttributes;

    public Canvas canvas_ui;
    public Text txt_Name;
    public Image img_Stamina;

   
    public void Update()
    {
        if (!canvas_ui.enabled)
            return;

        img_Stamina.fillAmount = playerAttributes.Stamina.ElapsedValue / playerAttributes.Stamina.MaxValue;
    }

    public void DisableCanvas()
    {
        canvas_ui.gameObject.SetActive(false);
    }
    public void EnableCanvas()
    {
        canvas_ui.gameObject.SetActive(true);
    }
}
