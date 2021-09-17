using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour{

    public Sound[] sounds;
    public AudioMixerGroup audioMixer;

    public bool isMenu = false;
    
    void Awake()
    {
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.spatialBlend = s.spatialBlend;

            s.source.outputAudioMixerGroup = audioMixer;
        }


    }

    void Start()
    {
        if (isMenu)
            Play("MenuMainTheme");
        else
            Play("MainTheme");
    }

    public void Play(string name)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);

        if(s == null)
        {
            Debug.LogWarning("Sound : " + name + " not found");
            return;
        }
        s.source.Play();
    }
}
