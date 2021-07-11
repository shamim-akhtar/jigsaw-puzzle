using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{
    public Image Filler;
    public AudioSource mAudioSource;
    public AudioClip mAudioClip;

    private void Start()
    {
        StartCoroutine(Coroutine_ApplyFadeout());
        mAudioSource.PlayOneShot(mAudioClip);
    }

    IEnumerator Coroutine_ApplyFadeout()
    {
        yield return new WaitForSeconds(mAudioClip.length);
        yield return StartCoroutine(Utils.Coroutine_FadeIn(Filler, 1.0f));

        GameApp.Instance.StartShufflePlay();
        SceneManager.LoadScene("JigsawImageSelection");
    }
}
