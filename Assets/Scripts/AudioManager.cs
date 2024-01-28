using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
	//Keys for saving and loading the volume

	public static readonly string MasterVolumeSaveKey = "master_volume";
	public static readonly string MusicVolumeSaveKey = "music_volume";
	public static readonly string SfxVolumeSaveKey = "sfx_volume";

	//Keys for accessing the exposed variables on the audio mixer

	public static readonly string MasterVolumeMixerKey = "master_volume";
	public static readonly string MusicVolumeMixerKey = "music_volume";
	public static readonly string SfxVolumeMixerKey = "sfx_volume";

	//References

	//A static reference to this game object that can be called from any other active script
	public static AudioManager instance;

	//Background Audio sources
	private List<AudioSource> musicAudioSources;

	//A counter showing the current music Audio Source
	private int currentMusicAudioSource;

	//SFX Audio sources
	private List<AudioSource> sfxAudioSources;

	//A counter showing the current sfx Audio Source
	private int currentSfxSource;

	//Master audio mixer
	public AudioMixer masterMixer;

	public List<AudioClip> sfxClips = new();
	public Dictionary<string, AudioClip> clipLookup = new();


	// Called when the object first gets initialise
	void Awake()
	{
		//Look for all objects of type "AudioManager", and if there is more than one in the scene then this one is not the original, destroy this one.
		if (FindObjectsOfType<AudioManager>().Length > 1)
		{
			Destroy(gameObject);
			return;
		}

		LoadAudio();

		//Load this version of the Audio Manager into the static reference
		instance = this;

		//Mark this game object as not to be destroyed when a new scene is loaded
		DontDestroyOnLoad(this);

		//Create SFX audio soruces
		sfxAudioSources = new();

		for (int i = 0; i < 5; i++)
		{
			sfxAudioSources.Add(CreateAudioSource("SFX Audio Source " + i,"SFX"));
		}


		//Create Music audio soruces
		musicAudioSources = new();

		for (int i = 0; i < 2; i++)
		{
			musicAudioSources.Add(CreateAudioSource("Music Audio Source " + i, "Music",true));
		}

		AudioSource CreateAudioSource(string name, string mixerGroupName, bool loop = false)
		{
			GameObject newAudioSourceGameObject = new GameObject(name);
			newAudioSourceGameObject.transform.SetParent(transform);
			AudioSource newAudioSource = newAudioSourceGameObject.AddComponent<AudioSource>();

			newAudioSource.loop = loop;
			newAudioSource.dopplerLevel = 0;
			newAudioSource.playOnAwake = false;
			newAudioSource.outputAudioMixerGroup = masterMixer.FindMatchingGroups(mixerGroupName)[0];

			return newAudioSource;
		}

	}

	private void LoadAudio()
    {
		// Specify the folder path relative to the Resources folder
		string folderPath = "SFX";

		// Load all assets in the specified folder with the ".mp3" extension
		Object[] mp3Files = Resources.LoadAll(folderPath, typeof(AudioClip));

		// Filter only the AudioClips from the loaded assets
		foreach (Object obj in mp3Files)
		{
			if (obj is AudioClip)
			{
				sfxClips.Add((AudioClip)obj);
			}
		}


		foreach (AudioClip clip in sfxClips)
		{
			clipLookup[clip.name] = clip;
		}

	}

	private void Start()
	{
		//Load any saved volume levels from player prefs

		//Set the mixer volume of the master volume
		SetMasterVolume(GetMasterVolume(), false);

		//Get the saved Music volume level (returns 1 if nothing has been saved)
		SetMusicVolume(GetMusicVolume(), false);

		//Get the saved SFX volume level (returns 1 if nothing has been saved)
		SetSfxVolume(GetSfxVolume(), false);
	}

	//Pause the Background Music
	public void PauseMusic()
	{
		musicAudioSources[currentMusicAudioSource].Pause();
	}

	//Play the Background Music
	public void PlayMusic()
	{
		sfxAudioSources[currentMusicAudioSource].Play();
	}


	//-----------------------------------------------------
	// Set Volume levels

	//Change the master volume level
	public void SetMasterVolume(float value, bool doSave = true)
	{
		//Change the volume on the mixer
		masterMixer.SetFloat(MasterVolumeMixerKey, LinearToDecibel(value));

		//Save the volume to the player prefs
		if (doSave)
		{
			PlayerPrefs.SetFloat(MasterVolumeSaveKey, value);
		}
	}

	public float GetMasterVolume()
	{
		return PlayerPrefs.GetFloat(MasterVolumeSaveKey, 1);
	}

	//Change the music volume level
	public void SetMusicVolume(float value, bool doSave = true)
	{
		//Change the volume on the mixer
		masterMixer.SetFloat(MusicVolumeMixerKey, LinearToDecibel(value));

		//Save the volume to the player prefs
		if (doSave)
		{
			PlayerPrefs.SetFloat(MusicVolumeSaveKey, value);
		}
	}

	public float GetMusicVolume()
	{
		return PlayerPrefs.GetFloat(MusicVolumeSaveKey,1);
	}

	//Change the sfx music volume level
	public void SetSfxVolume(float value, bool doSave = true)
	{
		//Change the volume on the mixer
		masterMixer.SetFloat(SfxVolumeMixerKey, LinearToDecibel(value));

		//Save the volume to the player prefs
		if (doSave)
		{
			PlayerPrefs.SetFloat(SfxVolumeSaveKey, value);
		}
	}

	public float GetSfxVolume()
	{
		return PlayerPrefs.GetFloat(SfxVolumeSaveKey,1);
	}


	private float LinearToDecibel(float linear)
	{
		float dB;

		if (linear != 0)
			dB = 20.0f * Mathf.Log10(linear);
		else
			dB = -144.0f;

		return dB;
	}

	//-----------------------------------------------------
	// Playing SFX

	public void PlaySFX(AudioClip clip,Vector3 position, float spatial = 1)
	{
		AudioSource selectedAudioSource = GetAvaliableAudioSource();

		selectedAudioSource.transform.position = position;

		PlaySFX(clip, spatial);
	}

	public void PlaySFX(string clipID)
    {
        if (clipID == "_top" || clipID == "_hat")
        {
			return;
        }
		PlaySFX(clipLookup[clipID]);
	}

	public void PlaySFX(string clipID, Vector3 position)
	{
		if (clipID == "_top" || clipID == "_hat")
		{
			return;
		}
		PlaySFX(clipLookup[clipID], position);
	}


	// Play a SFX clip with position
	public void PlaySFX(AudioClip clip, float spatial = 0)
	{
		AudioSource selectedAudioSource = GetAvaliableAudioSource();

		//Set the clip to play to the current sfx audio source
		selectedAudioSource.clip = clip;

		//Set the spacial setting
		selectedAudioSource.spatialBlend = spatial;

		//Play the clip
		selectedAudioSource.Play();
	}

	// Play a SFX clip 
	private AudioSource GetAvaliableAudioSource()
	{
		//Increment the current SFX counter
		currentSfxSource++;

		//If the counter gets longer than the number of audio sources, the reset to 0
		if (currentSfxSource >= sfxAudioSources.Count)
		{
			currentSfxSource = 0;
		}

		return sfxAudioSources[currentSfxSource];
	}


	//-----------------------------------------------------
	// Switching Music

	//Switch the current music playing
	public void SwitchMusicClip(AudioClip newAudioClip)
	{
		//Fade the current audio source out
		StartCoroutine(FadeSongOut(musicAudioSources[currentMusicAudioSource]));

		//Increment the current Music counter
		currentMusicAudioSource = (currentMusicAudioSource + 1)% musicAudioSources.Count;

		//Fade the new audio source in
		StartCoroutine(FadeSongIn(musicAudioSources[currentMusicAudioSource], newAudioClip));
	}

	//Coroutine to fade in a song
	private IEnumerator FadeSongIn(AudioSource audioSource, AudioClip song)
	{
		//Set the audiosource volume to 0
		audioSource.volume = 0;

		//Set the audio source's clip to the song we want to play
		audioSource.clip = song;

		//Start playing the song
		audioSource.Play();

		//Timer to track the transition from 0 to 1
		float timer = 0;

		//Loop until timer gets over 1
		while (timer < 1)
		{
			//Add the time that has passed since the last frame to the timer
			timer += Time.deltaTime;

			//Set the audioSource's volume to be whatever the timer currently is
			audioSource.volume = timer;

			//Wait till the next frame to loop around again.
			yield return null;
		}

		//When we get here the timer should have gone from 0 to 1 over a 1 second.

		//Set the audiosource volume to 1 to ensure its set correctly
		audioSource.volume = 1;
	}

	//Coroutine to fade out a song
	private IEnumerator FadeSongOut(AudioSource audioSource)
	{
		//Set the timer to whatever the audiosource currently is
		float timer = audioSource.volume;

		//Loop until timer gets below or equal to  0
		while (timer > 0)
		{

			//Remove the time that has passed since the last frame from the timer
			timer -= Time.deltaTime;

			//Set the audioSource's volume to be whatever the timer currently is
			audioSource.volume = timer;

			//Wait till the next frame to loop around again.
			yield return null;
		}

		//When we get here the timer should have gone from 1 to 0 over a 1 second.

		//Stop the audiosource from playing music
		audioSource.Stop();
	}
}