using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class TurnTableFader : MonoBehaviour
{
	public Image i;

	public float fadeSpeed = 1.5f;          // Speed that the screen fades to and from black.

	private bool sceneStarting = true;      // Whether or not the scene is still fading in.

	private bool endingScene = false;

	private string sceneName;

	void Awake ()
	{
        GetComponent<Canvas>().enabled = true;
		i.rectTransform.sizeDelta = new Vector2 (Screen.width+50, Screen.height +50);
		i.color = Color.black;
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
		i.color = Color.Lerp(i.color, Color.clear, fadeSpeed * Time.deltaTime);
	}


	void FadeToBlack ()
	{
		// Lerp the colour of the texture between itself and black.
		i.color = Color.Lerp(i.color, Color.black, fadeSpeed * Time.deltaTime);
	}


	void StartScene ()
	{
		// Fade the texture to clear.
		FadeToClear();

		// If the texture is almost clear...
		if(i.color.a <= 0.05f)
		{
			// ... set the colour to clear and disable the GUITexture.
			i.color = Color.clear;
			i.enabled = false;

            GetComponent<Canvas>().enabled = false;

			// The scene is no longer starting.
			sceneStarting = false;
		}
	}


	public void EndScene (string name)
	{
		sceneName = name;
		// Make sure the texture is enabled.
		i.enabled = true;
		endingScene = true;
		// Start fading towards black.
		FadeToBlack();

		// If the screen is almost black...
		if(i.color.a >= 0.95f)
			// ... reload the level.
			SceneManager.LoadScene (name);
	}
}