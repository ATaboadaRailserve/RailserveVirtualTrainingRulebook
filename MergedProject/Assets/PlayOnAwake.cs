using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOnAwake : MonoBehaviour {

Animator _anim;
public string MyTrigger;
void Awake()
{
	_anim = GetComponent<Animator>();
}

	void OnEnable()
{

		_anim.SetTrigger(MyTrigger);

}
}