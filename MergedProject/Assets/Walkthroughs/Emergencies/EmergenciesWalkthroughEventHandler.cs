using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergenciesWalkthroughEventHandler : MonoBehaviour
{

    public GameObject PlayerHeadCamera;
    public GameObject CutsceneCamera;
    public float CutSceneExitTime = 3.0f;
    public List<CutscenePoint> ResLimitPoints;
    public List<CutscenePoint> LineLimitPoints;
    public CutscenePoint leakingTankerPoint;
    public float generalDialogueTime = 3.0f;
    public TeleportTo tele;

    InteractionHandler interactionHandler;
    PlayerController playerController;
    BasicQuester quester;
    bool ResLimitCompleted;
    bool LineLimitCompleted;
    bool leakingCompleted;
    bool emergencyAvailable;
    bool emergencyCallUsed;

    [System.Serializable]
    public struct CutscenePoint
    {
        public Transform point;
        public float transitionTime;
        public float stayTime;
    }

    void Awake()
    {
        interactionHandler = GameObject.FindObjectOfType<InteractionHandler>();
        playerController = GameObject.FindObjectOfType<PlayerController>();
        quester = GameObject.FindObjectOfType<BasicQuester>();
    }

    void MatchCamera()
    {
        CutsceneCamera.transform.position = PlayerHeadCamera.transform.position;
        CutsceneCamera.transform.rotation = PlayerHeadCamera.transform.rotation;
    }

    void FreezePlayer(bool isFrozen)
    {
        interactionHandler.PlayerCanCursorOver = !isFrozen;
        interactionHandler.PlayerCanInteract = !isFrozen;
        interactionHandler.PlayerCanLook = !isFrozen;
        interactionHandler.PlayerCanWalk = !isFrozen;
    }

    public void DoResLimitDialogue()
    {
        if (!ResLimitCompleted)
            StartCoroutine(C_ResLimit());
        ResLimitCompleted = true;
    }

    IEnumerator C_ResLimit()
    {
        MatchCamera();
        Vector3 startPos = CutsceneCamera.transform.position;
        Quaternion startRot = CutsceneCamera.transform.rotation;
        Vector3 currentPos;
        Quaternion currentRot;
        PlayerHeadCamera.SetActive(false);
        CutsceneCamera.SetActive(true);
        FreezePlayer(true);
        for (int i = 0; i < ResLimitPoints.Count; i++)
        {
            quester.Next();
            currentPos = CutsceneCamera.transform.position;
            currentRot = CutsceneCamera.transform.rotation;
            for (float t = 0.0f; t < ResLimitPoints[i].transitionTime; t += Time.deltaTime)
            {
                CutsceneCamera.transform.position = Vector3.Slerp(currentPos, ResLimitPoints[i].point.position, t / ResLimitPoints[i].transitionTime);
                CutsceneCamera.transform.rotation = Quaternion.Slerp(currentRot, ResLimitPoints[i].point.rotation, t / ResLimitPoints[i].transitionTime);
                yield return null;
            }
            CutsceneCamera.transform.position = ResLimitPoints[i].point.position;
            CutsceneCamera.transform.rotation = ResLimitPoints[i].point.rotation;

            for (float t = 0.0f; t < ResLimitPoints[i].stayTime; t += Time.deltaTime)
            {
                yield return null;
            }
        }

        currentPos = CutsceneCamera.transform.position;
        currentRot = CutsceneCamera.transform.rotation;
        for (float t = 0.0f; t < CutSceneExitTime; t += Time.deltaTime)
        {
            CutsceneCamera.transform.position = Vector3.Slerp(currentPos, startPos, t / CutSceneExitTime);
            CutsceneCamera.transform.rotation = Quaternion.Slerp(currentRot, startRot, t / CutSceneExitTime);
            yield return null;
        }

        CutsceneCamera.SetActive(false);
        PlayerHeadCamera.SetActive(true);
        FreezePlayer(false);
        quester.Next();
    }

    public void DoLineLimitDialogue()
    {
        if (!LineLimitCompleted)
            StartCoroutine(C_LineLimit());
        LineLimitCompleted = true;
    }

    IEnumerator C_LineLimit()
    {
        yield return null;
    }

    public void DoLeakingTankerDialogue()
    {
        if (!leakingCompleted)
            StartCoroutine(C_LeakingCutscene());
        leakingCompleted = true;
    }

    IEnumerator C_LeakingCutscene()
    {
        MatchCamera();
        Vector3 startPos = CutsceneCamera.transform.position;
        Quaternion startRot = CutsceneCamera.transform.rotation;
        PlayerHeadCamera.SetActive(false);
        CutsceneCamera.SetActive(true);
        FreezePlayer(true);
        quester.Next();

        for (float t = 0.0f; t < leakingTankerPoint.transitionTime; t += Time.deltaTime)
        {
            CutsceneCamera.transform.position = Vector3.Slerp(startPos, leakingTankerPoint.point.position, t / leakingTankerPoint.transitionTime);
            CutsceneCamera.transform.rotation = Quaternion.Slerp(startRot, leakingTankerPoint.point.rotation, t / leakingTankerPoint.transitionTime);
            yield return null;
        }
        CutsceneCamera.transform.position = leakingTankerPoint.point.position;
        CutsceneCamera.transform.rotation = leakingTankerPoint.point.rotation;

        quester.Next();

        for (float t = 0.0f; t < leakingTankerPoint.stayTime; t += Time.deltaTime)
            yield return null;

        Vector3 currentPos = CutsceneCamera.transform.position;
        Quaternion currentRot = CutsceneCamera.transform.rotation;
        for (float t = 0.0f; t < CutSceneExitTime; t += Time.deltaTime)
        {
            CutsceneCamera.transform.position = Vector3.Slerp(currentPos, startPos, t / CutSceneExitTime);
            CutsceneCamera.transform.rotation = Quaternion.Slerp(currentRot, startRot, t / CutSceneExitTime);
            yield return null;
        }

        CutsceneCamera.SetActive(false);
        PlayerHeadCamera.SetActive(true);
        FreezePlayer(false);
        emergencyAvailable = true;
    }

    public void TryEmergencyCall()
    {
        if (emergencyAvailable && !emergencyCallUsed)
        {
            StartCoroutine(C_EmergencyCall());
            emergencyCallUsed = true;
        }
    }

    IEnumerator C_EmergencyCall()
    {
        for(int i = 0; i < 4; i++)
        {
            quester.Next();
            for (float t = 0.0f; t < generalDialogueTime; t += Time.deltaTime)
                yield return null;
        }
        quester.Next();
        tele.Teleport();
    }
}
