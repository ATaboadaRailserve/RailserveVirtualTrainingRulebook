using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillBoxIndicator : MonoBehaviour {

    //public KillBox killbox;
    public Text uiText;

    KillBox[] killboxes;

    void Awake()
    {
        if(uiText == null)
        {
            this.enabled = false;
            this.gameObject.SetActive(false);
            return;
        }

        killboxes = FindObjectsOfType<KillBox>();
        foreach(KillBox killbox in killboxes)
        {
            killbox.OnSafe.AddInitialListener(OnSafe);
            killbox.OnWarn.AddInitialListener(OnWarn);
            killbox.OnKill.AddInitialListener(OnKill);
        }
        gameObject.SetActive(false);
    }

    void Update()
    {
        int lowest = int.MaxValue;
        foreach(KillBox killbox in killboxes)
        {
            if (killbox.CurrentState == KillBox.STATE.WARNING)
            {
                lowest = Mathf.Min(lowest, (int)killbox.CurrentWarningTime);
                uiText.text = (lowest + 1).ToString();
            }
        }
    }

    void OnSafe()
    {
        gameObject.SetActive(false);
    }

    void OnWarn()
    {
        gameObject.SetActive(true);
    }

    void OnKill()
    {
        gameObject.SetActive(false);
    }
}