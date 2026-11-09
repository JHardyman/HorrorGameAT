using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SheepyControl : MonoBehaviour
{
    public AudioClip [] Bleets;
    float nextBleetTime=0;
    bool bleetingEnabled = true;
    AudioSource audioSource;
    AudioReverbFilter audioFilter;

    public UnityEngine.Audio.AudioMixerGroup audioMixerGroup;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("sheep created");
        audioSource = gameObject.AddComponent<AudioSource>();
        System.Console.WriteLine($"audioSource created:{audioSource.ToString()}");

        System.Console.WriteLine($"audioMixerGroup:{audioMixerGroup.name}");
        audioSource.outputAudioMixerGroup = audioMixerGroup;
        audioSource.minDistance = 2.0f;     // anything nearer will be max volume
        audioSource.maxDistance = 10.0f;     //

        audioSource.spatialBlend = SoundObjectPlayer.spatialBlend;    // Sets how much this AudioSource is affected by 3D spatialisation calculations. 1.0 makes it fully 3d ... also makes it use distance for drop-off in AudioSource::CacheVolume()

    }

    // Update is called once per frame
    void Update()
    {
        if (bleetingEnabled && Time.realtimeSinceStartup > nextBleetTime)
        {
            nextBleetTime = Time.realtimeSinceStartup +  Random.Range(3.0f, 10.0f);
            audioSource.spatialize = SoundObjectPlayer.spatialize;  // when true we pass it through to the spatializer plugin
            audioSource.spatializePostEffects = SoundObjectPlayer.spatializePostEffects;
            audioSource.spatialBlend = SoundObjectPlayer.spatialBlend;
            audioSource.PlayOneShot(Bleets[ Random.Range(0,Bleets.Length)]);
        }
    }
}
