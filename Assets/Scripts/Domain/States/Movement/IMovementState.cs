using System;
using System.Collections.Generic;
using System.Linq;
using static Utilities;

namespace Game.Domain.StateMachine
{
    public interface IMovementState : IPlayerState
    {
        MovementStateType StateType { get; }
    }

    public static class MoveTransitionRules
    {
        private static readonly Dictionary<MovementStateType, MovementStateType[]> _allowedTransitions =
            new()
            {
            { MovementStateType.Idle, new[] { MovementStateType.Moving, MovementStateType.Jumping, MovementStateType.Dash,MovementStateType.Hurt,MovementStateType.Death } },
            { MovementStateType.Moving, new[] { MovementStateType.Idle, MovementStateType.Jumping, MovementStateType.Dash,MovementStateType.Hurt,MovementStateType.Death } },
            { MovementStateType.Jumping, new []{MovementStateType.Idle,MovementStateType.Moving,MovementStateType.Hurt,MovementStateType.Death } },
            { MovementStateType.Climb, new []{MovementStateType.Jumping,MovementStateType.Hurt,MovementStateType.Death } },
            { MovementStateType.Dash, new []{ MovementStateType.Idle, MovementStateType.Moving, MovementStateType.Jumping,MovementStateType.Hurt,MovementStateType.Death } },
            { MovementStateType.Hurt, new []{ MovementStateType.Idle, MovementStateType.Moving, MovementStateType.Jumping, MovementStateType.Climb, MovementStateType.Death } },
            { MovementStateType.Death, Array.Empty<MovementStateType>() }
            };

        public static bool CanTransition(MovementStateType from, MovementStateType to)
        {
            if (!_allowedTransitions.TryGetValue(from, out var nextStates))
                return false;

            return nextStates.Contains(to);
        }
    }
}
