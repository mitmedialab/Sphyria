using UnityEngine;
using System.Collections;

public class gaze : MonoBehaviour
{

	// Use this for initialization
	float timeScale = 3000f;
	bool inProx = false;
	bool inSphere = false;

	AudioSource soundByte;
	AudioSource ambientSound;

	GameObject soundTrigger;

	Vector3 enterPosition;
	Quaternion enterRotation;

	Mover moverScript;
	GameObject currSphere;

	Vector3 smallScale;
	Vector3 largeScale;

	int pressCount = 0;
	public float scale = 10;
	public Camera cam;
	public float treshold = 100;
	public float clickDistance = 10;
	public GameObject reticleObj;
	public GvrReticle reticle;
	public Vector3 sphereScale;

	bool sphereHover = false;
	bool buttonHover = false;

	public Material reticleMat;
	Color reticleColor;
	public Color recordingColor;
	bool recording = false;


	// Use this for initialization
	void Start ()
	{
        
		moverScript = GetComponent<Mover> ();
		smallScale = sphereScale;
		largeScale = smallScale * scale;

		reticle = reticleObj.GetComponent<GvrReticle> ();
		reticleColor = reticleMat.color;
	}

	IEnumerator growUp (GameObject obj)
	{
		float progress = 0;
		currSphere = obj;

		for (int i = 0; i < obj.transform.childCount; i++) {
			obj.transform.GetChild (i).localScale *= scale;
		}
		while (progress <= 1) {
			obj.transform.localScale = Vector3.Lerp (sphereScale, largeScale, progress);

			progress += Time.deltaTime * timeScale;
			yield return null;
		}
		obj.transform.localScale = largeScale;
		yield return null;
	}

	IEnumerator shrink (GameObject obj)
	{
		float progress = 0;

		for (int i = 0; i < obj.transform.childCount; i++) {
			obj.transform.GetChild (i).localScale /= scale;
		}
		while (progress <= 1) {
			obj.transform.localScale = Vector3.Lerp (largeScale, smallScale, progress);
			progress += Time.deltaTime * timeScale;
			yield return null;
		}
		obj.transform.localScale = smallScale;
		yield return null;
	}

	void playSound ()
	{
		if (soundByte != null && soundByte.enabled && soundByte.clip != null) {
			ambientSound.Stop ();
			soundByte.Play ();
		}
	}

	void enterSphere ()
	{
		pressCount = 0;

		enterPosition = transform.position;
		enterRotation = transform.rotation;

		RaycastHit hitInfo;
		Vector3 fwd = transform.TransformDirection (Vector3.forward);
		if (Physics.Raycast (transform.position, fwd, out hitInfo, clickDistance)) {
			;
			if (hitInfo.collider.gameObject.name == "Photosphere" || hitInfo.collider.gameObject.name == "VSphere" || hitInfo.collider.gameObject.tag == "Photosphere") {

				sphereHover = true;
				reticle.SetGazeTarget (hitInfo.point, true);

				if (Input.GetMouseButtonDown (0)) {
					sphereHover = false;
					reticle.GazeExit ();
					if (hitInfo.collider.gameObject.transform.childCount > 4 && hitInfo.collider.gameObject.transform.FindChild ("SoundTrigger").gameObject != null) {
						soundTrigger = hitInfo.collider.gameObject.transform.FindChild ("SoundTrigger").gameObject;
						soundTrigger.SetActive (true);
						soundByte = hitInfo.collider.gameObject.transform.FindChild ("SoundTrigger").gameObject.GetComponentInChildren<AudioSource> ();
					}
					StartCoroutine (growUp (hitInfo.collider.gameObject));
					//transform.position = hitInfo.collider.gameObject.transform.position;
					StartCoroutine (LerpInSphere (hitInfo.collider.gameObject.transform.position));
					print (transform.position);
					moverScript.speed = 0;
					inSphere = !inSphere;
					ambientSound = hitInfo.collider.gameObject.GetComponentInChildren<AudioSource> ();

					if (ambientSound.enabled && ambientSound.clip != null) {
						ambientSound.Play ();
						ambientSound.loop = true;
					}
				}
			} else if (sphereHover) {
				sphereHover = false;
				reticle.GazeExit ();
			}
		} else if (sphereHover) {
			sphereHover = false;
			reticle.GazeExit ();
		}
	}

	void exitSphere ()
	{
		ambientSound.Stop ();
		if (soundByte != null && soundByte.enabled && soundByte.clip != null && soundByte.isPlaying)
			soundByte.Stop ();
		//transform.position = enterPosition;
		StartCoroutine (LerpInSphere (enterPosition));
		transform.rotation = enterRotation;
		if (soundTrigger != null)
			soundTrigger.SetActive (false);
		moverScript.speed = 1;
		inSphere = !inSphere;
		StartCoroutine (shrink (currSphere));

	}

	public bool onExit = false;

	void FixedUpdate ()
	{
       
		if (inSphere) {
			RaycastHit hitInfo;
			Vector3 fwd = transform.TransformDirection (Vector3.forward);
			if (Physics.Raycast (transform.position, fwd, out hitInfo, clickDistance) && hitInfo.collider.gameObject.tag == "exit") {
				sphereHover = true;
				reticle.SetGazeTarget (hitInfo.point, true);

				if (Input.GetMouseButtonDown (0)) {
					onExit = false;
					reticle.GazeExit();
					exitSphere ();
				}

			}
			else if (onExit) {
				print ("exit");
				onExit = false;
				reticle.GazeExit ();
			}else if (Physics.Raycast (transform.position, fwd, out hitInfo, 10)) {
				if (hitInfo.collider.gameObject.name == "SoundTrigger" && hitInfo.collider.gameObject.activeSelf) {

					buttonHover = true;
					reticle.SetGazeTarget (hitInfo.point, true);

					if (Input.GetMouseButtonDown (0)) {
						buttonHover = false;
						reticle.GazeExit ();
						playSound ();
					}
				} else if (buttonHover) {
					buttonHover = false;
					reticle.GazeExit ();
				}
			} else if (buttonHover) {
				buttonHover = false;
				reticle.GazeExit ();
			} else if (Input.GetMouseButton (0) && !recording) {
				pressCount++;
				if (pressCount > 10) {
					//exitSphere ();
					recording = true;
					//StartCoroutine (Record ());
				}
			}
		} else {
			//if (Input.GetKeyDown(KeyCode.Space)) {
			//enterSphere();
			//}
			enterSphere ();
		}





		if (inSphere) {
			if (Input.touchCount > 0) {
				if (Input.GetTouch (0).phase == TouchPhase.Moved || Input.GetTouch (0).phase == TouchPhase.Stationary) {
					pressCount++;
					if (pressCount > 200) {
						exitSphere ();
					}
				}
			}
		} else {
			if (Input.touchCount > 0) {
				if (Input.GetTouch (0).phase == TouchPhase.Began) {
					enterSphere ();
				}
			}
		}

	}

	public float speed = 1f;

	IEnumerator LerpInSphere (Vector3 target)
	{
		float timeSinceStarted = 0f;
		while (true) {
			timeSinceStarted += Time.deltaTime * speed;
			transform.position = Vector3.Slerp (transform.position, target, timeSinceStarted);

			// If the object has arrived, stop the coroutine
			if (Vector3.Distance (transform.position, target) < 0.1f) {
				yield break;
			}

			// Otherwise, continue next frame
			yield return null;
		}
	}

	public float colorLerpTime = 0.2f;
	public float colorTimeScale = 300f;
	public Color currColor;
	public Color otherColor;

	IEnumerator Record ()
	{
		float currTime = 0;
		bool toRecordingColor = false;
		while (Input.GetMouseButton (0)) {
			currTime += Time.deltaTime * colorTimeScale;
			reticle.transform.localScale = new Vector3 (2, 2, 1);
			if (toRecordingColor) {
				if (currTime >= 1) {
					reticle.GetComponent<Renderer> ().material.color = recordingColor;
					toRecordingColor = false;
					currTime = 0;
				} else
					reticle.GetComponent<Renderer> ().material.color = Color.Lerp (otherColor, recordingColor, currTime);
			} else {
				if (currTime >= 1) {
					reticle.GetComponent<Renderer> ().material.color = otherColor;
					toRecordingColor = true;
					currTime = 0;
				} else
					reticle.GetComponent<Renderer> ().material.color = Color.Lerp (recordingColor, otherColor, currTime);
			}
			yield return null;
		}
		reticle.transform.localScale = new Vector3 (1, 1, 1);
		reticle.GetComponent<Renderer> ().material.color = reticleColor;
		recording = false;
	}
}
