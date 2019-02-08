using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClipboardList : MonoBehaviour {

    public bool toggleAlwaysCompletes = true;
    public DISPLAY_MODE displayMode = DISPLAY_MODE.SHOW_BOXES;
    public List<ClipboardListElement> elements;
    public InteractionHandler.InvokableState OnToggleElement;
    public InteractionHandler.InvokableState OnAllCompleted;

    private int numElements;
    private GameObject uiListElement;
    private string[] hiddenText;
    private bool initialized = false;
	private bool isComplete = false;
	
	public int NumElements {get; private set;}
	public int NumCompleted {get; private set;}
	
    [System.Serializable]
    public class ClipboardListElement
    {
        public bool isChecked;
        public string text;
    }

    public enum DISPLAY_MODE
    {
        SHOW_ALL,
        SHOW_CHECKED,
        SHOW_BOXES
    }

    void Initialize()
    {
        if (initialized)
            return;

		isComplete = false;
        initialized = true;
        numElements = elements.Count;
		NumElements = numElements;

        // Create hidden text array for SHOW_BOXES display mode
        hiddenText = new string[numElements];
        for (int i = 0; i < numElements; i++)
            hiddenText[i] = elements[i].text;

		// Delete previous elements
		for(int i = transform.childCount - 1; i > 0; i--)
			Destroy(transform.GetChild(i).gameObject);
		
        // Create list elements
        uiListElement = transform.GetChild(0).gameObject;
        for (int i = 0; i < numElements - 1; i++)
        {
            Instantiate(uiListElement, this.transform);
        }

        // Update the display
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        if (!initialized)
            Initialize();
        int numCompleted = 0;
        for(int i = 0; i < numElements; i++)
        {
            if (elements[i].isChecked)
                numCompleted++;
            transform.GetChild(i).GetChild(0).GetChild(0).gameObject.SetActive(elements[i].isChecked);
            switch(displayMode)
            {
                case DISPLAY_MODE.SHOW_CHECKED:
                    if (elements[i].isChecked)
                        transform.GetChild(i).gameObject.SetActive(true);
                    else
                        transform.GetChild(i).gameObject.SetActive(false);
                    break;
                case DISPLAY_MODE.SHOW_BOXES:
                    if (elements[i].isChecked)
                        elements[i].text = hiddenText[i];
                    else
                        elements[i].text = "";
                    transform.GetChild(i).gameObject.SetActive(true);
                    break;
                default:
                    transform.GetChild(i).gameObject.SetActive(true);
                    break;
            }
            transform.GetChild(i).GetComponentInChildren<Text>().text = elements[i].text;
        }
		
		NumCompleted = numCompleted;
		
        if (numElements != 0 && numCompleted == numElements)
			isComplete = true;
    }

    public void ToggleElement(int index)
    {
        if (!initialized)
            Initialize();
        if (index < 0 || index >= numElements)
            return;

        if (toggleAlwaysCompletes)
            elements[index].isChecked = true;
        else
            elements[index].isChecked = !elements[index].isChecked;
        UpdateDisplay();
        OnToggleElement.Invoke();
		if(isComplete)
			OnAllCompleted.Invoke();
    }

    public void ToggleElement(string name)
    {
        if (!initialized)
            Initialize();
        foreach(ClipboardListElement e in elements)
        {
            if(e.text == name)
            {
                if (toggleAlwaysCompletes)
                    e.isChecked = true;
                else
                    e.isChecked = !e.isChecked;
            }
        }
        UpdateDisplay();
        OnToggleElement.Invoke();
		if(isComplete)
			OnAllCompleted.Invoke();
    }

    public void NewList(IEnumerable<string> items)
    {
        elements.Clear();
        foreach(string s in items)
        {
            ClipboardListElement newItem = new ClipboardListElement();
            newItem.isChecked = false;
            newItem.text = s;
            elements.Add(newItem);
        }
        initialized = false;
        Initialize();
    }
}
