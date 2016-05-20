using UnityEngine;
using System.Collections;

public class ScaleToBackground : MonoBehaviour {

    private Sprite sprite;
    private Camera cam;

	// Use this for initialization
	void Start () {
        sprite = this.gameObject.GetComponent<SpriteRenderer>().sprite;
        cam = Camera.main;
        float height = (float)Camera.main.orthographicSize * 2.0f;
        float width = height * Screen.width / Screen.height;
        Vector2 dimensions = new Vector2(width, height);
        ScaleSpriteToTile(this.gameObject, dimensions);
    }

    void ScaleSpriteToTile(GameObject sprite, Vector2 dimensions)
    {
        Sprite sp = sprite.GetComponent<SpriteRenderer>().sprite;

        Vector3 spriteBounds = SpriteLocalToWorld(sp);
        Vector3 localScale = sprite.transform.localScale;

        localScale.x = (localScale.x * dimensions.x) / spriteBounds.x;
        localScale.y = (localScale.y * dimensions.y) / spriteBounds.y * -1;
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
