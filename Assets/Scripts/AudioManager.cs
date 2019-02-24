using UnityEngine.Audio;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
//original owner of the original script:brackeys
//https://www.youtube.com/watch?v=6OT43pvUyfY
//old.http://brackeys.com/wp-content/FilesForDownload/AudioManager.zip
public class AudioManager : NetworkBehaviour
{

	public static AudioManager instance;

	public AudioMixerGroup mixerGroup;

	public Sound[] sounds;

    public bool playInInspector = false;
    public string play;

    [HideInInspector] public bool running;//used for playing running sound, update this in movement script
    float runningSoundEndTime=0f;//used for scheduling running sounds
    [Range(0,5)] int randomRunningSound;//used for playing different running sounds
    


    void Awake()
	{
		//if (instance != null)
		//{
		//	Destroy(gameObject);
		//}
		//else
		//{
		//	instance = this;
		//	DontDestroyOnLoad(gameObject);
		//}

		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;
            s.source.spatialBlend = s.spatialBlend;
            s.source.maxDistance = s.max3DDistance;
            s.source.rolloffMode = s.rolloffMode;

			s.source.outputAudioMixerGroup = mixerGroup;
		}


	}

    private void Start()
    {
        foreach (Sound s in sounds)
        {
            if (s.playOnInstantiate) s.source.Play();//s.source.playonawake can't do this
        }

        //stop main music
        Debug.Log("stopping main music");
        GameObject.FindGameObjectWithTag("audioManager").GetComponent<MainAudioManager>().StopMusic();
    }



    private void OnDestroy()
    {
        if(this.gameObject.CompareTag("localPlayer"))
        {
            Debug.Log("re-starting main music");
            if (GameObject.FindGameObjectWithTag("audioManager") != null)
                GameObject.FindGameObjectWithTag("audioManager").GetComponent<MainAudioManager>().StartMusic();
        }
    }
    

    private void Update()
    {
        //TODO remove this
        if (playInInspector)
        {
            playInInspector = false;
            Play(play);
        }

        if (running)
        {
            if (runningSoundEndTime<Time.time)//need to do this, since running becomes true in movement script
            {
                randomRunningSound = UnityEngine.Random.Range(0, 6);//get an integer 0,1,2,3,4,5
                CmdPlay("Running"+randomRunningSound);
                runningSoundEndTime = Time.time + 0.4f;
            }

            running = false;
        }

    }

    /// <summary>
    /// plays sound on server which plays it on all clients
    /// </summary>
    /// <param name="sound">name of the sound to play</param>
    [Command]
    public void CmdPlay(string sound)
    {
        RpcPlay(sound);
    }

    [ClientRpc]
    private void RpcPlay(string sound)
    {
        Play(sound);
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

		s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance , s.volumeVariance ));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance , s.pitchVariance ));
        

        s.source.Play();
	}



}
