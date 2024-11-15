using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{

    private AudioSource musicSource;
    private List<AudioSource> sfxSources;

    private void Start()
    {
        sfxSources = new List<AudioSource>(FindObjectsOfType<AudioSource>());
        for (int i = sfxSources.Count - 1; i >= 0; i--)
        {
            if (sfxSources[i].gameObject.CompareTag("Music"))
            {
                musicSource = sfxSources[i];
                sfxSources.RemoveAt(i);  
            }
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSources != null)
            sfxSources.ForEach(s => s.volume = volume);
    }
}