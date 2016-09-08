using UnityEngine;
using System.Collections;

public class LoadImage : MonoBehaviour {

	public Texture image;
	public GameObject innerSphere;

	// Use this for initialization
	void Start () {
		if (image != null) {
			GetComponent<Renderer> ().material.mainTexture = image;
		}
		if (image != null && innerSphere != null) {
			innerSphere.GetComponent<Renderer>().material.mainTexture = image;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
