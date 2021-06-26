using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Patterns;

public class Jigsaw : MonoBehaviour
{
    SplitImage mSplitImage = new SplitImage();

    public string mImageFilename;
    public SpriteRenderer mSpriteRenderer;
    public Transform TilesParent;
    public Button mPlayButton;
    public List<Rect> mRegions = new List<Rect>();
    public Material mShadowMaterial;

    private FiniteStateMachine<GameStates> mFsm = new FiniteStateMachine<GameStates>();
    enum GameStates
    {
        LOADING,
        SHUFFLING,
        PLAYING,
        WIN,
        SHOW_SOLUTION,
    }

    #region Jigsaw Game Data
    #endregion

    void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        mSplitImage.mImageFilename = mImageFilename;
        mSplitImage.mSpriteRenderer = mSpriteRenderer;
        mSplitImage.TilesParent = TilesParent;
        mSplitImage.mShadowMaterial = mShadowMaterial;

        mFsm.Add(new State<GameStates>(GameStates.LOADING, OnEnterLoading, null, null, null));
        mFsm.Add(new State<GameStates>(GameStates.SHUFFLING, OnEnterShuffling, null, null, null));
        mFsm.Add(new State<GameStates>(GameStates.PLAYING, OnEnterPlaying, null, OnUpdatePlaying, null));
        mFsm.Add(new State<GameStates>(GameStates.WIN, OnEnterWin, null, null, null));
        mFsm.Add(new State<GameStates>(GameStates.SHOW_SOLUTION, OnEnterShowSolution, null, null, null));

        mFsm.SetCurrentState(GameStates.LOADING);
    }

    void OnDestroy()
    {
        if(mFsm.GetCurrentState().ID == GameStates.PLAYING)
            mSplitImage.SaveGame();
    }

    bool LoadLevel()
    {
        // Load data asscociated with the game.
        if (!mSplitImage.LoadGame())
        {
            mSplitImage.CreateJigsawTiles();
        }
        else
        {
            // directly go to PLAY mode.
            mPlayButton.gameObject.SetActive(false);
            mFsm.SetCurrentState(GameStates.PLAYING);
        }
        return false;
    }

    void OnEnterLoading()
    {
        mPlayButton.gameObject.SetActive(true);
        LoadLevel();
    }

    void OnEnterShuffling()
    {
        Shuffle();
    }

    void OnEnterPlaying()
    {

    }

    void OnUpdatePlaying()
    {
        if (HasCompleted())
        {
            mFsm.SetCurrentState(GameStates.WIN);
        }
    }

    void OnEnterWin()
    {
        mPlayButton.gameObject.SetActive(true);
    }

    void OnEnterShowSolution()
    {

    }

    public void OnClicplPlayButton()
    {
        mFsm.SetCurrentState(GameStates.SHUFFLING);
    }

    IEnumerator Coroutine_Shuffle()
    {
        for(int i = 0; i < mSplitImage.mTilesX; ++i)
        {
            for(int j = 0; j < mSplitImage.mTilesY; ++j)
            {
                Shuffle(mSplitImage.mGameObjects[i, j]);
                yield return null;
            }
        }
    }

    void Shuffle(GameObject obj)
    {
        // determine the final position of the tile after shuffling.
        // which region.
        int regionIndex = Random.Range(0, mRegions.Count);
        // get a random point within the region.
        float x = Random.Range(mRegions[regionIndex].xMin, mRegions[regionIndex].xMax);
        float y = Random.Range(mRegions[regionIndex].yMin, mRegions[regionIndex].yMax);

        // final position of the tile.
        Vector3 pos = new Vector3(x, y, 0.0f);

        StartCoroutine(Coroutine_MoveOverSeconds(obj, pos, 1.0f));
    }

    // coroutine to swap tiles smoothly
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

    IEnumerator Coroutine_Delay(float duration)
    {
        yield return new WaitForSeconds(duration);
        mPlayButton.gameObject.SetActive(false);
        mFsm.SetCurrentState(GameStates.PLAYING);
    }

    void Shuffle()
    {
        StartCoroutine(Coroutine_Shuffle());
        StartCoroutine(Coroutine_Delay(1.0f));
    }

    // Update is called once per frame
    void Update()
    {
        mFsm.Update();
    }

    bool HasCompleted()
    {
        for(int i = 0; i < mSplitImage.mTilesX; ++i)
        {
            for(int j = 0; j < mSplitImage.mTilesY; ++j)
            {
                if (mSplitImage.mGameObjects[i, j].transform.position.x != i * 100.0f ||
                    mSplitImage.mGameObjects[i, j].transform.position.y != j * 100.0f)
                    return false;
            }
        }
        return true;
    }
}
