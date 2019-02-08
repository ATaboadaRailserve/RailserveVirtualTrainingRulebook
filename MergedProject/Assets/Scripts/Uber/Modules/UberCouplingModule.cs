using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UberCouplingModule : UberModule {

	public UberCoupling uberCoupling;
	public AnimatedCouplingCombination[] ACCs;
	
	[System.Serializable]
	public class AnimatedCouplingCombination
	{
		public string Name;
		public string Part1;
		public string Part2;
		public string Part3;
		
		public UnityEvent OnACCSetup;
	}
	
	private AnimatedCouplingCombination activeCombination;
	
	public void SetupRandomACC()
	{
		if(ACCs.Length > 0)
		{
			int randIndex = Random.Range(0, ACCs.Length);
			activeCombination = ACCs[randIndex];
			activeCombination.OnACCSetup.Invoke();
		}
		else
			Debug.LogError("No Animated Coupling Combinations");
	}
	
	public void StartACCPart1()
	{
		if(activeCombination != null)
			uberCoupling.StartScenario(activeCombination.Part1);
		else
			Debug.LogError("No prepared Animated Coupling Combination");
	}
	
	public void StartACCPart2()
	{
		if(activeCombination != null)
			uberCoupling.StartScenario(activeCombination.Part2);
		else
			Debug.LogError("No prepared Animated Coupling Combination");
	}
	
	public void StartACCPart3()
	{
		if(activeCombination != null)
			uberCoupling.StartScenario(activeCombination.Part3);
		else
			Debug.LogError("No prepared Animated Coupling Combination");
	}
}
