using UnityEngine;
using System.Collections;

public class BackgroundController : MonoBehaviour {    

	// Use this for initialization
	void Awake () {
        Camera cam = Camera.main;

        float height = (float)Camera.main.orthographicSize * 2.0f;
        float width = height * Screen.width / Screen.height;
        transform.localScale = new Vector3(width / 10, 1.0f, height / 10);

        Vector3 position = transform.position;
        position.z = 11;
        transform.position = position;
    }

    // Update is called once per frame
    void Update () {
	
	}
}
