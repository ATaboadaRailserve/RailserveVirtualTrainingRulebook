using UnityEngine;
using System.Collections;

// Notes for Kyle/Ray.
// 1) Textures must be imported as an advanced texture
// 2) Textures must be a power of 2, so the font is 1024x1024.  The black texture automatically gets resized to 1024 as well
// 3) There's an option of "Non-power of 2" that you should make larger if applicable.
// 4) The "format" at the bottom should be RGBA 32-bit
// 5) Texture must be read/write enabled

public class TextToTexture : MonoBehaviour {

	public string Text = "";
	public Texture2D fontTex;
	private Texture2D baseTex;
	public int heightOfFontTex = 1023;
	public int horizontalAlignment = 4;
	public int verticalAlignment = 20;
	public int number;
	public bool notTanker;

	// Bottom pixel of each row
	private int[] bottomYOfEachRow = {50, 117, 190, 269, 352};
	private int charWidth = 47;
	private IDController cars;

	// Use this for initialization
	void Start () {
		baseTex = (Texture2D)Instantiate(this.GetComponent<Renderer>().material.mainTexture);
		cars = GameObject.FindWithTag("Cars").GetComponent<IDController>();
		if(!cars)
		{
			Debug.LogWarning("Using inefficient method to find IDController because of multiple Cars tags in scene");
			cars = (IDController)GameObject.FindObjectOfType(typeof(IDController));
			if(!cars)
			{
				Debug.LogError("No IDController in scene");
			}
		}
		number = (int)Random.Range(600000,699999);
		
		if(cars && gameObject.name != "CarPlaque"){
			bool good = false;
			while(!good){
				good = true;
				foreach (TextToTexture n in cars.cars){
					if (number == n.number)
						good = false;
				}
			}
			SetText(number);
			cars.cars.Add(this);
			if (transform.parent && transform.parent.parent && transform.parent.parent.parent)
				transform.parent.parent.parent.SendMessage("AddNumber", number, SendMessageOptions.DontRequireReceiver);
		}
	}

	public void SetText(int car) {
		if (notTanker)
			Text = car.ToString();
		else
			Text = "UTLX " + car;
		MakeTexture();
	}

	// This is hard-coded to a specific font - "monofont"
	private Vector2 GetLetterLocation(char letter) {
		int val = (int)letter;
		int row = 0;

		// Space
		if (val == 32) {
			return new Vector2(0, 0);
		}
		// Upper-case
		if ((val >= 65)&&(val <= 90)) {
			val -=65;
			if (val > 12) {row++;val-=13;}
			return new Vector2(val*charWidth, heightOfFontTex-bottomYOfEachRow[row]);
		}
		// Lower-case
		if ((val >= 97)&&(val <= 122)) {
			row = 2;
			val -= 97;
			if (val > 12) {row++;val-=13;}
			return new Vector2(val*charWidth, heightOfFontTex-bottomYOfEachRow[row]);
		}
		// Numbers
		if ((val >= 48)&&(val <= 57)) {
			row = 4;
			val -= 48;
			return new Vector2(val*charWidth, heightOfFontTex-bottomYOfEachRow[row]);

		}
		return new Vector2(0, 0);
	}

	void MakeTexture() {
		Color[] letterTex;
		Vector2 charPos;

		for (int i = 0; i < Text.Length; i++) {
			charPos = GetLetterLocation(Text[i]);
			letterTex = fontTex.GetPixels((int)(charPos.x), (int)(charPos.y), 50, 50);
			baseTex.SetPixels(i*65 + horizontalAlignment, verticalAlignment, 50, 50, letterTex);
			baseTex.Apply();
		}
		this.GetComponent<Renderer>().material.mainTexture = baseTex;
	}
	
	/*
	public void SetToPull () {
		transform.parent.parent.parent.SendMessage("AddPull", number, SendMessageOptions.DontRequireReceiver);
		transform.parent.GetComponent<SmartTankerScript>().isPulled = true;
	}
	*/
}
