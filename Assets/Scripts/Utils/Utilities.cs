using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities 
{
    public enum AttackType { Light, Heavy }
    public enum MovementStateType { Idle, Moving, Jumping, Climb, Dash, Death }
    public enum ActionStateType { Idle, Attacking, Hurt, Death }
}

public class StateTimer
{
    float _duration, _elapsed;

    public StateTimer(float duration)
    {
        _duration = duration;
    }

    public bool IsFinished => _elapsed >= _duration;
    public float Elapsed => _elapsed;
    public void Update(float dt) => _elapsed += dt;
}
