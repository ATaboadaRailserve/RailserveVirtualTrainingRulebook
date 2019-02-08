using UnityEngine;
using System.Collections.Generic;

public class UberModule : MonoBehaviour
{
    public string ModuleName;
	public bool CheckpointOnStart;
    public InteractionHandler.InvokableState OnBegin;
    public InteractionHandler.InvokableState OnCompleteModule;
    public InteractionHandler.InvokableState OnFailModule;
    public InteractionHandler.InvokableState OnEnd;
	public InteractionHandler.InvokableState OnClearForCheckpoint;
	public InteractionHandler.InvokableState OnRestartAsCheckpoint;
	
    public UberSubmodule[] Submodules;
    public string[] SubmoduleOrder;

    protected UberMaster master;
    protected UberSubmodule currentSubmodule;
	protected UberSubmodule lastSubmoduleCheckpoint;
	protected int lastSubmoduleCheckpointIndex;
    protected int currentSubmoduleIndex = 0;
	protected Stack<UberSubmodule> unsavedProgress;

    public bool IsModuleActive { get; private set; }

    #region Automatics
    // Automatically called by UberMaster Awake()
    public void SetMaster(UberMaster master)
    {
        this.master = master;
        foreach (UberSubmodule s in Submodules)
            s.SetModule(this);
    }

    // Automatically called by UberMaster, overrideable
    public virtual void StartModule()
    {
		unsavedProgress = new Stack<UberSubmodule>();
        IsModuleActive = true;
        PushServerMessage("module started");
        OnBegin.Invoke();
		if(CheckpointOnStart)
			master.SetCheckpoint(this);
		else
			master.AddUnsavedProgress(this);
        if (SubmoduleOrder.Length > 0)
        {
            UberSubmodule sub = FindSubmodule(SubmoduleOrder[0]);
            if (sub != null)
                SwitchSubmodule(null, sub);
        }
    }

    // Automatically called by UberMaster, overrideable
    public virtual void EndModule()
    {
        IsModuleActive = false;
        PushServerMessage("module ended");
        OnEnd.Invoke();
    }
	
	public virtual void SetSubcheckpoint(UberSubmodule usm)
	{
		if(!CheckpointOnStart)
		{
			CheckpointOnStart = true;
			master.SetCheckpoint(this);
		}
		unsavedProgress.Clear();
		lastSubmoduleCheckpoint = usm;
		lastSubmoduleCheckpointIndex = FindSubmoduleIndex(usm.SubmoduleName);
	}
	
	public virtual void AddUnsavedProgress(UberSubmodule usm)
	{
		unsavedProgress.Push(usm);
	}
	
	public virtual void ResetToLastCheckpoint()
	{	
		if(lastSubmoduleCheckpoint != null)
		{
			while(unsavedProgress.Count > 0)
				unsavedProgress.Pop().ClearForCheckpoint();
			
			currentSubmodule = lastSubmoduleCheckpoint;
			currentSubmoduleIndex = lastSubmoduleCheckpointIndex;
			
			lastSubmoduleCheckpoint.ClearForCheckpoint();
			lastSubmoduleCheckpoint.RestartAsCheckpoint();
		}
		else
		{
			ClearForCheckpoint();
			RestartAsCheckpoint();
		}
	}
	
	// Automatically called by UberMaster, overridable
	public virtual void ClearForCheckpoint()
	{	
		while(unsavedProgress.Count > 0)
		{
			unsavedProgress.Pop().ClearForCheckpoint();
		}
		
		currentSubmodule = lastSubmoduleCheckpoint;
		currentSubmoduleIndex = lastSubmoduleCheckpointIndex;
		
		IsModuleActive = false;
		OnClearForCheckpoint.Invoke();
		PushServerMessage("module cleared for previous checkpoint");
	}
	
	// Automatically called by UberMaster, overridable
	public virtual void RestartAsCheckpoint()
	{
		StartModule();
		OnRestartAsCheckpoint.Invoke();
		PushServerMessage("module \'" + ModuleName + "\' started again as checkpoint");
	}
    #endregion

    #region Manuals
    public virtual void NextSubmodule()
    {
        currentSubmoduleIndex++;
        if (currentSubmoduleIndex >= SubmoduleOrder.Length)
        {
            SwitchSubmodule(currentSubmodule, null);
            CompleteModule();
        }
        else
        {
            UberSubmodule sub = FindSubmodule(SubmoduleOrder[currentSubmoduleIndex]);
            if (sub != null)
                SwitchSubmodule(currentSubmodule, sub);
        }
    }

    // Manually call this to complete a module, overrideable
    public virtual void CompleteModule()
    {
        PushServerMessage("module complete");
        OnCompleteModule.Invoke();
        master.Next();
    }

    // Manually call this to fail a module, overridable
    public virtual void FailModule()
    {
        PushServerMessage("module failed");
        OnFailModule.Invoke();
        master.Fail();
    }

    public void PushServerMessage(string s)
    {
        master.PushServerMessage(this, s);
    }
    #endregion

    #region Helper Functions
    private void SwitchSubmodule(UberSubmodule oldSubmodule, UberSubmodule newSubmodule)
    {
        if (oldSubmodule != null)
            oldSubmodule.EndSubmodule();
        if (newSubmodule != null)
        {
            newSubmodule.StartSubmodule();
            currentSubmodule = newSubmodule;
        }
    }

    private UberSubmodule FindSubmodule(string submoduleName)
    {
        foreach(UberSubmodule sub in Submodules)
        {
            if (sub.SubmoduleName == submoduleName)
                return sub;
        }
        Debug.Log("submodule name ::" + submoduleName + ":: not found");
        return null;
    }
	
	private int FindSubmoduleIndex(string submoduleName)
	{
		for(int i = 0; i < SubmoduleOrder.Length; i++)
		{
			if(SubmoduleOrder[i] == submoduleName)
				return i;
		}
		Debug.Log("submodule name ::" + name + ":: not found");
		return -1;
	}
    #endregion
}