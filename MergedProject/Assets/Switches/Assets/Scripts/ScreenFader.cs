using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
	public Image i;

	public float fadeSpeed = 1.5f;          // Speed that the screen fades to and from black.

	private bool sceneStarting = true;      // Whether or not the scene is still fading in.

	private bool endingScene = false;

	private string sceneName;

	private float time;
	
	private SceneLoader sceneloader;

	void Awake ()
	{
		i.rectTransform.sizeDelta = new Vector2 (Screen.width+50, Screen.height +50);
		i.color = Color.black;
		sceneloader = GameObject.FindGameObjectWithTag ("SceneLoader").GetComponent<SceneLoader>();
		StartScene ();
	}


	void Update ()
	{
		// If the scene is starting...
		if(sceneStarting)
			// ... call the StartScene function.
			StartScene();
		if (endingScene)
			EndScene (sceneName);
	}


	void FadeToClear ()
	{
		// Lerp the colour of the texture between itself and transparent.
		time += Time.deltaTime / (fadeSpeed*100);
		i.color = Color.Lerp(i.color, Color.clear,  time);
	}


	void FadeToBlack ()
	{
		// Lerp the colour of the texture between itself and black.
		time += Time.deltaTime / (fadeSpeed*100);
		i.color = Color.Lerp(i.color, Color.black, time);

	}


	void StartScene ()
	{
		endingScene = false;
		// Fade the texture to clear.
		FadeToClear();

		// If the texture is almost clear...
		if(i.color.a <= 0.05f)
		{
			time = 0;
			// ... set the colour to clear and disable the GUITexture.
			i.color = Color.clear;
			i.enabled = false;

			// The scene is no longer starting.
			sceneStarting = false;
		}
	}


	public void EndScene (string name)
	{
		sceneStarting = false;
		sceneName = name;
		// Make sure the texture is enabled.
		i.enabled = true;
		endingScene = true;
		// Start fading towards black.
		FadeToBlack();

		// If the screen is almost black...
		if(i.color.a >= 0.95f)
		{
			// ... reload the level.
			//SceneManager.LoadScene (name);
			sceneloader.LoadScene(name);
		}
	}
}