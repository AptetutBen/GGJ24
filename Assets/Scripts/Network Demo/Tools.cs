
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tools
{
	public static Color RandomColour()
	{
		// Create a random number generator
		System.Random random = new System.Random();

		// Choose a channel to be full (255) and another to be empty (0)
		int fullChannel = random.Next(3); // 0 for Red, 1 for Green, 2 for Blue
		int emptyChannel = (fullChannel + random.Next(1, 3)) % 3; // Ensure different channels

		// Initialize the RGB components
		float[] rgb = { 0, 0, 0 };

		// Set the full channel to 255 and the empty channel to 0
		rgb[fullChannel] = 1;
		rgb[emptyChannel] = 0;

		// Set the remaining channel to a random value
		int remainingChannel = 3 - fullChannel - emptyChannel;
		rgb[remainingChannel] = UnityEngine.Random.Range(0, 1f);

		// Create a Color with full saturation
		return new Color(rgb[0], rgb[1], rgb[2]);
	}

	// Function to convert Color to hex string
	public static string ColorToHex(Color color)
	{
		// Convert each color component to its hexadecimal representation
		int r = Mathf.RoundToInt(color.r * 255f);
		int g = Mathf.RoundToInt(color.g * 255f);
		int b = Mathf.RoundToInt(color.b * 255f);

		// Combine components into a hex string
		string hexString = string.Format("#{0:X2}{1:X2}{2:X2}", r, g, b);

		return hexString;
	}

	public static string GenerateRandomName()
	{
		string[] descriptors = {
			"Brave", "Clever", "Energetic", "Gentle", "Happy",
			"Inquisitive", "Jolly", "Kind", "Lucky", "Mysterious",
			"Playful", "Quirky", "Radiant", "Sunny", "Thoughtful",
			"Unique", "Vibrant", "Whimsical", "Xenial", "Youthful",
			"Zesty", "Adventurous", "Ambitious", "Caring", "Daring",
			"Empathetic", "Fearless", "Generous", "Harmonious", "Imaginative",
			"Joyful", "Lively", "Magical", "Nurturing", "Optimistic",
			"Passionate", "Resourceful", "Serene", "Tranquil", "Uplifting",
			"Versatile", "Witty", "Xclusive", "Yearning", "Zealous",
			"Amicable", "Bountiful", "Charismatic", "Dazzling", "Effervescent",
			"Fanciful", "Glorious", "Hopeful", "Inspiring", "Jubilant",
			"Kindred", "Luminous", "Majestic", "Noble", "Outgoing",
			"Peaceful", "Quaint", "Resilient", "Spirited", "Tenacious",
			"Unwavering", "Vivacious", "Winsome", "Xenodochial", "Youthful",
			"Zany", "Alluring", "Blissful", "Captivating", "Delightful",
			"Enchanting", "Fascinating", "Graceful", "Harmonious", "Invigorating",
			"Jovial", "Kaleidoscopic", "Luscious", "Mesmerizing", "Nostalgic",
			"Opulent", "Pristine", "Quixotic", "Ravishing", "Serenading",
			"Tantalizing", "Umbra", "Velvet", "Whispering", "Xenon",
			"Yearning", "Zenith", "Amber", "Breeze", "Celestial"
		};

		string[] objects = {
			"Hawk", "Mountain", "River", "Star", "Tree",
			"Wave", "Meadow", "Rainbow", "Sword", "Phoenix",
			"Dragon", "Castle", "Moon", "Sun", "Ocean",
			"Valley", "Echo", "Dream", "Harmony", "Comet",
			"Brook", "Dawn", "Garden", "Journey", "Lighthouse",
			"Cascade", "Enigma", "Quest", "Whisper", "Cascade",
			"Serenade", "Cascade", "Aurora", "Horizon", "Eclipse",
			"Sculpture", "Horizon", "Sculpture", "Infinity", "Serenity",
			"Nebula", "Enchantment", "Symphony", "Rhapsody", "Crescent",
			"Galaxy", "Oasis", "Gleam", "Quasar", "Celestial",
			"Cascade", "Ethereal", "Lullaby", "Twilight", "Labyrinth",
			"Blossom", "Reflection", "Oracle", "Sonnet", "Mystique",
			"Radiance", "Melody", "Tranquility", "Cascade", "Cynosure",
			"Aegis", "Reverie", "Vortex", "Halcyon", "Jubilee",
			"Panorama", "Zephyr", "Eden", "Talisman", "Cascade",
			"Enigma", "Cascade", "Ascendancy", "Torch", "Ecliptic",
			"Gossamer", "Horizon", "Cascade", "Fountain", "Cascade",
			"Ephemeral", "Reverie", "Luminosity", "Voyage", "Cascade",
			"Elysium", "Wanderlust", "Inception", "Crest", "Cascade"
		};

		string randomDescriptor = descriptors[Random.Range(0,descriptors.Length)];
		string randomObject = objects[Random.Range(0, objects.Length)];

		return randomDescriptor + " " + randomObject;
	}

}
