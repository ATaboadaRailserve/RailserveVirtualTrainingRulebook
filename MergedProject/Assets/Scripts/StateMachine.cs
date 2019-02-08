using UnityEngine;

public class StateMachine : MonoBehaviour {

    public string machineName = "basic state machine";
    public int startState = 0;
    public State[] states = { new State("default") };
    public Transition[] defaultTransitions;
    
    public State CurrentState
    {
        get { return states[index]; }
        private set { states[index] = value; }
    }

    private int index;
    private bool initialized;

    [System.Serializable]
    public class State
    {
        public string name;
        public InteractionHandler.InvokableState OnStateBegin;
        public InteractionHandler.InvokableState OnStateEnd;

        public State()
        {
            this.name = "unassigned";
        }

        public State(string name)
        {
            this.name = name;
        }
    }

    [System.Serializable]
    public class Transition
    {
        public int start;
        public int end;
    }

    public void Start()
    {
        if(!initialized)
            Initialize();
    }

    private void Initialize()
    {
        if (states.Length == 0)
        {
            Debug.LogWarning("state machine: " + this.machineName + " contains no states, disabled.");
            this.enabled = false;
        }
        else
        {
            index = startState;
            CurrentState.OnStateBegin.Invoke();
        }
    }

    public void GoToState(int index)
    {
        if (!this.enabled && index < 0 || index >= states.Length)
            return;

        if (initialized)
            CurrentState.OnStateEnd.Invoke();
        else
            Initialize();
        this.index = index;
        CurrentState.OnStateBegin.Invoke();
    }

    public void GoToState(string name)
    {
        for(int i = 0; i < states.Length; i++)
        {
            if(states[i].name == name)
            {
                GoToState(i);
                return;
            }
        }
    }

    // Used to cycle through states assuming they are in an organized order
    public void LoopNext()
    {
        if (!this.enabled)
            return;
        int next = (index + 1) % states.Length;
        GoToState(next);
    }

    public void TransitionWithDefault()
    {
        if (!this.enabled)
            return;
        for(int i = 0; i < defaultTransitions.Length; i++)
        {
            if(this.index == defaultTransitions[i].start)
            {
                GoToState(defaultTransitions[i].end);
                return;
            }
        }
    }
}