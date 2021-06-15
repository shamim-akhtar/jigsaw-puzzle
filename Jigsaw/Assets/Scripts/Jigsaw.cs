using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Patterns;

public class Jigsaw : SplitImage
{
    public Button mPlayButton;
    // regions to place all the tiles.
    List<Rect> mRegions = new List<Rect>();

    private FiniteStateMachine mFsm = new FiniteStateMachine();
    enum GameStates
    {
        WAITING,
        SHUFFLING,
        PLAYING,
        WIN,
        SHOW_SOLUTION,
    }

    // Start is called before the first frame update
    void Start()
    {
        base.CreateJigsawTiles();
        mRegions.Add(new Rect(mTilesX * 100 + 50.0f, 0.0f, 0, (mTilesY - 1)* 100));
        mRegions.Add(new Rect(-200.0f, 0.0f, 0, (mTilesY -1) * 100));

        mFsm.Add(new State((int)GameStates.WAITING, OnEnterWaiting));
        mFsm.Add(new State((int)GameStates.SHUFFLING, OnEnterShuffling));
        mFsm.Add(new State((int)GameStates.PLAYING, OnEnterPlaying, null, OnUpdatePlaying));
        mFsm.Add(new State((int)GameStates.WIN, OnEnterWin));
        mFsm.Add(new State((int)GameStates.SHOW_SOLUTION, OnEnterShowSolution));

        mFsm.SetCurrentState((int)GameStates.WAITING);
    }

    void OnEnterWaiting()
    {
        mPlayButton.gameObject.SetActive(true);
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
            mFsm.SetCurrentState((int)GameStates.WIN);
        }
    }

    void OnEnterWin()
    {
        mPlayButton.gameObject.SetActive(true);
        Debug.Log("Congratulations! Puzzle splved.");
    }

    void OnEnterShowSolution()
    {

    }

    public void OnClicplPlayButton()
    {
        mFsm.SetCurrentState((int)GameStates.SHUFFLING);
    }

    IEnumerator Coroutine_Shuffle()
    {
        for(int i = 0; i < mTilesX; ++i)
        {
            for(int j = 0; j < mTilesY; ++j)
            {
                Shuffle(mGameObjects[i, j]);
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
        mFsm.SetCurrentState((int)GameStates.PLAYING);
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
        for(int i = 0; i < mTilesX; ++i)
        {
            for(int j = 0; j < mTilesY; ++j)
            {
                if (mGameObjects[i, j].transform.position.x != i * 100.0f ||
                    mGameObjects[i, j].transform.position.y != j * 100.0f)
                    return false;
            }
        }
        return true;
    }
}
