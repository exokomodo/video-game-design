using UnityEngine;
using Cinemachine;
using System.Collections.Generic;
using System;

public struct RecenterSettings {
    public bool enabled;
    public float recenterTime;
    public float waitTime;
    public CinemachineInputProvider inputProvider;

    public RecenterSettings(bool enabled, float recenterTime, float waitTime, CinemachineInputProvider inputProvider) {
        this.enabled = enabled;
        this.recenterTime = recenterTime;
        this.waitTime = waitTime;
        this.inputProvider = inputProvider;
    }
}

/// <summary>
/// A Camera Controller that provides immediate snap-to-target-center capability
/// triggered by user input across a number of state-driven virtual cameras
/// Author: Geoffrey Roth
/// </summary>
public class CameraController : MonoBehaviour {

    [SerializeField]
    private InputReader input;
    private CinemachineBrain brain;
    private CinemachineStateDrivenCamera sdc;
    private CinemachineFreeLook current;
    private Dictionary<int, RecenterSettings> cameraSettings;
    private bool recentering;
    private bool inited = false;
    private bool hasFreeLook = false;
    private float easing = 10;
    private bool isDialogue = false;


    private void Start() {
        EventManager.StartListening<DialogueOpenEvent, Vector3, string>(OnDialogOpen);
        EventManager.StartListening<DialogueCloseEvent, string>(OnDialogClose);
    }

    private void OnDialogOpen(Vector3 position, string dialogueName) {
        isDialogue = true;
        ToggleEnabled(false);
        Invoke("Recenter", 0.05f);
    }

    private void OnDialogClose(string dialogueName) {
        isDialogue = false;
        ToggleEnabled(true);
    }

    public void ToggleEnabled(bool b) {
        foreach (ICinemachineCamera cam in sdc.ChildCameras) {
            if (cam.GetType() == typeof(CinemachineFreeLook)) {
                CinemachineFreeLook freeLook = cam as CinemachineFreeLook;
                RecenterSettings s = cameraSettings[freeLook.GetHashCode()];
                s.inputProvider.enabled = b;
            }
        }
    }

    private void Init() {
        brain = GetComponent<CinemachineBrain>();
        if (brain == null) throw new Exception("CinemachineBrain component is required but was not found. Exiting...");

        ICinemachineCamera c = brain.ActiveVirtualCamera;
        sdc = c as CinemachineStateDrivenCamera;
        cameraSettings = new Dictionary<int, RecenterSettings>();
        // Store the initial camera settings so we can reset them later
        foreach (ICinemachineCamera cam in sdc.ChildCameras) {
            if (cam.GetType() == typeof(CinemachineFreeLook)) {
                hasFreeLook = true;
                CinemachineFreeLook freeLook = cam as CinemachineFreeLook;
                cameraSettings[cam.GetHashCode()] = new RecenterSettings(
                    freeLook.m_RecenterToTargetHeading.m_enabled,
                    freeLook.m_RecenterToTargetHeading.m_RecenteringTime,
                    freeLook.m_RecenterToTargetHeading.m_WaitTime,
                    freeLook.GetComponent<CinemachineInputProvider>()
                );
            }
        }
        recentering = false;
        if (hasFreeLook) input.RecenterEvent += Recenter;
    }

    public void Recenter() {
        if (recentering) return;
        easing = 10;
        doRecenter();
    }

    public void LevelStartRecenter() {
        if (recentering) return;
        easing = 25;
        doRecenter();
    }

    private void doRecenter() {
        recentering = true;
        if (sdc.LiveChild.GetType() == typeof(CinemachineFreeLook)) {
            current = sdc.LiveChild as CinemachineFreeLook;
            current.m_RecenterToTargetHeading.m_RecenteringTime = 0.1f;
            current.m_RecenterToTargetHeading.m_WaitTime = 0f;
            current.m_RecenterToTargetHeading.m_enabled = true;
        }
        ToggleEnabled(false);
    }

    private void FixedUpdate() {
        if (!inited) {
            try {
                // It may take a few frames for the ActiveVirtualCamera to become available.
                // Init will throw an error if ActiveVirtualCamera has not yet been set.
                Init();
                inited = true;
            } catch (Exception e) {
                Debug.Log(e.ToString());
                return;
            }
        }
        if (current && recentering) {

            float deltaY = Math.Abs(current.m_YAxis.Value - 0.1f);
            if (current.m_YAxis.Value > 0.1f) current.m_YAxis.Value -= deltaY/easing;
            bool recenterComplete = Math.Abs(current.m_XAxis.Value) <= 0.01f && Math.Abs(current.m_YAxis.Value) - 0.1f < 0.01f;
            if (recenterComplete) {
                RecenterSettings s = cameraSettings[current.GetHashCode()];
                current.m_RecenterToTargetHeading.m_RecenteringTime = s.recenterTime;
                current.m_RecenterToTargetHeading.m_WaitTime = s.waitTime;
                current.m_RecenterToTargetHeading.m_enabled = s.enabled;
                recentering = false;
                if (!isDialogue) ToggleEnabled(true);
            }
        }
    }

    private void OnDestroy() {
        if (hasFreeLook) input.RecenterEvent -= Recenter;
        EventManager.StopListening<DialogueOpenEvent, Vector3, string>(OnDialogOpen);
        EventManager.StopListening<DialogueCloseEvent, string>(OnDialogClose);
    }
}
