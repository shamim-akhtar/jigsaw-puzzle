using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGen : MonoBehaviour
{
  public string imageFilename;

  // The opaque sprite. 
  Sprite mBaseSpriteOpaque;

  // The transparent (or Ghost sprite)
  Sprite mBaseSpriteTransparent;

  // The game object that holds the opaque sprite.
  // This should be SetActive to false by default.
  GameObject mGameObjectOpaque;

  // The game object that holds the transparent sprite.
  GameObject mGameObjectTransparent;

  public float ghostTransparency = 0.2f;

  private void Start()
  {
    mBaseSpriteOpaque = LoadBaseTexture();
    mGameObjectOpaque = new GameObject();
    mGameObjectOpaque.name = imageFilename + "_Opaque";
    mGameObjectOpaque.AddComponent<SpriteRenderer>().sprite = mBaseSpriteOpaque;
    mGameObjectOpaque.GetComponent<SpriteRenderer>().sortingLayerName = "Opaque";

    mBaseSpriteTransparent = CreateTransparentView(mBaseSpriteOpaque.texture);
    mGameObjectTransparent = new GameObject();
    mGameObjectTransparent.name = imageFilename + "_Transparent";
    mGameObjectTransparent.AddComponent<SpriteRenderer>().sprite = mBaseSpriteTransparent;
    mGameObjectTransparent.GetComponent<SpriteRenderer>().sortingLayerName = "Transparent";

    // Hide the mBaseSpriteOpaque game object.
    mGameObjectOpaque.gameObject.SetActive(false);

    SetCameraPosition();
  }

  void SetCameraPosition()
  {
    Camera.main.transform.position = new Vector3(mBaseSpriteOpaque.texture.width / 2,
      mBaseSpriteOpaque.texture.height / 2, -10.0f);
    Camera.main.orthographicSize = mBaseSpriteOpaque.texture.width / 2;
  }

  Sprite LoadBaseTexture()
  {
    Texture2D tex = SpriteUtils.LoadTexture(imageFilename);
    if (!tex.isReadable)
    {
      Debug.Log("Error: Texture is not readable");
      return null;
    }

    if (tex.width % Tile.tileSize != 0 || tex.height % Tile.tileSize != 0)
    {
      Debug.Log("Error: Image must be of size that is multiple of <" + Tile.tileSize + ">");
      return null;
    }

    // Add padding to the image.
    Texture2D newTex = new Texture2D(
        tex.width + Tile.padding * 2,
        tex.height + Tile.padding * 2,
        TextureFormat.ARGB32,
        false);

    // Set the default colour as white
    for (int x = 0; x < newTex.width; ++x)
    {
      for (int y = 0; y < newTex.height; ++y)
      {
        newTex.SetPixel(x, y, Color.white);
      }
    }

    // Copy the colours.
    for (int x = 0; x < tex.width; ++x)
    {
      for (int y = 0; y < tex.height; ++y)
      {
        Color color = tex.GetPixel(x, y);
        color.a = 1.0f;
        newTex.SetPixel(x + Tile.padding, y + Tile.padding, color);
      }
    }
    newTex.Apply();

    Sprite sprite = SpriteUtils.CreateSpriteFromTexture2D(
        newTex,
        0,
        0,
        newTex.width,
        newTex.height);
    return sprite;
  }

  Sprite CreateTransparentView(Texture2D tex)
  {
    // Add padding to the image.
    Texture2D newTex = new Texture2D(
        tex.width,
        tex.height,
        TextureFormat.ARGB32,
        false);

    //for (int x = Tile.padding; x < Tile.padding + Tile.tileSize; ++x)
    for (int x = 0; x < newTex.width; ++x)
    {
      //for (int y = Tile.padding; y < Tile.padding + Tile.tileSize; ++y)
      for (int y = 0; y < newTex.height; ++y)
      {
        Color c = tex.GetPixel(x, y);
        if (x > Tile.padding && x < (newTex.width - Tile.padding) && y > Tile.padding && y < newTex.height - Tile.padding)
        {
          c.a = ghostTransparency;
        }
        newTex.SetPixel(x, y, c);
      }
    }

    newTex.Apply();

    Sprite sprite = SpriteUtils.CreateSpriteFromTexture2D(
        newTex,
        0,
        0,
        newTex.width,
        newTex.height);
    return sprite;
  }
}
