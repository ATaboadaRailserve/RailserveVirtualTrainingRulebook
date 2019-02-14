using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformTo : MonoBehaviour {

    public List<Transition> transitions;
    private Coroutine active;

    [System.Serializable]
    public class Transition
    {
        public string name;
        public Transform from;
        public Transform to;
        public bool copyTransforms = true;
        public float length;
        public bool doPosition = true;
        public bool doRotation = true;
        public bool doScale = false;
		public bool instant = false;
        public AnimationCurve curve;
        public InteractionHandler.InvokableState OnStart;
        public InteractionHandler.InvokableState OnEnd;
    }

	public void TransitionTo(int index)
    {
        if (index >= 0 && index < transitions.Count)
            StartTransition(transitions[index]);
    }

    public void TransitionTo(string name)
    {
        foreach(Transition t in transitions)
        {
            if(t.name == name)
            {
				if(!t.instant)
				{
					StartTransition(t);
					break;
				}
				else
				{
					InstantTransition(t);
					break;
				}
            }
        }
    }

    void StartTransition(Transition trans)
    {
        if(trans != null)
        {
            if (active != null)
                StopCoroutine(active);
            active = StartCoroutine(DoTransition(trans));
        }
    }
	
	void InstantTransition(Transition trans)
	{
		trans.OnStart.Invoke();
		Vector3 copyFromPosition = trans.from.position;
        Quaternion copyFromRotation = trans.from.rotation;
        Vector3 copyFromScale = trans.from.localScale;
        Vector3 copyToPosition = trans.to.position;
        Quaternion copyToRotation = trans.to.rotation;
        Vector3 copyToScale = trans.to.localScale;
		
		if (trans.doPosition)
			 transform.position = copyToPosition;
        if (trans.doRotation)
			 transform.rotation = copyToRotation;
		if (trans.doScale)
			transform.localScale = copyToScale;
		
		trans.OnEnd.Invoke();
	}

    IEnumerator DoTransition(Transition trans)
    {
        Vector3 copyFromPosition = trans.from.position;
        Quaternion copyFromRotation = trans.from.rotation;
        Vector3 copyFromScale = trans.from.localScale;
        Vector3 copyToPosition = trans.to.position;
        Quaternion copyToRotation = trans.to.rotation;
        Vector3 copyToScale = trans.to.localScale;
        if (trans.curve == null || trans.curve.length == 0)
        {
            trans.curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }

        trans.OnStart.Invoke();
        for(float t = 0.0f; t < trans.length; t += Time.deltaTime)
        {
            float curvePos = trans.curve.Evaluate(t / trans.length);
            if (trans.copyTransforms)
            {
                if (trans.doPosition)
                    transform.position = Vector3.Lerp(copyFromPosition, copyToPosition, curvePos);
                if (trans.doRotation)
                    transform.rotation = Quaternion.Lerp(copyFromRotation, copyToRotation, curvePos);
                if (trans.doScale)
                    transform.localScale = Vector3.Lerp(copyFromScale, copyToScale, curvePos);
            }
            else
            {
                if (trans.doPosition)
                    transform.position = Vector3.Lerp(trans.from.position, trans.to.position, curvePos);
                if (trans.doRotation)
                    transform.rotation = Quaternion.Lerp(trans.from.rotation, trans.to.rotation, curvePos);
                if (trans.doScale)
                    transform.localScale = Vector3.Lerp(trans.from.localScale, trans.to.localScale, curvePos);
            }
            yield return null;
        }
		if (trans.doPosition)
			 transform.position = copyToPosition;
        if (trans.doRotation)
			 transform.rotation = copyToRotation;
		if (trans.doScale)
			transform.localScale = copyToScale;
        trans.OnEnd.Invoke();
    }
}
