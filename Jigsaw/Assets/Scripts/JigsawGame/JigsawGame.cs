using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns;
using UnityEngine.SceneManagement;

public class JigsawGame : BoardGen
{
    public Menu menu;

    [Tooltip("Set the regions where the tiles should be scrambled to")]
    public List<Rect> Regions = new List<Rect>();

    //private BoardGen mBoard = new BoardGen();
    public FiniteStateMachine<JigsawGameStates> Fsm { get; set; } = new FiniteStateMachine<JigsawGameStates>();

    new void Start()
    {
        // Add all the states to the state machine.
        Fsm.Add(new StateLoading(this));
        Fsm.Add(new StateWaiting(this));
        Fsm.Add(new StateScrambling(this));
        Fsm.Add(new StatePlaying(this));
        Fsm.Add(new StateCompleted(this));

        Fsm.SetCurrentState(JigsawGameStates.LOADING);

        menu.OnClickPlay += OnClickBtnScramble;
        menu.OnClickNext += OnClickNextGame;
    }

    void Update()
    {
        Fsm.Update();

        if(menu.BtnHint.Pressed)
        {
            ShowOpaqueImage(true);
        }
        else
        {
            ShowOpaqueImage(false);
        }
    }

    private void FixedUpdate()
    {
        Fsm.FixedUpdate();
    }

    #region Game play methods
    public void LoadLevel()
    {
        // Clear the tiles from the sorter before loading a new level.
        // Remember that it is static.
        Puzzle.Tile.TilesSorting.Clear();

        // Get the finename from the JigsawGameData singleton.
        ImageFilename = JigsawGameData.Instance.mFilename;

        // Create the Jigsaw board.
        // We will change the flow when we implement
        // savling and loading data from file.
        CreateJigsawBoardUsingCoroutines();

        // Reposition camera.
        CameraMovement cm = Camera.main.GetComponent<CameraMovement>();
        if (cm != null)
        {
            cm.RePositionCamera(NumTilesX, NumTilesY);
        }
    }

    public void OnFinishedLoading()
    {
        // After loading change the state to waiting.
        Fsm.SetCurrentState(JigsawGameStates.WAITING);
        for (int i = 0; i < NumTilesX; i++)
        {
            for (int j = 0; j < NumTilesY; ++j)
            {
                TileMovement tile = mTileGameObjects[i, j].GetComponent<TileMovement>();
                tile.OnTileInPlace += OnTileOnPlace;
            }
        }

        // Set the values to the menu.
        menu.SetTotalTiles(NumTilesX * NumTilesY);
        menu.SetTilesInPlace(JigsawGameData.Instance.mTotalTilesInCorrectPosition);
        menu.SetTimeInSeconds(JigsawGameData.Instance.mSecondsSinceStart);

    }

    public void OnClickBtnScramble()
    {
        Fsm.SetCurrentState(JigsawGameStates.SCRAMBLING);
    }

    public void OnClickNextGame()
    {
        // Load the next game.
        // Get a random index.
        int index = Random.Range(0, JigsawGameData.Instance.mNamedImages.Count);
        string filename = JigsawGameData.Instance.mNamedImages[index].Item2;
        while(filename == JigsawGameData.Instance.mFilename)
        {
            index = Random.Range(0, JigsawGameData.Instance.mNamedImages.Count);
            filename = JigsawGameData.Instance.mNamedImages[index].Item2;
        }
        JigsawGameData.Instance.mFilename = JigsawGameData.Instance.mNamedImages[index].Item2;

        // Load the same scene with a different image.
        SceneManager.LoadScene("JigsawGame");
    }


    void OnTileOnPlace(TileMovement tm)
    {
        JigsawGameData.Instance.mTotalTilesInCorrectPosition += 1;
        // We disable the tile for any movement.
        tm.enabled = false;

        SpriteRenderer spriteRenderer = tm.gameObject.GetComponent<SpriteRenderer>();

        // We then remove this tile from the tile sorter.
        Puzzle.Tile.TilesSorting.Remove(spriteRenderer);

        // We also change the name of the sorting layer.
        spriteRenderer.sortingLayerName = "TilesInPlace";
        if (JigsawGameData.Instance.mTotalTilesInCorrectPosition == mTileGameObjects.Length)
        {
            Fsm.SetCurrentState(JigsawGameStates.COMPLETED);
        }
        //mTextInPlaceTiles.text = mTotalTilesInCorrectPosition.ToString();
        menu.SetTilesInPlace(JigsawGameData.Instance.mTotalTilesInCorrectPosition);
    }
    #endregion

    #region Shuffling

    public void Shuffle()
    {
        StartCoroutine(Coroutine_Shuffle());
        StartCoroutine(Coroutine_DelayPlay(2.0f));
    }

    IEnumerator Coroutine_Shuffle()
    {
        for (int i = 0; i < NumTilesX; ++i)
        {
            for (int j = 0; j < NumTilesY; ++j)
            {
                Shuffle(mTileGameObjects[i, j]);
                yield return null;
            }
        }
    }

    void Shuffle(GameObject obj)
    {
        // determine the final position of the tile after shuffling.
        // which region.
        int regionIndex = Random.Range(0, Regions.Count);
        // get a random point within the region.
        float x = Random.Range(Regions[regionIndex].xMin, Regions[regionIndex].xMax);
        float y = Random.Range(Regions[regionIndex].yMin, Regions[regionIndex].yMax);

        // final position of the tile.
        Vector3 pos = new Vector3(x, y, 0.0f);

        StartCoroutine(Coroutine_MoveOverSeconds(obj, pos, 1.0f));
    }

    // coroutine to move tiles smoothly
    private IEnumerator Coroutine_MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = end;
    }

    IEnumerator Coroutine_DelayPlay(float duration)
    {
        yield return new WaitForSeconds(duration);
        Fsm.SetCurrentState(JigsawGameStates.PLAYING);
    }

    #endregion

    #region Timer
    public void StartTimer()
    {
        StartCoroutine(Coroutime_Timer());
    }

    public void StopTimer()
    {
        StopCoroutine(Coroutime_Timer());
    }
    IEnumerator Coroutime_Timer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            JigsawGameData.Instance.mSecondsSinceStart += 1;

            menu.SetTimeInSeconds(JigsawGameData.Instance.mSecondsSinceStart);
        }
    }

    #endregion
}
