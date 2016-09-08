using UnityEngine;
using System.Collections;

public class SplashScreenDelayed : MonoBehaviour {
	public float delayTime = 5;

	// Use this for initialization
	IEnumerator Start () 
	{
		yield return new WaitForSeconds (delayTime);

		Application.LoadLevel (1);
	}
}
