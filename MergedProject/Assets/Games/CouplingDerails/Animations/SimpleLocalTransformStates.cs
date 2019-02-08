using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class SimpleLocalTransformStates : MonoBehaviour {

	public int startIndex = 0;
	public bool doPosition = false;
	public bool doRotation = false;
	public bool doScale = false;
	public bool pushDefault = true;
	public AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);
	public List<State> states;
	
	private int currentIndex;
	
	public class State
	{
		float time;
		public Vector3 localPosition;
		public Quaternion localRotation;
		public Vector3 localScale;
		public UnityEvent OnTransition;
		public UnityEvent OnComplete;
	}
	
	void Start()
	{
		if (pushDefault)
		{
			State s = new State();
			s.localPosition = transform.localPosition;
			s.localRotation = transform.localRotation;
			s.localScale = transform.localScale;
			states.Add(s);
		}
		startIndex = Mathf.Min(startIndex, states.Count - 1);
	}
	
	public void TransformTo(int index)
	{
		
	}
}
