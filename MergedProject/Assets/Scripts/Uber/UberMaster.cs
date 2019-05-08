using UnityEngine;
using System.Collections.Generic;

public class UberMaster : MonoBehaviour
{
	public int CheckpointLives = 2;
	
    public InteractionHandler.InvokableState OnStart;
    public InteractionHandler.InvokableState OnFail;
    public InteractionHandler.InvokableState OnWin;
    public InteractionHandler.InvokableState OnEnd;

    public UberModule[] Modules;
    public string[] ModuleOrder;

    //public System.Collections.Generic.List<string> ServerMessages;

    private UberModule currentModule;
	private UberModule lastModuleCheckpoint;
	private int lastModuleCheckpointIndex;
    private int currentModuleIndex = 0;     // refers to module in moduleOrder, ie current step in game
	private Stack<UberModule> unsavedProgress;
	
    // Found References
    private DatabaseMessageHolder messageHolder;
    private InteractionHandler interactionHandler;

    #region Unity_Callbacks
    // herp
    void Awake()
    {
        foreach(UberModule m in Modules)
        {
            m.SetMaster(this);
        }

        interactionHandler = GameObject.FindObjectOfType<InteractionHandler>();
        if (interactionHandler == null)
            Debug.LogWarning("No InteractionHandler found in scene");
    }

    // derp
    void Start()
    {
		unsavedProgress = new Stack<UberModule>();
		
        // Server messaging setup
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("MessageHolder"))
        {
            if (go.GetComponent<DataLoader>())
                messageHolder = go.GetComponent<DatabaseMessageHolder>();
        }
        
        if (!messageHolder && GameObject.FindWithTag("MessageHolder"))
            messageHolder = GameObject.FindWithTag("MessageHolder").GetComponent<DatabaseMessageHolder>();
        
        if (messageHolder != null)
        {
            messageHolder.WriteMessage("Uber Started");
        }
        else
            Debug.LogWarning("MessageHolder not found in scene");

        if (ModuleOrder.Length > 0)
        {
            UberModule m = FindModule(ModuleOrder[0]);
            if (m != null)
                SwitchModule(null, m);
        }
        else
        {
            Debug.LogWarning("No modules listed");
        }
		OnStart.Invoke();
        print("UBER GAME STARTED");
    }
    #endregion

    #region Public_Functions
    // Should be called when a module is completed
    public void Next()
    {
        currentModuleIndex++;
        if (currentModuleIndex >= ModuleOrder.Length)
        {
            SwitchModule(currentModule, null);
            Win();
        }
        else
        {
            UberModule m = FindModule(ModuleOrder[currentModuleIndex]);
            if (m != null)
                SwitchModule(currentModule, m);
        }
    }

    // Should be called when a module is failed
    public void Fail()
    {
		if(CheckpointLives > 0)
		{
			CheckpointLives--;
			PushServerMessage("uber game resuming at last checkpoint");
			ResetToLastCheckpoint();
		}
		else
		{
			PushServerMessage("uber game failed");
			OnFail.Invoke();
			interactionHandler.Lose();
			OnEnd.Invoke();
		}
    }
	
	public void SetCheckpoint(UberModule um)
	{
		unsavedProgress.Clear();
		lastModuleCheckpoint = um;
		lastModuleCheckpointIndex = FindModuleIndex(um.ModuleName);
	}
	
	public void AddUnsavedProgress(UberModule um)
	{
		Debug.Log("Saving progress: " + um.ModuleName);
		for (int i = 0; i < Modules.Length; i++) {
			if (Modules[i].ModuleName == um.ModuleName) {
				messageHolder.WriteMessage (i.ToString(), 8, true);
			}
		}
		unsavedProgress.Push(um);
	}
	
	public void ResetToLastCheckpoint()
	{
		// clear progress on all unsaved states
		while(unsavedProgress.Count > 0)
		{
			unsavedProgress.Pop().ClearForCheckpoint();
		}
		
		// Restart last saved state
		if(lastModuleCheckpoint != null)
		{
			lastModuleCheckpoint.ResetToLastCheckpoint();
		
			currentModule = lastModuleCheckpoint;
			currentModuleIndex = lastModuleCheckpointIndex;
		}
		
		PushServerMessage("Reset to Last Checkpoint: Complete");
	}

    // Using for pushing messages within the master, or from an arbitrary location
    private void PushServerMessage(string message)
    {
        string text = "master: " + message;
        //ServerMessages.Add(text);
        messageHolder.WriteMessage(text);
        Debug.Log(text);
    }

    // Used for pushing messages from a module
    public void PushServerMessage(UberModule fromModule, string message)
    {
        string text = fromModule.ModuleName + ": " + message;
        //ServerMessages.Add(text);
        messageHolder.WriteMessage(text);
        Debug.Log(text);
    }

    public void PushServerMessage(string source, string message)
    {
        string text = source + ": " + message;
        //ServerMessages.Add(text);
        messageHolder.WriteMessage(text);
        Debug.Log(text);
    }
    #endregion

    #region Private_Functions
    // Automatically called when Next() is called and there are no more modules
    void Win()
    {
        PushServerMessage("uber game completed");
        OnWin.Invoke();
        interactionHandler.Win();
        OnEnd.Invoke();
    }

    // Used to change modules
    void SwitchModule(UberModule oldModule, UberModule newModule)
    {
        if (oldModule != null)
            oldModule.EndModule();
        if(newModule != null)
        {
            newModule.StartModule();
            currentModule = newModule;
        }
    }

    // Used to grab modules by name
    UberModule FindModule(string name)
    {
        foreach(UberModule m in Modules)
        {
            if (m.ModuleName == name)
                return m;
        }
        Debug.Log("module name ::" + name + ":: not found");
        return null;
    }
	
	int FindModuleIndex(string name)
	{
		for(int i = 0; i < ModuleOrder.Length; i++)
		{
			if(ModuleOrder[i] == name)
				return i;
		}
		Debug.Log("module name ::" + name + ":: not found");
		return -1;
	}

    void SendServerMessages()
    {
        messageHolder.PushingMessages();
    }
    #endregion
}
