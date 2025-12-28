using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
  public bool isPressed = false;

  public UnityEvent onPressAndHold;
  public UnityEvent onRelease;

  public void OnPointerDown(PointerEventData eventData)
  {
    isPressed= true;
  }

  public void OnPointerUp(PointerEventData eventData)
  {
    isPressed = false;
  }

  void Update()
  {
    if(isPressed)
    {
      onPressAndHold.Invoke();
    }
    else
    {
      onRelease.Invoke();
    }
  }
}
