using UnityEngine;
using System.Collections;

public class zoomIn : MonoBehaviour {


    public GameObject target;
    public float speed = 0.001f;
	// Use this for initialization
	void Start () {
		StartCoroutine(waitForXSeconds());
	}

    IEnumerator MoveFunction() {
        float timeSinceStarted = 0f;
        while (true) {
            timeSinceStarted += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(transform.position, target.transform.position, timeSinceStarted);

            // If the object has arrived, stop the coroutine
            if (Vector3.Distance(transform.position, target.transform.position) < 1f) {
				transform.position = target.transform.position;
                yield break;
            }

            // Otherwise, continue next frame
            yield return null;
        }
    }
	public float waitTime = 2f;
	IEnumerator waitForXSeconds() {
		yield return new WaitForSeconds(waitTime);
		StartCoroutine(MoveFunction());
	}
}
