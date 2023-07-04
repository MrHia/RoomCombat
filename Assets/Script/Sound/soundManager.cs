using UnityEngine;
using UnityEngine.Audio;
using System;

public class soundManager : MonoBehaviour {
    public Sound[] sounds;
    // Start is called before the first frame update
    private void Awake() {
        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
            s.source.pitch = s.pitch;

        }
    }
    private void Start() {
        Play("Theme");
    }
    public void TurnOffSound() {
        AudioListener.volume = 0;
    }
    public void TurnOnSound() {
        AudioListener.volume = 0.8f;
    }
    public void Play(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) {
            return;
        }
        s.source.Play();
    }
}
