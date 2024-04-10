using UnityEngine;
using Cinemachine;
using System.Collections.Generic;
using System;

public struct RecenterSettings {
    public bool enabled;
    public float recenterTime;
    public float waitTime;

    public RecenterSettings(bool enabled, float recenterTime, float waitTime) {
        this.enabled = enabled;
        this.recenterTime = recenterTime;
        this.waitTime = waitTime;
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
                    freeLook.m_RecenterToTargetHeading.m_WaitTime
                );
            }
        }
        recentering = false;
        if (hasFreeLook) input.RecenterEvent += OnRecenter;
    }

    private void OnRecenter() {
        if (recentering) return;

        recentering = true;
        if (sdc.LiveChild.GetType() == typeof(CinemachineFreeLook)) {
            current = sdc.LiveChild as CinemachineFreeLook;
            current.m_RecenterToTargetHeading.m_RecenteringTime = 0.1f;
            current.m_RecenterToTargetHeading.m_WaitTime = 0f;
            current.m_RecenterToTargetHeading.m_enabled = true;
        }
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
            bool recenterComplete = Math.Abs(current.m_XAxis.Value) <= 0.01f;
            if (recenterComplete) {
                RecenterSettings s = cameraSettings[current.GetHashCode()];
                current.m_RecenterToTargetHeading.m_RecenteringTime = s.recenterTime;
                current.m_RecenterToTargetHeading.m_WaitTime = s.waitTime;
                current.m_RecenterToTargetHeading.m_enabled = s.enabled;
                recentering = false;
            }
        }
    }

    private void OnDestroy() {
        if (hasFreeLook) input.RecenterEvent -= OnRecenter;
    }
}
