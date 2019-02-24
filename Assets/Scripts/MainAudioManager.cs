using UnityEngine.Audio;
using System;
using System.Collections;
using UnityEngine;
//original owner of the original script:brackeys
//https://www.youtube.com/watch?v=6OT43pvUyfY
//old.http://brackeys.com/wp-content/FilesForDownload/AudioManager.zip
public class MainAudioManager : MonoBehaviour
{

    public static MainAudioManager instance;

    public AudioMixerGroup mixerGroup;

    public Sound[] sounds;

    int currentMusicIndex;//specifies which music is playing at the moment

    bool musicPlaying = false;
    Coroutine musicCoroutine;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
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
            //s.source.loop = s.loop;
            //s.source.spatialBlend = s.spatialBlend;
            //s.source.maxDistance = s.max3DDistance;
            //s.source.rolloffMode = s.rolloffMode;

            s.source.outputAudioMixerGroup = mixerGroup;
        }

        //start main music sound
        StartMusic();

    }

    private void Start()
    {
        foreach (Sound s in sounds)
        {
            if (s.playOnInstantiate) s.source.Play();//s.source.playonawake can't do this
        }
    }

    
    

    /// <summary>
    /// plays sounds that start with name and end with an int less than number of clips, consecutively(sounds must be in sounds array)
    /// currently used only for main music
    /// </summary>
    /// <param name="name">initial name of clips</param>
    /// <param name="numberOfClips">number of clips</param>
    /// <returns></returns>
    IEnumerator PlayConsecutiveSounds(string name, int numberOfClips)
    {
        currentMusicIndex = UnityEngine.Random.Range(0, sounds.Length);
        for (; currentMusicIndex < numberOfClips; currentMusicIndex++) 
        {
            Play(name + currentMusicIndex);
            yield return new WaitForSecondsRealtime(sounds[currentMusicIndex].clip.length);
            if (currentMusicIndex == numberOfClips - 1) currentMusicIndex = 0;
        }
    }

    

    /// <summary>
    /// start main music & the coroutine that deals with main music
    /// </summary>
    public void StartMusic()
    {
        if (musicPlaying) return;

        musicPlaying = true;
        musicCoroutine= StartCoroutine(PlayConsecutiveSounds("MainMusic", sounds.Length));
    }
    
    /// <summary>
    /// stops main music & the coroutine that deals with main music
    /// </summary>
    public void StopMusic()
    {
        musicPlaying = false;
        sounds[currentMusicIndex].source.Stop();
        StopCoroutine(musicCoroutine);
    }

    /// <summary>
    /// plays audio
    /// </summary>
    /// <param name="sound">name of the sound to play</param>
    public void Play(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance, s.volumeVariance));
        s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance, s.pitchVariance));


        s.source.Play();
    }



}
