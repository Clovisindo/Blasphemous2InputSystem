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
            { MovementStateType.Idle, new[] { MovementStateType.Moving, MovementStateType.Jumping, MovementStateType.Dash } },
            { MovementStateType.Moving, new[] { MovementStateType.Idle, MovementStateType.Jumping, MovementStateType.Dash } },
            { MovementStateType.Jumping, new []{MovementStateType.Idle,MovementStateType.Moving } },
            { MovementStateType.Dash, new []{ MovementStateType.Idle, MovementStateType.Moving, MovementStateType.Jumping } },
            };

        public static bool CanTransition(MovementStateType from, MovementStateType to)
        {
            if (!_allowedTransitions.TryGetValue(from, out var nextStates))
                return false;

            return nextStates.Contains(to);
        }
    }
}
