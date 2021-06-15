using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSound : MonoBehaviour
{
    public AudioSource mAudioSource1;
    public AudioSource mAudioSource2;

    protected bool active1 = true;

    public static IEnumerator Coroutine_PlayShot(AudioSource source, AudioClip clip, float vol = 1.0f)
    {
        float length = clip.length;
        source.volume = AudioListener.volume * vol;
        source.loop = false;
        source.PlayOneShot(clip);
        yield return new WaitForSeconds(length);
    }

    public void Play(AudioClip clip, float volume = 1.0f, float pitch=1.0f)
    {
        volume *= AudioListener.volume;
        if (active1)
        {
            Stop(mAudioSource1);
            Play(mAudioSource2, clip, volume, pitch);
            active1 = false;
        }
        else
        {
            Stop(mAudioSource2);
            Play(mAudioSource1, clip, volume, pitch);
            active1 = true;
        }
    }

    private void Play(AudioSource audioSource, AudioClip clip, float volume, float pitch)
    {
        audioSource.clip = clip;
        audioSource.pitch = pitch;
        audioSource.volume = 0.0f;
        audioSource.loop = true;
        audioSource.Play();
        StartCoroutine(StartFade(audioSource, 1.0f, volume));
    }

    private void Stop(AudioSource audioSource)
    {
        StartCoroutine(StartFade(audioSource, 1.0f, 0.0f));
    }

    public IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
    {
        if (audioSource == null) yield break;

        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }
}