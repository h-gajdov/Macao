using UnityEngine.Audio;
using UnityEngine;
using System;
using JetBrains.Annotations;

public class AudioManager : MonoBehaviour {
    public Sound[] sounds;

    public static AudioManager instance;

    public static float musicVolume;
    public static float sfxVolume;

    private void Awake() {
        if (instance == null) instance = this;
        else {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this);

        foreach(Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public static void UpdateVolumes() {
        foreach(Sound s in instance.sounds) {
            float multiplier = (s.isSFX) ? sfxVolume : musicVolume;
            s.source.volume = s.volume * multiplier;
        }
    }

    public static void Play(string name) {
        Sound s = Array.Find(instance.sounds, sound => sound.name == name);
        if(s == null) {
            Debug.LogWarning($"Sound: {name} not found!");
            return;
        }
        s.source.Play();
    }
}
