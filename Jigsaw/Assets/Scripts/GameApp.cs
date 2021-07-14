using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Patterns;
using UnityEngine.SceneManagement;

public class GameApp : Singleton<GameApp>
{
    public AmbientSound mAmbientSound;
    public InterstitialAdsScript mAds;

    public List<AudioClip> mGameAudioClips = new List<AudioClip>();

    void Start()
    {
        SceneManager.LoadScene("SplashScreen");
    }

    private int mCurrentAudioIndex = 0;

    public void StartShufflePlay()
    {
        StartCoroutine(Coroutine_ShuffleAudio());
    }

    private void ChangeAudio()
    {
        int index = Random.Range(0, mGameAudioClips.Count);
        while (mCurrentAudioIndex == index)
        {
            index = Random.Range(0, mGameAudioClips.Count);
        }
        mCurrentAudioIndex = index;
        GameApp.Instance.mAmbientSound.Play(mGameAudioClips[mCurrentAudioIndex]);
    }

    IEnumerator Coroutine_ShuffleAudio()
    {
        while (true)
        {
            ChangeAudio();
            //float secs = Random.Range(60, 180);
            yield return new WaitForSeconds(mGameAudioClips[mCurrentAudioIndex].length);
        }
    }
}
