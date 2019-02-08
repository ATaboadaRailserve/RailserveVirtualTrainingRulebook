using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PullListOrganizor : MonoBehaviour {
	
	public Text[] texts;
	public Camera cameraObj;
		
	private List<int> carOrder;
	private int index = 0;
	private bool firing;
	private CarGenerator current;

	void Start () {
		carOrder = new List<int>();
	}

	void Update () {
		if (!EventSystem.current.IsPointerOverGameObject()){
			if (!firing && Input.GetAxis("Fire1") != 0){
				firing = true;
				RaycastHit hit = new RaycastHit();
				Ray ray = cameraObj.ScreenPointToRay(Input.mousePosition);
				
				if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.tag == "CarSet") {
					current = hit.collider.gameObject.GetComponent<CarGenerator>();
					index = 0;
					carOrder = current.carList;
					UpdateList(0);
				}
			}
			else if (firing && Mathf.Round(Input.GetAxis("Fire1")) < 0.001f)
				firing = false;
		}
	}
	
	public void UpdateList(int offset){
		index += offset;
		if (index < 0)
			index = 0;
		if (carOrder.Count > 10 && index > carOrder.Count - 10)
			index = carOrder.Count - 10;
		else if (carOrder.Count <= 10)
			index = 0;
		for(int i = 0; i < texts.Length; i++){
			texts[i].enabled = false;
			
			if (i+index < carOrder.Count){
				texts[i].enabled = true;
				texts[i].color = Color.white;
				texts[i].text = carOrder[i + index].ToString();
				for (int j = 0; j < current.pullList.Count; j++) {
					if (carOrder[i + index] == current.pullList[j])
						texts[i].color = Color.green;
				}
			}
		}
	}
}
