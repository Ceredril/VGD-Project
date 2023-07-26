using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEditorInternal;
using static System.TimeZoneInfo;
using System.Collections;

public class AudioManager : MonoBehaviour
{

	public static AudioManager instance;
    public AudioMixerGroup mixerGroup;
    public ThemeSound[] Themes;
    public Sound[] GlobalSounds;
    public Sound[] LocalSounds;

    [Range(0f, 2f)]
    public float generalVolume = 1f;
    [Range(0f, 5f)]
    public float themeTransitionTime = 2f;
    public AudioSource ThemeSource;

	void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
        }

        foreach (ThemeSound s in Themes)
        {
            s.source = ThemeSource;
        }

        foreach (Sound s in GlobalSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
		ThemePlay("MenuTheme");
    }

    public void GlobalPlay(string sound)
    {
        Sound s = Array.Find(GlobalSounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.volume = generalVolume * s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
        s.source.Play();
    }
    public void LocalPlay(string sound)
    {
        Sound s = Array.Find(LocalSounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.volume = generalVolume * s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

        s.source.Play();
    }
    public void ThemePlay(string theme)
    {
        ThemeSound s = Array.Find(Themes, item => item.name == theme);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source = ThemeSource;
        s.source.clip = s.clip;
        s.source.loop = s.loop;
        s.source.volume = generalVolume * s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
        s.source.Play();
    }



    public void setVolume(float volume)
    {
		generalVolume = volume;
    }

    public void ThemeTransition(string theme)
    {
        StartCoroutine(ChangeTheme(theme));
    }

    IEnumerator ChangeTheme(string theme)
    {
        float percentage = 0;
        while (ThemeSource.volume > 0f)
        {
            ThemeSource.volume = Mathf.Lerp(ThemeSource.volume, 0f, percentage);
            percentage += Time.deltaTime / (themeTransitionTime/2);
            yield return null;
        }
        ThemeSound s = Array.Find(Themes, item => item.name == theme);
        ThemeSource.clip = s.clip;
        s.source.loop = s.loop;
        while (ThemeSource.volume > generalVolume)
        {
            ThemeSource.volume = Mathf.Lerp(0f, generalVolume, percentage);
            percentage += Time.deltaTime / (themeTransitionTime/2);
            yield return null;
        }
    }

}
