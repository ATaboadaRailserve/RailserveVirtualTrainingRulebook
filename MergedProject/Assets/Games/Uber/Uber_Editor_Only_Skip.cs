using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Uber_Editor_Only_Skip : MonoBehaviour {
	public List<Skiper> submoduleList;
	
	// Use this for initialization
	void Start () {
		gameObject.SetActive(false);
	#if UNITY_EDITOR
		gameObject.SetActive(true);
	#endif
	}
	
	public void CompleteModule(int x)
	{
		submoduleList[x].OnStateBegin.Invoke();
		submoduleList[x].module.CompleteSubmodule();
		//for(int i = 0; i <= x; i++)
		//{
		//	submoduleList[i].CompleteSubmodule();
		//}
	}
	
	[System.Serializable]
    public class Skiper
    {
        public UberSubmodule module;
        public InteractionHandler.InvokableState OnStateBegin;

    }
	
}
