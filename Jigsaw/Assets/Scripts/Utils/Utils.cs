using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Utils : MonoBehaviour
{
    static public IEnumerator Coroutine_FadeIn(Image image, float duration)
    {
        float dt = 0.0f;
        Color c = image.color;
        while (dt <= duration)
        {
            c = image.color;
            c.a = dt / duration;
            image.color = c;
            dt += Time.deltaTime;
            yield return null;
        }
        c = image.color;
        c.a = 1.0f;
        image.color = c;
    }
    static public IEnumerator Coroutine_FadeOut(Image image, float duration)
    {
        float dt = 0.0f;
        Color c = image.color;
        while (dt <= duration)
        {
            c = image.color;
            c.a = 1.0f - dt / duration;
            image.color = c;
            dt += Time.deltaTime;
            yield return null;
        }
        c = image.color;
        c.a = 0.0f;
        image.color = c;
    }
    static public IEnumerator Coroutine_KeepValue(Image image, float duration, float value)
    {
        float dt = 0.0f;
        Color c = image.color;
        while (dt <= duration)
        {
            c = image.color;
            c.a = value;
            image.color = c;
            dt += Time.deltaTime;
            yield return null;
        }
        c = image.color;
        c.a = value;
        image.color = c;
    }

}
