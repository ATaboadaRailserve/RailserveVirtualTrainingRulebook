using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IDController : MonoBehaviour {
	public int pullAmount = 4;
	public List<TextToTexture> cars;
	public int[] pullList;
	
	// Kyle Addatives
	public LocoScript leaf;
	public Balloons balloons;
	public AvatarChanger avatar;
	
	void Start(){
		//balloons.gameObject.SetActive(false);
		cars = new List<TextToTexture>();
		pullList = new int[pullAmount];
		StartCoroutine("PullList");
	}
	
	public void ChockOneOff (int ID) {
		if (pullList.Length <= 1) {
			balloons.gameObject.SetActive(true);
			leaf.Win();
			balloons.Win();
			avatar.Win();
		} else {
			List<int> temp = new List<int>();
			for (int i = 0; i < pullList.Length; i++) {
				if (pullList[i] != ID)
					temp.Add(pullList[i]);
			}
			pullList = temp.ToArray();
		}
	}
	
	IEnumerator PullList(){
		yield return null;
		bool check;
		for(int i = 0; i < pullAmount; i++){
			check = false;
			while(!check){
				check = true;
				pullList[i] = (int)Random.Range(0, cars.Count);
				for(int j = 0; j < i; j++)
					if(pullList[i] == pullList[j])
						check = false;
				yield return null;
			}
		}
		for(int i = 0; i < pullAmount; i++) {
			//cars[pullList[i]].SetToPull();
			pullList[i] = cars[pullList[i]].number;
		}
	}
}
