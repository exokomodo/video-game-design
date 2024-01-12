using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputMap
{
    public const KeyCode ToggleCursor = KeyCode.F1;
    public const KeyCode ToggleControlsUI = KeyCode.F12;
    public const KeyCode HardQuit = KeyCode.Escape;
    public const KeyCode CameraSprint = KeyCode.LeftShift;
    public const KeyCode CameraSprintAlt = KeyCode.RightShift;

    public static bool IsCameraSprinting => Input.GetKey(InputMap.CameraSprint) || Input.GetKey(InputMap.CameraSprintAlt);
    public static bool ShouldQuitApplication => Input.GetKeyUp(InputMap.HardQuit);
    public static bool ShouldToggleControlsUI => Input.GetKeyUp(InputMap.ToggleControlsUI);
    public static bool ShouldToggleCursor => Input.GetKeyUp(InputMap.ToggleCursor);
}
