using UnityEngine;
public class RandomEvent : MonoBehaviour {
	
	[Range(0,1)]
	public float chance;
	public InteractionHandler.InvokableState onGreaterThan;
	public InteractionHandler.InvokableState onLessThan;
	
	void Start () {
		if (Random.value < chance) onLessThan.Invoke();
		else onGreaterThan.Invoke();
	}
}