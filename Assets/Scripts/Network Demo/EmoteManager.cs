using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public static class EmoteManager
{
	private static Dictionary<string, Sprite> spriteLookup;

	private static void ReadSpritesIfNeeded()
	{
		if (spriteLookup != null)
		{
			return;
		}

		spriteLookup = new();

		// Specify the folder path (without "Resources/" and file extension)
		string folderPath = "EmoteButtons";

		// Load all sprites from the specified folder
		Sprite[] sprites = Resources.LoadAll<Sprite>(folderPath);

		foreach (Sprite sprite in sprites)
		{
			spriteLookup[sprite.name] = sprite; 
		}
	}

	public static Sprite GetEmote(string id)
	{
		ReadSpritesIfNeeded();

		if (!spriteLookup.ContainsKey(id))
		{
			WeekendLogger.LogError($"Emote '{id}' not found in lookup");
			return null;
		}

		return spriteLookup[id];
	}

	public static Sprite[] GetEmoteSprites()
	{
		ReadSpritesIfNeeded();

		return spriteLookup.Values.ToArray();
	}
}
