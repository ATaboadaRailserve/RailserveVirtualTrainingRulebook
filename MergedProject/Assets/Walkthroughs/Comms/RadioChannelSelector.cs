using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadioChannelSelector : MonoBehaviour {

    public int currentChannel;
    public AudioSource audioSource;
    public Text noticeText;
    public Text channelText;
    public List<Channel> channels;
    public InteractionHandler.InvokableState OnAccepted;
    public InteractionHandler.InvokableState OnRevoked;
    public InteractionHandler.InvokableState OnComplete;

    [System.Serializable]
    public class Channel
    {
        public AudioClip clip;
        public string noticeText;
        public bool openChannel;
    }

    void Start()
    {
        PlayChannel();
    }

	public void GoUp()
    {
        currentChannel++;
        if (currentChannel >= channels.Count)
            currentChannel = 0;
        PlayChannel();
    }

    public void GoDown()
    {
        currentChannel--;
        if (currentChannel < 0)
            currentChannel = channels.Count - 1;
        PlayChannel();
    }

    public void CheckIfOpen()
    {
        if (channels.Count > 0 && currentChannel >= 0 && currentChannel < channels.Count)
        {
            noticeText.text = channels[currentChannel].noticeText;
            if (channels[currentChannel].openChannel)
                OnAccepted.Invoke();
            else
                OnRevoked.Invoke();
            OnComplete.Invoke();
        }
    }

    void PlayChannel()
    {
        if(channels.Count > 0 && currentChannel >= 0 && currentChannel < channels.Count)
        {
            channelText.text = (currentChannel + 1).ToString();
            if(audioSource != null && channels[currentChannel].clip != null)
            {
                audioSource.PlayOneShot(channels[currentChannel].clip);
            }
        }
    }
}
