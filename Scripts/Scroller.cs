using UnityEngine;
using System.Collections;

public class Scroller : MonoBehaviour {

    Renderer myRenderer;

    void Awake()
    {
        myRenderer = GetComponent<Renderer>();
    }

	// Update is called once per frame
	void Update () {
        Vector2 offset = new Vector2(0, Time.time * 0.0075f);
        myRenderer.material.mainTextureOffset = offset;
	}
}
