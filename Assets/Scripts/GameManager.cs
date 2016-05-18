using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public string sceneName;
    public GameObject tilePrefab;
    public List<GameObject> tileFillPrefab;
    public GameObject finalFillPrefab;
    public GameObject pauseMenuPrefab;
    private List<GameObject> tileList;
    private System.Random rnd;

	void Awake ()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        rnd = new System.Random();
        tileList = new List<GameObject>();
        CreatePlayfield();

        Input.multiTouchEnabled = false;
        GetMoveAbleTiles();
    }

	void Update ()
    {
        if (CheckWinCondition())
        {
            foreach (GameObject tile in tileList)
            {
                if (!tile.GetComponent<TileController>().fc)
                {
                    GameObject finalFiller = Instantiate(finalFillPrefab);
                    ScaleSpriteToTile(finalFiller, tile.transform.localScale.x);
                    Vector3 pos = finalFiller.transform.position;
                    pos = tile.transform.position;
                    pos.z -= 10;
                    finalFiller.transform.position = pos;
                    tile.GetComponent<TileController>().fc = finalFiller.GetComponent<FillController>();
                    GameObject pauseMenu = Instantiate(pauseMenuPrefab);
                }
            }

            
        }
	}

    //Hardcoded method based on a playfield of 3x3 IF PLAYFIELD CHANGES THIS DOES NOT WORK ANYMORE
    public List<TileController> GetMoveAbleTiles()
    {
        List<TileController> availableTiles = new List<TileController>();
        foreach (GameObject tile in tileList)
        {
            if (!tile.GetComponent<TileController>().fc)
            {
                int tileNumber = tile.GetComponent<TileController>().tileNumber;

                if ((tileNumber - 3) >= 0)
                    availableTiles.Add(tileList[tileNumber - 3].GetComponent<TileController>());

                if ((tileNumber + 3) <= 8)
                    availableTiles.Add(tileList[tileNumber + 3].GetComponent<TileController>());

                if ((tileNumber - 1) >= 0 && tileNumber != 0 && tileNumber != 3 && tileNumber != 6)
                    availableTiles.Add(tileList[tileNumber - 1].GetComponent<TileController>());

                if ((tileNumber + 1) <= 8 && tileNumber!= 2 && tileNumber != 5 && tileNumber != 8)
                    availableTiles.Add(tileList[tileNumber + 1].GetComponent<TileController>());
            }
        }

        return availableTiles;
    }

    public void PlaceFiller(GameObject filler)
    {
        List<GameObject> availableTiles = new List<GameObject>();

        GameObject destinationTile = null;
        foreach (GameObject tile in tileList)
        {
            if (!tile.GetComponent<TileController>().fc)
            {
                if (!destinationTile)
                {
                    destinationTile = tile;
                } else
                {
                    float distance = Vector3.Distance(destinationTile.transform.position, filler.transform.position);
                    float newDistance = Vector3.Distance(tile.transform.position, filler.transform.position);

                    if (newDistance < distance)
                    {
                        destinationTile = tile;
                    }
                }
                availableTiles.Add(tile);
            }
        }

        destinationTile.GetComponent<TileController>().fc = filler.GetComponent<FillController>();
        Vector3 fillerPos = filler.transform.position;
        fillerPos = destinationTile.transform.position;
        fillerPos.z -= 10;
        filler.transform.position = fillerPos;
    }

    void CreatePlayfield()
    {
        float height = (float)Camera.main.orthographicSize * 2.0f;
        float width = height * Screen.width / Screen.height;

        float result = (height >= width) ? width : height;

        List<Vector3> tilePositions = new List<Vector3>();

        float tileWidth = result / 3;
        float z = 10;

        tilePositions.Add(new Vector3(-tileWidth, tileWidth, z)); //tile 0
        tilePositions.Add(new Vector3(0, tileWidth, z));
        tilePositions.Add(new Vector3(tileWidth, tileWidth, z));
        tilePositions.Add(new Vector3(-tileWidth, 0, z));
        tilePositions.Add(new Vector3(0, 0, z));
        tilePositions.Add(new Vector3(tileWidth, 0, z));
        tilePositions.Add(new Vector3(-tileWidth, -tileWidth, z));
        tilePositions.Add(new Vector3(0, -tileWidth, z));
        tilePositions.Add(new Vector3(tileWidth, -tileWidth, z));
        int i = 0;
        foreach (Vector3 tilePosition in tilePositions)
        {
            GameObject tile = Instantiate(tilePrefab);

            Vector3 tilePos = tile.transform.position;
            tilePos.x = tilePosition.x;
            tilePos.y = tilePosition.y;
            tilePos.z = tilePosition.z + 10;
            tile.transform.position = tilePos;
            tile.transform.localScale = new Vector3(tileWidth, tileWidth, tileWidth);

            TileController tc = tile.GetComponent<TileController>();
            tc.tileNumber = i;

            tileList.Add(tile);
            i++;
        }

        FillTiles();
    }

    bool CheckIfSolvalbe()
    {
        int inversions = 0;
        for (int i = 0; i < tileList.Count; i++)
        {
            if (tileList[i].GetComponent<TileController>().fc)
            {
                int tileNumber = tileList[i].GetComponent<TileController>().tileNumber;
                int fillNumber = tileList[i].GetComponent<TileController>().fc.fillNumber;
                for (int y = i + 1; y < tileList.Count; y++)
                {
                    if (tileList[y].GetComponent<TileController>().fc && tileList[y].GetComponent<TileController>().fc.fillNumber < fillNumber)
                        inversions++;
                }
            }            
        }
        
        return inversions % 2 != 0;
    }

    void ResetTiles()
    {
        foreach (GameObject tile in tileList)
        {
            FillController fc = tile.GetComponent<TileController>().fc;
            tile.GetComponent<TileController>().fc = null;
            if (fc)
                Destroy(fc.gameObject);
        }
    }

    void FillTiles()
    {
        List<GameObject> tileFillers = new List<GameObject>(tileFillPrefab);
        foreach (GameObject tile in tileList)
        {
            if (tileFillers.Count > 0)
            {
                TileController tc = tile.GetComponent<TileController>();
                int random = rnd.Next(0, tileFillers.Count - 1);
                GameObject fill = tileFillers[random];
                tileFillers.RemoveAt(random);

                GameObject fillObject = Instantiate(fill);
                ScaleSpriteToTile(fillObject, tile.transform.localScale.x);
                fillObject.transform.position = tile.transform.position;
                Vector3 pos = fillObject.transform.position;
                pos.z -= 10;
                fillObject.transform.position = pos;

                tc.fc = fillObject.GetComponent<FillController>();
            }
        }

        if (CheckIfSolvalbe())
        {
            ResetTiles();
            FillTiles();
        }
    }

    bool CheckWinCondition()
    {
        bool result = true;

        for (int i = 0; i < 8; i++)
        {
            TileController tc = tileList[i].GetComponent<TileController>();
            if (!tc.fc || tc.fc.fillNumber != tc.tileNumber)
                result = false;
        }

        return result;
    }

    void ScaleSpriteToTile(GameObject sprite, float tileWidth)
    {
        Sprite sp = sprite.GetComponent<SpriteRenderer>().sprite;

        Vector3 spriteBounds = SpriteLocalToWorld(sp);
        Vector3 localScale = sprite.transform.localScale;

        localScale.x = (localScale.x * tileWidth) / spriteBounds.x;
        localScale.y = (localScale.y * tileWidth) / spriteBounds.y * -1;
        sprite.transform.localScale = localScale;
    }

    //Returns the width and height of a sprite in units
    Vector3 SpriteLocalToWorld(Sprite sp)
    {
        Vector3 pos = transform.position;
        Vector3[] array = new Vector3[2];

        Vector3 result = new Vector3(0, 0, 0);
        //top left
        array[0] = pos + sp.bounds.min;
        // Bottom right
        array[1] = pos + sp.bounds.max;

        result.x = array[1].x - array[0].x;
        result.y = array[0].y - array[1].y;

        return result;
    }

}
