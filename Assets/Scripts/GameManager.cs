using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public GameObject tilePrefab;
    public List<GameObject> tileFillPrefab;
    private List<GameObject> tileList;
    public System.Random rnd;

	// Use this for initialization
	void Start () {
        rnd = new System.Random();
        tileList = new List<GameObject>();
        CreatePlayfield();

        Input.multiTouchEnabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (CheckWinCondition())
        {
            print("win");
        }
        foreach ( Touch touch in Input.touches)
        {

        }
	}

    List<GameObject> GetMoveAbleTiles()
    {
        foreach (GameObject tile in tileList)
        {
            if (!tile.GetComponent<TileController>().fc)
            {

            }
        }

        return null;
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

            if (tileFillPrefab.Count > 0)
            {
                int random = rnd.Next(0, tileFillPrefab.Count - 1);
                GameObject fill = tileFillPrefab[random];
                tileFillPrefab.RemoveAt(random);

                GameObject fillObject = Instantiate(fill);
                ScaleSpriteToTile(fillObject, tileWidth);
                fillObject.transform.position = tilePosition;
                tc.fc = fillObject.GetComponent<FillController>();
            }

            tileList.Add(tile);
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
