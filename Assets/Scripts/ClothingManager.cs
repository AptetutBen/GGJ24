using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheetManager;
using System.Linq;

public class ClothingManager : MonoBehaviour
{
    public static ClothingManager instance;

    public SpreadsheetDatabase clothingDatabase;

    public List<Clothing> clothings = new List<Clothing>();

    private void Awake()
    {
        instance = this;

        // Build database from spreadsheet data
        foreach (SpreadsheetDataSet clothingItem in clothingDatabase.DataSets)
        {
            clothings.Add(new Clothing(clothingItem));
        }
    }

    public Clothing GetItemByID(string id)
    {
        return clothings.FirstOrDefault(item => item.id == id);
    }

    public Clothing GetRandomItem()
    {
        return clothings[Random.Range(0, clothings.Count)];
    }

    public Clothing GetRandomHat()
    {
        var sortedList =  clothings.Where(item => item.type == Clothing.ClothingType.Hat).ToList();

        return sortedList[Random.Range(0, sortedList.Count)];
    }

    public Clothing GetRandomTop()
    {
        var sortedList = clothings.Where(item => item.type == Clothing.ClothingType.Top).ToList();

        return sortedList[Random.Range(0, sortedList.Count)];
    }

    public Clothing GetRandomPants()
    {
        var sortedList = clothings.Where(item => item.type == Clothing.ClothingType.Pants).ToList();

        return sortedList[Random.Range(0, sortedList.Count)];
    }
}


public class Clothing
{
    public enum ClothingType { Hat, Pants, Top}

    public string id;
    public ClothingType type;
    public string clothingName;
    public string description;
    public string spriteName;

    public Clothing(SpreadsheetDataSet data)
    {
        id = data.GetValueAsString("ID");
        string clothingTypeString = data.GetValueAsString("Category");
        switch (clothingTypeString)
        {
            case "hat":
                type = ClothingType.Hat;
                break;
            case "pants":
                type = ClothingType.Pants;
                break;
            case "top":
                type = ClothingType.Top;
                break;
            default:
                break;
        }

        clothingName = data.GetValueAsString("Name");
        description = data.GetValueAsString("Description");
        spriteName = data.GetValueAsString("Sprite Name");
    }
}