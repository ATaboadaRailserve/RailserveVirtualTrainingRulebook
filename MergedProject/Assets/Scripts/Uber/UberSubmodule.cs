using UnityEngine;

public class UberSubmodule : MonoBehaviour
{
    public string SubmoduleName;
	public bool CheckpointOnStart;
    public InteractionHandler.InvokableState OnBegin;
    public InteractionHandler.InvokableState OnCompleteSubmodule;
    public InteractionHandler.InvokableState OnFailSubmodule;
    public InteractionHandler.InvokableState OnEnd;
	public InteractionHandler.InvokableState OnClearForCheckpoint;
	public InteractionHandler.InvokableState OnRestartAsCheckpoint;

    protected UberModule module;

    public bool IsSubmoduleActive { get; private set; }

    #region Automatics
    public void SetModule(UberModule module)
    {
        this.module = module;
    }

    public virtual void StartSubmodule()
    {
        IsSubmoduleActive = true;
        PushServerMessage("submodule started");
        OnBegin.Invoke();
		if(CheckpointOnStart)
			module.SetSubcheckpoint(this);
		else
			module.AddUnsavedProgress(this);
    }

    public virtual void EndSubmodule()
    {
        IsSubmoduleActive = false;
        PushServerMessage("submodule ended");
        OnEnd.Invoke();
    }
	
	public virtual void ClearForCheckpoint()
	{
		OnClearForCheckpoint.Invoke();
		PushServerMessage("module cleared for previous checkpoint");
	}
	
	public virtual void RestartAsCheckpoint()
	{
		StartSubmodule();
		OnRestartAsCheckpoint.Invoke();
		PushServerMessage("module \'" + SubmoduleName + "\' started again as checkpoint");
	}
	
    #endregion

    #region Manuals
    public virtual void CompleteSubmodule()
    {
        PushServerMessage("submodule completed");
        OnCompleteSubmodule.Invoke();
        module.NextSubmodule();
    }

    public virtual void FailSubmodule()
    {
        PushServerMessage("submodule failed");
        OnFailSubmodule.Invoke();
        module.FailModule();
    }

    public void PushServerMessage(string message)
    {
        module.PushServerMessage(SubmoduleName + "> " + message);
    }
    #endregion
}
