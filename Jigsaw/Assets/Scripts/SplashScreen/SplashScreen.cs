using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{
    public Image Filler;
    private void Start()
    {
        StartCoroutine(Coroutine_ApplyFadeout());
    }

    IEnumerator Coroutine_ApplyFadeout()
    {
        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(Utils.Coroutine_FadeIn(Filler, 1.0f));

        SceneManager.LoadScene("JigsawImageSelection");
    }
}
