using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RickAnimator : MonoBehaviour {

    [System.Serializable]
    public struct Rickimation
    {
        public string clipName;
        public float time;
    }

    public AnimationScrubber scrubber;
    public List<Rickimation> rickimations;
    Animator animator;

    void Start()
    {
        //print(name);
        //if (rickimations.Count <= 0)
        //    enabled = false;
        animator = GetComponent<Animator>();
        //rickimations.Sort((r1, r2) => r1.time.CompareTo(r2.time));
        //StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return null;
        //print(name);
        animator.Play(Animator.StringToHash(rickimations[0].clipName));
    }

    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).shortNameHash != Animator.StringToHash(rickimations[0].clipName))
        {
            animator.Play(rickimations[0].clipName, 0);
            enabled = false;
        }
    }
}
