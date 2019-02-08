using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedClipboard : MonoBehaviour {

    public float transitionTime = 0.5f;
    public float stayTime = 2.0f;
    public Transform pivot;
    public Transform up;
    public Transform down;
    public InteractionHandler.InvokableState OnStart;
    public InteractionHandler.InvokableState OnEnd;

	private float co_t;
    private Coroutine co;

    public void Start()
    {
        pivot.gameObject.SetActive(false);
    }

    public void ShowClipboard()
    {
        if (co != null)
        {
			co_t = 0.0f;
            //StopCoroutine(co);
            //OnEnd.Invoke();
        }
		else
			co = StartCoroutine(ShowClipboardAnimation());
    }
	
	public void StopClipboardAnimation()
	{
		Debug.Log("STOPPING ANIMATION");
		if(co != null)
		{
			StopCoroutine(co);
			pivot.gameObject.SetActive(false);
			OnEnd.Invoke();
			co = null;
		}
	}

    IEnumerator ShowClipboardAnimation()
    {
		Debug.Log("STARTING ANIMATION");
        OnStart.Invoke();
        pivot.transform.rotation = down.transform.rotation;
        pivot.gameObject.SetActive(true);

        for (float t = 0.0f; t < transitionTime; t += Time.deltaTime)
        {
            pivot.transform.rotation = Quaternion.Slerp(down.rotation, up.rotation, t / transitionTime);
            yield return null;
        }

        pivot.transform.rotation = up.transform.rotation;
		for(co_t = 0.0f; co_t < stayTime; co_t += Time.deltaTime)
			yield return null;
        //yield return new WaitForSeconds(stayTime);

        for (float t = 0.0f; t < transitionTime; t += Time.deltaTime)
        {
            pivot.transform.rotation = Quaternion.Slerp(up.rotation, down.rotation, t / transitionTime);
            yield return null;
        }

        pivot.gameObject.SetActive(false);
        OnEnd.Invoke();
		co = null;
    }
}
