using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayMovie : MonoBehaviour {
	bool startnow = false;
	//public float timeToPlay;
	//public Light light;
	//public float lightIntence;
	/*void Start () {
		if (GetComponent<Renderer>()) {
			((MovieTexture)GetComponent<Renderer>().material.mainTexture).Play();
			GetComponent<AudioSource>().clip = ((MovieTexture)GetComponent<Renderer>().material.mainTexture).audioClip;
			GetComponent<AudioSource>().Play();
		}
		else if (GetComponent<RawImage>()) {
			((MovieTexture)GetComponent<RawImage>().texture).Play();
			GetComponent<AudioSource>().clip = ((MovieTexture)GetComponent<RawImage>().texture).audioClip;
			GetComponent<AudioSource>().Play();
		}
	}*/
	Renderer r;
	MovieTexture movie;
	bool nullExceptionOccured;
	
	void Start()
	{
		gameObject.GetComponent<MeshRenderer> ().enabled = false;
		//light.intensity = 0;
		r = GetComponent<Renderer>();
		movie = (MovieTexture)r.material.mainTexture;
		if(movie == null)
		{
			Debug.LogWarning("No MovieTexture found on " + gameObject.name + ". Skipping movie.");
			nullExceptionOccured = true;
			return;
		}
		movie.loop = true;
		//StartCoroutine (PlayVideo (timeToPlay));
		StartCoroutine (PlayVideo (0f));
	}
	void Update()
	{
		if (startnow && !nullExceptionOccured) {
			movie.Play ();	
		}
	}
	IEnumerator PlayVideo(float i)
	{
		yield return new WaitForSeconds (i);
		gameObject.GetComponent<MeshRenderer> ().enabled = true;
		//light.intensity = lightIntence;
		startnow = true;
	}
}
