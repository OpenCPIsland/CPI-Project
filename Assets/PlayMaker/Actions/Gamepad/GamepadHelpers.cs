using System;

namespace HutongGames.PlayMaker
{
    [Serializable]
    public enum GamepadButton
    {
        ButtonNorth,
        ButtonEast,
        ButtonWest,
        ButtonSouth,
        LeftTrigger,
        RightTrigger,
        LeftShoulder,
        RightShoulder,
        SelectButton,
        StartButton
    }

    [Serializable]
    public enum GamepadStick
    {
        LeftStick,
        RightStick,
        DPad
    }
}