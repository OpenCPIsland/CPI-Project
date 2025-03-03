namespace HutongGames.PlayMaker.TweenEnums
{
    public enum TweenDirection
    {
        To = 0,
        From = 1
    }

    public enum LoopType
    {
        None = 0,
        Loop = 1,
        PingPong = 2
    }

    public enum RotationInterpolation
    {
        Spherical = 0,
        Linear = 1
    }

    public enum RotationOptions
    {
        CurrentRotation = 0,
        WorldRotation = 1,
        LocalRotation = 2, // same as world if no parent
        WorldOffsetRotation = 3,
        LocalOffsetRotation = 4,
        MatchGameObjectRotation = 5
    }

    public enum ScaleOptions
    {
        CurrentScale = 0,
        LocalScale = 1,
        MultiplyScale = 2,
        AddToScale = 3,
        MatchGameObject = 4
    }

    public enum PositionOptions
    {
        CurrentPosition = 0,
        WorldPosition = 1,
        LocalPosition = 2, // same as world position if no parent
        WorldOffset = 3,
        LocalOffset = 4,
        TargetGameObject = 5
    }

    public enum UiPositionOptions
    {
        CurrentPosition = 0,
        Position = 1,
        Offset = 2,
        OffscreenTop = 3,
        OffscreenBottom = 4,
        OffscreenLeft = 5,
        OffscreenRight = 6,
        TargetGameObject = 7
    }

    public enum TargetValueOptions
    {
        CurrentValue = 0,
        Offset = 1,
        Value = 2
    }
}