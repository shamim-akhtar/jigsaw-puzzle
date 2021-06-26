using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns;
using UnityEngine.SceneManagement;

public class GameApp : Singleton<GameApp>
{
    public AmbientSound mAmbientSound;
    public AudioClip[] mAudioClips;

    Dictionary<string, AudioClip> sceneAudios = new Dictionary<string, AudioClip>();

    void Start()
    {
        mAmbientSound.Play(mAudioClips[Random.Range(0, mAudioClips.Length)]);
        SceneManager.LoadScene("Jigsaw");
    }

    void Update()
    {

    }

    void OnEnable()
    {
        //Debug.Log("OnEnable called");
        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        //Debug.Log("OnDisable");
        //SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    Debug.Log("Loaded: " + scene.name);
    //    if (sceneAudios.ContainsKey(scene.name))
    //    {
    //        AudioClip clip = sceneAudios[scene.name];
    //        if (clip != null)
    //        {
    //            PlaySceneAudio(clip);
    //        }
    //    }
    //}

    void PlaySceneAudio(AudioClip clip)
    {
        mAmbientSound.Play(clip);
    }
}
