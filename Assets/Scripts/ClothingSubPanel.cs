using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClothingSubPanel : MonoBehaviour
{
    public Image clothingImage;
    public TextMeshProUGUI titleText,abilitiesText;
    public CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup.alpha = 0;
    }

    public void UpdatePanel(Clothing clothing)
    {
        if(clothing == null)
        {
            canvasGroup.alpha = 0;
            return;
        }

        canvasGroup.alpha = 1;
        titleText.text = clothing.clothingName;
        abilitiesText.text = string.Join(",", clothing.abilities).Replace(",", System.Environment.NewLine);
        clothingImage.sprite = ClothingManager.instance.GetPickupSpriteFromId(clothing.spriteName, clothing.type);
    }
}

