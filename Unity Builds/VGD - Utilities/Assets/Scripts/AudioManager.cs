using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;
	public AudioMixerGroup mixerGroup;
	public Sound[] sounds;

    public AudioSource themeSource;
    [Range(0f, 1f)]
    public float startingVolume = 0.7f;

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

		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;

			s.source.outputAudioMixerGroup = mixerGroup;
		}
	}


    private void Start()
    {
        Play("MainMenuTheme");
        setVolume(startingVolume);
    }

    public void Play(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		s.source.Play();
	}

    public void setVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void ThemeTransition(string theme, float transitionTime)
    {
        Debug.Log("Started Theme Transition");
        StartCoroutine(TransitionCoroutine(theme, transitionTime));
    }

    private IEnumerator TransitionCoroutine(string theme, float transitionTime)
    {
        Sound s = Array.Find(sounds, item => item.name == theme);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            yield break;
        }

        // Fade out the current music
        float startVolume = themeSource.volume;
        float timer = 0f;

        while (timer < transitionTime)
        {
            timer += Time.deltaTime;
            themeSource.volume = Mathf.Lerp(startVolume, 0f, timer / (transitionTime/2));
            yield return null;
        }

        // Switch the music clip
        Debug.Log("Started Theme Transition");
        themeSource.Stop();
        themeSource.clip = s.clip;

        // Fade in the new music
        themeSource.Play();
        timer = 0f;

        while (timer < transitionTime)
        {
            timer += Time.deltaTime;
            themeSource.volume = Mathf.Lerp(0f, startVolume, timer / (transitionTime/2));
            yield return null;
        }

        // Ensure the volume is back to normal
        themeSource.volume = startVolume;
    }

}
