using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothesPanel : MonoBehaviour
{
    public ClothingSubPanel subPanelHat, subPanelShirt;

    public void UpdateClothes(Clothing clothing)
    {
        if (clothing.type == Clothing.ClothingType.Hat)
        {
            subPanelHat.UpdatePanel(clothing);
        }
        else
        {
            subPanelShirt.UpdatePanel(clothing);
        }
       
    }
}
