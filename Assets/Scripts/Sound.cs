using UnityEngine.Audio;
using UnityEngine;

//original owner of the original script:brackeys
//https://www.youtube.com/watch?v=6OT43pvUyfY
//old.http://brackeys.com/wp-content/FilesForDownload/AudioManager.zip
[System.Serializable]
public class Sound {

	public string name;

	public AudioClip clip;

	[Range(0f, 1f)] public float volume = .75f;
	[Range(0f, 1f)] public float volumeVariance = .1f;

	[Range(.1f, 3f)] public float pitch = 1f;
	[Range(0f, 1f)] public float pitchVariance = .1f;

	public bool loop = false;

    [Range(0f, 1f)] public float spatialBlend;
    [Range(0f, 500f)] public float max3DDistance = 30f;
    public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;

    public bool playOnInstantiate;
    
	public AudioMixerGroup mixerGroup;

	[HideInInspector]
	public AudioSource source;

}
