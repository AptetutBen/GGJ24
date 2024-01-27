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
    public List<Sprite> clothingPickupSprites = new List<Sprite>();
    private Dictionary<string, Sprite> clothingPickupLookup = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> clothingLookup = new Dictionary<string, Sprite>();

    private void Awake()
    {
        instance = this;

        // Build database from spreadsheet data
        foreach (SpreadsheetDataSet clothingItem in clothingDatabase.DataSets)
        {
            if (clothingItem.GetValueAsBool("Active")){
                clothings.Add(new Clothing(clothingItem));
            }
          
        }

        foreach (Sprite item in clothingPickupSprites)
        {
            clothingPickupLookup[item.name] = item;
        }
    }

    public Sprite GetPickupSpriteFromId(string id, Clothing.ClothingType clothingType)
    {
        string lookupId = $"item_{(clothingType == Clothing.ClothingType.Hat ? "hat" : "shirt")}_{id}";

        if (!clothingPickupLookup.ContainsKey(lookupId))
        {
            WeekendLogger.LogError($"item {lookupId} not found");
            return null;
        }

        return clothingPickupLookup[lookupId];
    }

    public Sprite GetHatSpriteFromId(string id)
    {
        return clothingLookup["hat_" + id];
    }

    public Sprite[] GetTopPiecesFromId(string id)
    {
        return new Sprite[] { clothingLookup["shirt_" + id], clothingLookup["sleeve_" + id], clothingLookup["sleeve1_" + id] };
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
}


public class Clothing
{
    public enum ClothingType { Hat, Top}

    public string id;
    public bool active;
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