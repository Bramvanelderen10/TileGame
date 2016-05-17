using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DraggingScript : MonoBehaviour
{
    public GameManager gameManager;

    //This code is for 2D click/drag gameobject
    //Please make sure to change Camera Projection to Orthographic
    //Add Collider (not 2DCollider) to gameObject  

    public GameObject gameObjectTodrag; //refer to GO that being dragged

    public Vector3 GOcenter; //gameobjectcenter
    public Vector3 touchPosition; //touch or click position
    public Vector3 offset;//vector between touchpoint/mouseclick to object center
    public Vector3 newGOCenter; //new center of gameObject

    RaycastHit hit; //store hit object information

    public bool draggingMode = false;

    public List<TileController> emptyTiles = new List<TileController>();

    // Use this for initialization
    void Start()
    {
        emptyTiles = gameManager.GetMoveAbleTiles();
    }

    // Update is called once per frame
    void Update()
    {
        //***********************
        // *** TOUCH TO DRAG ****
        //***********************
        foreach (Touch touch in Input.touches)
        {
            switch (touch.phase)
            {
                //When just touch
                case TouchPhase.Began:
                    //convert mouse click position to a ray
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);

                    //if ray hit a Collider ( not 2DCollider)
                    // if (Physics.Raycast(ray, out hit))
                    if (Physics.SphereCast(ray, 0.3f, out hit))
                    {
                        foreach(TileController tile in emptyTiles)
                        {
                            if (tile.gameObject == hit.collider.gameObject)
                            {

                                gameObjectTodrag = tile.gameObject.GetComponent<TileController>().fc.gameObject;
                                tile.gameObject.GetComponent<TileController>().fc = null;

                                GOcenter = gameObjectTodrag.transform.position;
                                touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                                offset = touchPosition - GOcenter;
                                draggingMode = true;
                            }
                        }                        
                    }
                    break;

                case TouchPhase.Moved:
                    if (draggingMode)
                    {
                        touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        newGOCenter = touchPosition - offset;
                        gameObjectTodrag.transform.position = new Vector3(newGOCenter.x, newGOCenter.y, GOcenter.z);
                    }
                    break;

                case TouchPhase.Ended:
                    if (draggingMode)
                    {
                        gameManager.PlaceFiller(gameObjectTodrag);
                        emptyTiles = gameManager.GetMoveAbleTiles();
                    }                    
                    draggingMode = false;                    
                    break;
            }
        }
    }
}