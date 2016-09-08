using UnityEngine;
using System.Collections;

public class popUp : MonoBehaviour {

    float timeScale = 1.5f;
    bool inProx = false;

    Mover moverScript;

    Vector3 smallScale;
    Vector3 largeScale;

    public float scale = 10;
    public Camera cam;
    public float treshold = 100;

	// Use this for initialization
	void Start () {

        smallScale = transform.localScale;
        largeScale = smallScale * scale;
	}

    IEnumerator growUp() {
        float progress = 0;

        while (progress <= 1) {
            transform.localScale = Vector3.Lerp(smallScale, largeScale, progress);
            progress += Time.deltaTime * timeScale;
            yield return null;
        }
        transform.localScale = largeScale;
        yield return null;
    }

    IEnumerator shrink() {
        float progress = 0;

        while (progress <= 1) {
            transform.localScale = Vector3.Lerp(largeScale, smallScale, progress);
            progress += Time.deltaTime * timeScale;
            yield return null;
        }
        transform.localScale = smallScale;
        yield return null;
    }

    // Update is called once per frame
    void Update () {
	    if(Vector3.Distance(transform.position, cam.transform.position) < treshold && !inProx) {
            StartCoroutine(growUp());
            inProx = !inProx;
            Debug.Log("I AM IN ");
        }
        if (Vector3.Distance(transform.position, cam.transform.position) > treshold && inProx) {
            StartCoroutine(shrink());
            inProx = !inProx;
            Debug.Log("I AM out ");
        }
    }
}
