using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementStateMachine : StateMachine
{
    public Player Player { get; }
    public PlayerIdlingState IdlingState { get; }
    public PlayerRunningState RunningState { get; }
    public PlayerWalkingState WalkingState { get; }
    public PlayerSprintingState SprintingState { get; }

    public PlayerMovementStateMachine(Player player) // 생성자
    {
        Player = player;
        
        
        IdlingState = new PlayerIdlingState(this);
        WalkingState = new PlayerWalkingState(this);
        RunningState = new PlayerRunningState(this);
        SprintingState = new PlayerSprintingState(this);
    }
}
