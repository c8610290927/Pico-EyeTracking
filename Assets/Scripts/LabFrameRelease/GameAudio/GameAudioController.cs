using System.Collections;
using System.Collections.Generic;
using LabData;
using UnityEngine;

public class GameAudioController : MonoSingleton<GameAudioController> {

    public GameObject LoopPlay(AudioClip clip)
    {
        GameObject go = new GameObject("AudioPlayerTemp");
        var audioSource = go.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
        return go;
    }

    /// <summary>
    /// 在Target位置播放3D音效,volume音量，maxDistance代表最远能听到的距离
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="target"></param>
    /// <param name="volume"></param>
    /// <param name="maxDistance"></param>
    public void PlayOneShot(AudioClip clip, GameObject target,float volume=0.5f,float maxDistance=3f)
    {
        var audioSource = target.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1;
        audioSource.spread = 360;
        audioSource.volume = volume;
        audioSource.maxDistance = maxDistance;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.clip = clip;
        StartCoroutine(playCoroutine(audioSource));
    }


    public void PlayOneShot(AudioClip clip)
    {
        GameObject go = new GameObject("AudioPlayerTemp");
        var audioSource = go.AddComponent<AudioSource>();
        audioSource.clip = clip;
        StartCoroutine(playCoroutine(audioSource));
    }


    IEnumerator playCoroutine(AudioSource audioSource)
    {
        float time = audioSource.clip.length;
        audioSource.Play();

        yield return new WaitForSeconds(time);
        DestroyImmediate(audioSource.gameObject);
    }
   
}
