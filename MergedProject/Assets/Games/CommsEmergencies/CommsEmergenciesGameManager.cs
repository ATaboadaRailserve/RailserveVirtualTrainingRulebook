using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommsEmergenciesGameManager : MonoBehaviour {

    public int maxActivated = 3;
    public ClipboardList clipboard;
    public List<string> goals;
    public Findable[] harmlessFindable;
    public Findable[] harmfulFindable;
    public Findable[] emergencies;
    public InteractionHandler.InvokableState OnRadioFailed;
    public InteractionHandler.InvokableState OnClipboardFailed;
    public InteractionHandler.InvokableState OnFireFailed;
    public InteractionHandler.InvokableState OnToxicFailed;
    public InteractionHandler.InvokableState OnOilFailed;
    public InteractionHandler.InvokableState OnMissionSuccess;
    public InteractionHandler.InvokableState OnMissionFailure;

    [Header("Array Hospital")]
    public string removeName;
    public ArrayStr source = ArrayStr.Harmless;
	
	public int NumEmergencies {get {return goals.Count - 2;}}
	public int FoundEmergencies {get; private set;}

    public enum ArrayStr
    {
        Harmless,
        Harmful,
        Emergencies
    }

    List<string> completedGoals = new List<string>();

    [System.Serializable]
    public class Findable
    {
        public string name;
        public string[] blocks;
        public InteractionHandler.InvokableState OnActivated;
        public InteractionHandler.InvokableState OnDeactivated;
        
        public bool isActive { get; set; }
        public bool isBlocked { get; set; }
    }

    void SetFindableInactive(Findable f)
    {
        if (!f.isActive)
            return;

        f.OnDeactivated.Invoke();
        f.isActive = false;
        return;
    }

    void SetFindableActive(Findable f)
    {
        if (f.isActive)
            return;

        f.OnActivated.Invoke();
        f.isActive = true;
        return;
    }

    void BlockAllByName(string name)
    {
        foreach (Findable f in harmlessFindable)
        {
            if (f.name == name)
                f.isBlocked = true;
        }

        foreach (Findable f in harmfulFindable)
        {
            if (f.name == name)
                f.isBlocked = true;
        }

        foreach (Findable f in emergencies)
        {
            if (f.name == name)
                f.isBlocked = true;
        }
    }

    void UnblockAll()
    {
        foreach (Findable f in harmlessFindable)
            f.isBlocked = false;

        foreach (Findable f in harmfulFindable)
            f.isBlocked = false;

        foreach (Findable f in emergencies)
            f.isBlocked = false;
    }

    public void SetAllFindablesInactive()
    {
        foreach (Findable f in harmlessFindable)
            SetFindableInactive(f);

        foreach (Findable f in harmfulFindable)
            SetFindableInactive(f);
    }

    public void SetRandomFindablesActive()
    {
        SetAllFindablesInactive();
        UnblockAll();

        if (maxActivated > harmfulFindable.Length)
            return;

        List<Findable> pool = new List<Findable>(harmfulFindable);
        List<string> clipboardStrings = new List<string>();
        int selected = 0;
        int selectCount = Random.Range(1, maxActivated);

        Debug.Log("num harmful: " + selectCount);
        while(selected < selectCount && pool.Count > 0)
        {
            int selectIndex = Random.Range(0, pool.Count);
            Findable f = pool[selectIndex];

            if (!f.isBlocked)
            {
                selected++;
                SetFindableActive(f);
                WriteFindable(f, true, true);
                foreach (string s in f.blocks)
                    BlockAllByName(s);
                clipboardStrings.Add(f.name);
            }
            else
                Debug.Log(f.name + " was blocked");
            pool.RemoveAt(selectIndex);
        }

        clipboard.NewList(clipboardStrings);

        pool = new List<Findable>(harmlessFindable);
        selected = 0;
        selectCount = Random.Range(1, harmlessFindable.Length - 3);

        Debug.Log("num harmless: + " + selectCount);
        while(selected < selectCount && pool.Count > 0)
        {
            int selectIndex = Random.Range(0, pool.Count);
            Findable f = pool[selectIndex];

            if (!f.isBlocked)
            {
                selected++;
                SetFindableActive(f);
                WriteFindable(f, false, true);
                foreach (string s in f.blocks)
                    BlockAllByName(s);
            }
            else
                Debug.Log(f.name + " was blocked");
            pool.RemoveAt(selectIndex);
        }

        foreach(Findable f in emergencies)
        {
            if (!f.isBlocked)
                SetFindableActive(f);
        }
    }

    void WriteFindable(Findable f, bool harmful, bool active)
    {
        if(harmful && active)
        {
            Debug.Log("Harmful State \"" + f.name + "\" set active");
        }
        else if(harmful && !active)
        {
            Debug.Log("Harmful State \"" + f.name + "\" set inactive");
        }
        else if(!harmful && active)
        {
            Debug.Log("Harmless State \"" + f.name + "\" set active");
        }
        else
        {
            Debug.Log("Harmless State \"" + f.name + "\" set inactive");
        }
    }

    int IndexOf(Findable[] arr, string item)
    {
        for(int i = 0; i < arr.Length; i++)
        {
            if (item == arr[i].name)
                return i;
        }
        return -1;
    }

    Findable[] RemoveFromArray(Findable[] arr, int index)
    {
        if (index < 0 || index >= arr.Length)
            return arr;

        Findable[] newArr = new Findable[arr.Length - 1];
        for(int i = 0; i < newArr.Length; i++)
        {
            if (i < index)
                newArr[i] = arr[i];
            else
                newArr[i] = arr[i + 1];
        }
        return newArr;
    }

    public void HospitalDeleteIndex()
    {
        int index;
        switch(source)
        {
            case ArrayStr.Harmless:
                index = IndexOf(harmlessFindable, removeName);
                harmlessFindable = RemoveFromArray(harmlessFindable, index);
                break;
            case ArrayStr.Harmful:
                index = IndexOf(harmfulFindable, removeName);
                harmfulFindable = RemoveFromArray(harmfulFindable, index);
                break;
            case ArrayStr.Emergencies:
                index = IndexOf(emergencies, removeName);
                emergencies = RemoveFromArray(emergencies, index);
                break;
            default:
                break;
        }
    }

    public void AddGoal(string goal)
    {
        goals.Add(goal);
    }

    public void CompleteGoal(string goal)
    {
		if(goal == "Toxic Emergency" || goal == "Oil Emergency" || goal == "Fire Emergency")
			FoundEmergencies++;
        completedGoals.Add(goal);
    }

    public void CompleteMission()
    {
        List<string> remainingGoals = new List<string>(goals);
        foreach (string s in completedGoals)
            remainingGoals.Remove(s);
        foreach (string s in remainingGoals)
        {
            if (s == "Radio Check")
                OnRadioFailed.Invoke();
            else if (s == "Clipboard Complete")
                OnClipboardFailed.Invoke();
            else if (s == "Toxic Emergency")
                OnToxicFailed.Invoke();
            else if (s == "Oil Emergency")
                OnOilFailed.Invoke();
            else if (s == "Fire Emergency")
                OnFireFailed.Invoke();
        }
        if (remainingGoals.Count == 0)
            OnMissionSuccess.Invoke();
        else
            OnMissionFailure.Invoke();
    }
}
