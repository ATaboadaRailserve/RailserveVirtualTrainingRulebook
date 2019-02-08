using UnityEngine;
using System.Collections;

public class CarSpawnerGame : MonoBehaviour {
	
	public TutCarMover mover;
	public GameObject[] carList;
	public float carLength = 11.2f;
	public AnimationCurve carRarity;
	
	private int random;
	private GameObject[] cars;
	
	void Start () {
		if (mover)
			mover.percent = Random.value;
		else if (carList.Length != 0) {
			cars = new GameObject[9];
			for (int i = 0; i < 9; i++) {
				random = (int)(carRarity.Evaluate(Random.value)*carList.Length);
				GameObject temp = (GameObject)Instantiate(carList[random], Vector3.zero, Quaternion.identity);
				temp.transform.parent = transform;
				temp.transform.localPosition = new Vector3(carLength*(float)i,0,0);
				temp.transform.localEulerAngles = new Vector3(0,90,0);
				temp.GetComponent<RedZone>().game = true;
				cars[i] = temp;
			}
			for (int i = 0; i < 8; i++) {
				if (Random.value > 0.8f || i==7) {
					Destroy(cars[i]);
					Destroy(cars[i+1]);
					break;
				}
			}
		}
	}
}
