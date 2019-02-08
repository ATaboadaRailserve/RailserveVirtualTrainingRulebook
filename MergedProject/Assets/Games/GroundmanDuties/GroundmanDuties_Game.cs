using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundmanDuties_Game : MonoBehaviour {

    public InteractionHandler iHandler;
    public WarningSystem warningSystem;
    public DynamicRadialMenu wheelMenu;

    public bool debug = false;

    public bool isGame = true;
    public int pointGoal = 10;
    public int points = 0;
    public int pointDing = 1;
    public bool negativeDing = false;
    public int chockCount = 0;
    public int chockCountMin = 2;
    public GameObject[] Chocks;
    public GameObject[] KillChocks;
    public BoxCollider[] ChockSpot;
    public GameObject[] Triggers;

    public Problem[] Conflict;
    public Gate[] Gates;
    public bool allGood;
    public bool failure = false;
    public BaseLogic radio;
    public BaseLogic[] LookingAt;
    public BasicQuester quest;
    public InteractionHandler.InvokableState onSuccess;

    [System.Serializable]
    public struct Problem
    {
        public string name;
        public GameObject good;
        public GameObject bad;
        public bool clear;
        public InteractionHandler.InvokableState onClear;
    }

    // Use this for initialization
    void Start () {
        allGood = false;
        if (isGame == true)
        {
            ChockTrainGen();
            LoadingRackGen();
        }
	}
	
	// Update is called once per frame
	void Update () {

       // DetectConfirmation();
        LoadingRackUpdate();

    }

    

    //Detects to see if the radio was used extraneously or not
    public void DetectConfirmation ()
    {
        bool lookedAt = false;

        for (int i = 0; i < LookingAt.Length; i++)
        {
            Debug.Log("Check Location " + i);
            if (LookingAt[i].isTrue == true)
            {
                Debug.Log("FOUND! " + i);
                lookedAt = true;
            }
        }

        if (lookedAt == false)
        {
            iHandler.CustomGameState("That doesn't look out of place.");
        }
        //if (radio.IsTrue == true)
        //{
        //    radio.IsTrue = false;
        //    for (int i = 0; i < LookingAt.Length; i++)
        //    {
        //        if (LookingAt[i].IsTrue == true)
        //        {
        //            Conflict[i].clear = true;
        //            Conflict[i].onClear.Invoke();
        //            points++;
        //            Debug.Log("RadioTrue GOOD");
        //            return;
        //        }
        //    }
        //    Debug.Log("RadioTrue BAD");
        //    failure = true;
        //}
    }

    //Resets the chocks for the train
    public void ChockTrainGen ()
    {
        for (int i = 0; i < Chocks.Length; i++)
        {
            Chocks[i].SetActive (false);
        }

        for (int i = 0; i < ChockSpot.Length; i++)
        {
            ChockSpot[i].enabled = true;
        }

        for (int i = 0; i < KillChocks.Length; i++)
        {
            KillChocks[i].SetActive(false);
            //KillChocks[i].GetComponent<BasicInteractable>().IsInteractiable.Equals(false);
        }

        for (int i = 0; i < Triggers.Length; i++)
        {
            Triggers[i].SetActive(true);
        }
        chockCount = 0;
    }

    //Randomly generates the loading rack scenario assets
    public void LoadingRackGen ()
    {
        int trueCount = 0;
        for (int i = 0; i < Conflict.Length; i++)
        {
            if (Random.Range(0, 2) == 1)
            {
                Conflict[i].clear = true;
                Conflict[i].good.SetActive(true);
                Conflict[i].bad.SetActive(false);

                trueCount++;

                Debug.Log("True " + i);
            }
            else
            {
                Conflict[i].clear = false;
                Conflict[i].good.SetActive(false);
                Conflict[i].bad.SetActive(true);
                Debug.Log("False " + i);
            }
        }

        if (trueCount == (Conflict.Length))
        {
            //allGood = true;
            int temp = Random.Range(0, Conflict.Length);
            Conflict[temp].clear = false;
            Conflict[temp].good.SetActive(false);
            Conflict[temp].bad.SetActive(true);
            Debug.Log("False FORCED " + temp);
        }
        else
        {
            allGood = false;
        }
    }

    //Updates loading rack assets and checks boolean allGood status
    void LoadingRackUpdate ()
    {
        int trueCount = 0;

        for (int i = 0; i < Conflict.Length; i++)
        {
            if (Conflict[i].clear == true)
            {
                Conflict[i].good.SetActive(true);
                Conflict[i].bad.SetActive(false);

                trueCount++;
            }
            else// if (!debug)
            {
                Conflict[i].good.SetActive(false);
                Conflict[i].bad.SetActive(true);
            }
        }

        if (trueCount == (Conflict.Length))
        {
            allGood = true;
        }
        else
        {
            allGood = false;
        }
    }

    //Used to externally set the CLEAR status of Problem pairs
    public void MakeClear (int index)
    {
        Gates[index].IsTrue = false;
        Conflict[index].clear = true;
        points++;
        Debug.Log("MakeClear");
    }

    //Keeps up with the trucks chocked
    public void Chocked (GameObject chockPairing)
    {
        if (chockPairing.activeInHierarchy)
        {
            chockCount++;
            //if (chockCount >= chockCountMin)
            //{
            //    quest.Next();
            //    points++;
            //    chockCount = 0;
            //}
        }
        
        Debug.Log("Chocked");
    }

    //Ensures the MINIMUM number of trucks have been chocked, otherwise Lose()
    public void ChockCheck ()
    {
        if (chockCount < chockCountMin)
        {
            iHandler.CustomGameState("You failed to properly chock the train.");
        }
        else
        {
            quest.Next();
        }
    }

    //Removes points for improper action
    public void Ding()
    {
        if (!negativeDing)
        {
            points -= pointDing;
            if (points < 0)
            {
                points = 0;
            }
        }
        else
        {
            points -= pointDing;
        }

    }

    //Checks to see if the cargo transfer is clear
    public void CheckAllClear ()
    {
        if (allGood)
        {
            if (points >= pointGoal)
            {
                wheelMenu.overrideActivation = true;
                iHandler.Win();
            }
            else
            {
                ResetScene();
            }
        }
        else
        {
            iHandler.CustomGameState("Not everything is cleared.");
        }
    }

    //Allows the scene to cycle again after completing the previous round
    public void ResetScene()
    {
        ChockTrainGen();
        LoadingRackGen();
        quest.JumpToIndex(0);
        onSuccess.Invoke();
    }
}
