using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grader : MonoBehaviour {
    Exploder[] exploders;
    public int grade;
	// Use this for initialization
	void Start () {
        exploders = GameObject.FindObjectsOfType<Exploder>();
        for(int i = 0; i < exploders.Length; i++)
        {

        }
	}
	
	// Update is called once per frame
	void Update () {
        CheckGrade();
	}

    void CheckGrade()
    {
        float totalPossible = exploders.Length;
        float totalViewed = 0;
        for(int i = 0; i < exploders.Length; i++)
        {
            if (exploders[i].viewed)
                totalViewed++;
        }
        grade = (int)(totalViewed / totalPossible * 100);
        //Debug.Log(grade);
    }
}
