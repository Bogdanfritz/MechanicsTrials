using UnityEngine;

public class Constants {
    
    public enum TetherMode
    {
        PullObjectToPlayer,
        PullPlayerToObject,
        Invalid
    }

    public enum TetherState
    {
        Idle,
        Launched,
        Retracting,
        Attached
    }
}
