using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CutsceneManager : MonoBehaviour {

    public Text currentSubtext;
    public AudioSource instructionAudioSource;
    public Transform cutsceneCamera;
    public Transform headCamera;
    public InteractionHandler interactionHandler;
    [Tooltip("Objects that will be enabled during any cutscene")]
    public List<GameObject> genericEnableObjects;
    [Tooltip("Scripts that will be enabled during any cutscene")]
    public List<MonoBehaviour> genericEnableScripts;
    [Tooltip("Objects that will be disabled during any cutscene")]
    public List<GameObject> genericDisableObjects;
    [Tooltip("Scripts that will be disabled during any cutscene")]
    public List<MonoBehaviour> genericDisableScripts;
    public List<Cutscene> cutscenes;

    private string activeName;
    private Coroutine activeCutscene;
    private Dictionary<string, bool> consumedScenes;

    #region UNITY_CALLBACKS

    void Awake()
    {
        if(currentSubtext == null)
        {
            Debug.LogError("Missing CurrentSubtext reference, disabling script.");
            this.enabled = false;
        }
        if (instructionAudioSource == null)
        {
            Debug.LogError("Missing CutsceneAudioSource reference, disabling script.");
            this.enabled = false;
        }
        if (cutsceneCamera == null)
        {
            Debug.LogError("Missing CutsceneCamera reference, disabling script.");
            this.enabled = false;
        }
        else
            cutsceneCamera.gameObject.SetActive(false);
        if (interactionHandler == null)
        {
            Debug.LogError("Missing InteractionHandler reference, disabling script.");
            this.enabled = false;
        }
        if(headCamera == null)
        {
            Debug.LogError("Missing HeadCamera reference, disabling script.");
            this.enabled = false;
        }
    }

    void Start()
    {
        consumedScenes = new Dictionary<string, bool>();
        foreach (Cutscene cut in cutscenes)
        {
            consumedScenes.Add(cut.name, false);
            float sumPreviousLengths = 0.0f;
            for(int i = 0; i < cut.audioClips.Count; i++)
            {
                CutsceneAudioClip clip = cut.audioClips[i];
                clip.subtitles = clip.subtitles.OrderBy(elem => elem.timestamp).ToList();
                clip.StartTime = sumPreviousLengths + clip.preTime;
                clip.EndTime = clip.StartTime + clip.GetClipTime();
                sumPreviousLengths += clip.GetTotalTime();

                for (int j = 0; j < clip.subtitles.Count; j++)
                    clip.subtitles[j].StartTime = clip.StartTime + clip.subtitles[j].timestamp;
            }
            cut.cameraTransitions = cut.cameraTransitions.OrderBy(elem => elem.startTime).ToList();
        }
        PlayCutscene("Test");
    }
     
    #endregion

    #region MANAGER_CLASSES

    [System.Serializable]
	public class Cutscene
    {
        public string name;
        public List<CutsceneAudioClip> audioClips;
        public List<CutsceneTransition> cameraTransitions;
        public InteractionHandler.InvokableState OnBegin;
        public InteractionHandler.InvokableState OnEnd;

        public float GetTotalTime()
        {
            // Setup
            float audioTime = 0.0f;
            float transitionTime = 0.0f;
            float maxTransEnd = 0.0f;
            float minTransStart = float.PositiveInfinity;
            
            // Get total time spent during audio clips
            foreach (CutsceneAudioClip clip in audioClips)
                audioTime += clip.GetTotalTime();

            // Get total time spent during camera transitions
            foreach (CutsceneTransition transition in cameraTransitions)
            {
                if (transition.startTime < minTransStart && transition.startTime >= 0.0f)
                    minTransStart = transition.startTime;
                if (transition.endTime > maxTransEnd)
                    maxTransEnd = transition.endTime;
            }

            // return value based on presence of data
            if (audioClips.Count > 0 && cameraTransitions.Count > 0)
                return Mathf.Max(audioTime, maxTransEnd - minTransStart);
            else if (audioClips.Count > 0)
                return audioTime;
            else if (cameraTransitions.Count > 0)
                return maxTransEnd - minTransStart;
            else
                return 0.0f;
        }
    }

    [System.Serializable]
    public class CutsceneAudioClip
    {
        public AudioClip clip;
        public float preTime;
        public float postTime = 1.0f;
        public float overrideClipTime = 0.0f;
        public List<CutsceneTimedSubtitle> subtitles;
        public InteractionHandler.InvokableState OnBegin;
        public InteractionHandler.InvokableState OnEnd;

        public float StartTime { get; set; }
        public float EndTime { get; set; }

        public float GetTotalTime()
        {
                return preTime + postTime + GetClipTime();
        }

        public float GetClipTime()
        {
            if (clip == null || overrideClipTime != 0.0f)
                return overrideClipTime;
            else
                return clip.length;
        }
    }

    [System.Serializable]
    public class CutsceneTimedSubtitle
    {
        public string subtitle;
        [Tooltip("Time relative to 0.0 within clip")]
        public float timestamp;
        
        public float StartTime { get; set; }
    }

    [System.Serializable]
    public class CutsceneTransition
    {
        public Transform from;
        public Transform to;
        [Tooltip("Time relative to 0.0 within total cutscene")]
        public float startTime;
        [Tooltip("Time relative to 0.0 within total cutscene")]
        public float endTime;
        public AnimationCurve curve;
        public InteractionHandler.InvokableState OnBegin;
        public InteractionHandler.InvokableState OnEnd;
    }

    #endregion

    #region MANAGER_FUNCTIONS

    public void MatchCamera(Transform point)
    {
        cutsceneCamera.position = point.transform.position;
        cutsceneCamera.rotation = point.transform.rotation;
    }

    void FreezePlayer(bool isFrozen)
    {
        interactionHandler.PlayerCanCursorOver = !isFrozen;
        interactionHandler.PlayerCanInteract = !isFrozen;
        interactionHandler.PlayerCanLook = !isFrozen;
        interactionHandler.PlayerCanWalk = !isFrozen;
    }

    public void PlayCutscene(string name)
    {
        if(this.enabled == false)
        {
            Debug.LogError("Unable to play cutscene, manager is disabled!");
            return;
        }
        if (activeCutscene != null)
        {
            Debug.LogWarning("Cutscene -" + name + "- blocked due to -" + activeName + "- currently playing.");
            return;
        }
        Cutscene[] matches = cutscenes.Where(elem => elem.name == name).ToArray();
        if(matches.Length > 0)
        {
            bool isConsumed;
            consumedScenes.TryGetValue(matches[0].name, out isConsumed);
            if (!isConsumed)
            {
                activeName = matches[0].name;
                activeCutscene = StartCoroutine(CPlayCoroutine(matches[0]));
            }
        }
    }

    public float GetRelativeTime(float start, float end, float current)
    {
        current -= start;
        end -= start;
        return current / end;
    }

    #endregion

    #region MANAGER_COROUTINES

    IEnumerator CPlayCoroutine(Cutscene cut)
    {
        // Announce Cutscene Beginning
        Debug.Log("Beginning cutscene -" + cut.name + "-");

        // Prevents clips with no data from being executed
        bool useClips = cut.audioClips.Count > 0;
        bool useTransitions = cut.cameraTransitions.Count > 0;
        if(!useClips && !useTransitions)
        {
            Debug.LogWarning("Cutscene contains no clips or transitions, ending cutscene.");
            yield break;
        }

        // Freeze Player
        FreezePlayer(true);

        // Unity Events
        cut.OnBegin.Invoke();

        // Enable Cutscene Camera
        if (useTransitions)
            MatchCamera(cut.cameraTransitions[0].from);
        else
            MatchCamera(headCamera);
        cutsceneCamera.gameObject.SetActive(true);
        headCamera.gameObject.SetActive(false);

        // Setup Generics
        foreach (GameObject go in genericDisableObjects)
            go.SetActive(false);
        foreach (MonoBehaviour mb in genericDisableScripts)
            mb.enabled = false;
        foreach (GameObject go in genericEnableObjects)
            go.SetActive(true);
        foreach (MonoBehaviour mb in genericEnableScripts)
            mb.enabled = true;

        // Setup flexible variables
        int currentClip = -1;
        int currentTransition = -1;
        float maxTime = cut.GetTotalTime();

        // Ease of use variables
        CutsceneAudioClip clip = null;
        CutsceneTransition transition = null;
        int currentSubtitle = -1;

        // Save old dialogue
        string oldDialogue = currentSubtext.text;

        // Main Loop
        for(float t = 0.0f; t < maxTime; t+=Time.deltaTime)
        {
            if(useClips)
            {
                // Monitor end of current clip
                if(clip != null)
                {
                    if(clip.EndTime < t)
                    {
                        clip.OnEnd.Invoke();
                        clip = null;
                    }
                }

                // Monitor start of new clip
                if(clip == null)
                {
                    int next = currentClip + 1;
                    if(next < cut.audioClips.Count && cut.audioClips[next].StartTime < t)
                    {
                        currentClip = next;
                        currentSubtitle = -1;
                        clip = cut.audioClips[currentClip];
                        clip.OnBegin.Invoke();
                        instructionAudioSource.PlayOneShot(clip.clip);
                        instructionAudioSource.time = t - clip.StartTime;
                    }
                }

                // Update Subtitles
                if(clip != null && clip.subtitles.Count > 0)
                {
                    int nextSubtitle = currentSubtitle + 1;
                    if(clip.subtitles.Count > nextSubtitle)
                    {
                        if (clip.subtitles[nextSubtitle].StartTime < t)
                        {
                            currentSubtitle = nextSubtitle;
                            currentSubtext.text = clip.subtitles[currentSubtitle].subtitle;
                        }
                    }
                }
                else
                {
                    currentSubtext.text = "";
                }
            }
            if(useTransitions)
            {
                // Monitor end of current transition
                if(transition != null)
                {
                    if(transition.endTime < t)
                    {
                        MatchCamera(transition.to);
                        transition.OnEnd.Invoke();
                        transition = null;
                    }
                }

                // Monitor start of new transition
                if(transition == null)
                {
                    int next = currentTransition + 1;
                    if(cut.cameraTransitions.Count > next)
                    {
                        if(cut.cameraTransitions[next].startTime < t)
                        {
                            transition = cut.cameraTransitions[next];
                            currentTransition = next;
                            if(transition.curve == null || transition.curve.length == 0)
                            {
                                transition.curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
                            }
                            transition.OnBegin.Invoke();
                        }
                    }
                }

                // Place camera
                if(transition != null)
                {
                    float curveTime = GetRelativeTime(transition.startTime, transition.endTime, t);
                    float curveValue = transition.curve.Evaluate(curveTime);
                    Vector3 position = Vector3.Lerp(transition.from.position, transition.to.position, curveValue);
                    Quaternion rotation = Quaternion.Lerp(transition.from.rotation, transition.to.rotation, curveValue);
                    cutsceneCamera.position = position;
                    cutsceneCamera.rotation = rotation;
                }
            }
            yield return null;
        }

        // Finalize Generics
        foreach (GameObject go in genericDisableObjects)
            go.SetActive(true);
        foreach (MonoBehaviour mb in genericDisableScripts)
            mb.enabled = true;
        foreach (GameObject go in genericEnableObjects)
            go.SetActive(false);
        foreach (MonoBehaviour mb in genericEnableScripts)
            mb.enabled = false;

        // Switch back to head camera
        headCamera.gameObject.SetActive(true);
        cutsceneCamera.gameObject.SetActive(false);

        // Replace text
        currentSubtext.text = oldDialogue;

        // Unity Events
        cut.OnEnd.Invoke();

        // Mark cutscene as consumed
        consumedScenes[cut.name] = true;

        // Let player move
        FreezePlayer(false);

        // Stop blocking of other cutscenes
        activeName = null;
        activeCutscene = null;
    }

    #endregion
}
