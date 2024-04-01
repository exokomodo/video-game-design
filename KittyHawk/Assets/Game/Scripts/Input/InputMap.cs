using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputMap
{
    public const KeyCode ToggleCursor = KeyCode.F1;
    public const KeyCode ToggleControlsUI = KeyCode.F12;
    public const KeyCode HardQuit = KeyCode.Escape;
    public const KeyCode Sprint = KeyCode.LeftShift;
    public const KeyCode SprintAlt = KeyCode.RightShift;
    public const KeyCode Jump = KeyCode.Space;

    public static bool IsJumping => Input.GetKey(InputMap.Jump);
    public static bool IsSprinting => Input.GetKey(InputMap.Sprint) || Input.GetKey(InputMap.SprintAlt);
    public static bool ShouldQuitApplication => Input.GetKeyUp(InputMap.HardQuit);
    public static bool ShouldToggleControlsUI => Input.GetKeyUp(InputMap.ToggleControlsUI);
}
