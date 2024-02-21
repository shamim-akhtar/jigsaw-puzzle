using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
  public delegate void DelegateOnClick();
  public DelegateOnClick btnPlayOnClick;

  public GameObject panelTopPanel;
  public GameObject panelBottomPanel;
  public GameObject panelGameCompletion;

  public Text textTime;
  public Text textTotalTiles;
  public Text textTilesInPlace;

  IEnumerator FadeInUI(GameObject panel, float fadeInDuration = 2.0f)
  {
    Graphic[] graphics = panel.GetComponentsInChildren<Graphic>();
    foreach(Graphic graphic in graphics)
    {
      graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 0.0f);
    }

    float timer = 0.0f;
    while(timer < fadeInDuration)
    {
      timer += Time.deltaTime;
      float normalisedTime = timer / fadeInDuration;
      foreach(Graphic graphic in graphics)
      {
        graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, normalisedTime);
      }
      yield return null;
    }
    foreach (Graphic graphic in graphics)
    {
      graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 1.0f);
    }
  }

  public void SetEnableBottomPanel(bool flag)
  {
    panelBottomPanel.SetActive(flag);
    if (flag)
    {
      FadeInUI(panelBottomPanel);
    }
  }

  public void SetEnableTopPanel(bool flag)
  {
    panelTopPanel.SetActive(flag);
    if (flag)
    {
      FadeInUI(panelTopPanel);
    }
  }

  public void OnClickPlay()
  {
    btnPlayOnClick?.Invoke();
  }

  public void SetTimeInSeconds(double tt)
  {
    System.TimeSpan t = System.TimeSpan.FromSeconds(tt);
    string time = string.Format("{0:D2} : {1:D2} : {2:D2}", t.Hours, t.Minutes, t.Seconds);

    textTime.text = time;
  }

  public void SetTotalTiles(int count)
  {
    textTotalTiles.text = count.ToString();
  }

  public void SetTilesInPlace(int count)
  {
    textTilesInPlace.text = count.ToString();
  }

  public void SetEnableGameCompletionPanel(bool flag)
  {
    panelGameCompletion.SetActive(flag);
    if(flag)
    {
      FadeInUI(panelGameCompletion);
    }
  }

  public void OnClickExit()
  {
    Application.Quit();
  }

  public void OnClickPlayAgain()
  {
    SceneManager.LoadScene("Scene_JigsawGame");
  }
}
