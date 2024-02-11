using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesGen : MonoBehaviour
{
  public string imageFilename;
  private Texture2D mTextureOriginal;

  // Start is called before the first frame update
  void Start()
  {
    CreateBaseTexture();
  }

  void CreateBaseTexture()
  {
    mTextureOriginal = SpriteUtils.LoadTexture(imageFilename);
    if(!mTextureOriginal.isReadable)
    {
      Debug.Log("Texture is nor readable");
      return;
    }

    SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
    spriteRenderer.sprite = SpriteUtils.CreateSpriteFromTexture2D(
      mTextureOriginal,
      0,
      0,
      mTextureOriginal.width,
      mTextureOriginal.height);
  }

  // Update is called once per frame
  void Update()
  {

  }
}
