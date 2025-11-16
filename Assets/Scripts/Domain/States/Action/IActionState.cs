using System;
using System.Collections.Generic;
using System.Linq;
using static Utilities;

namespace Game.Domain.StateMachine
{
    public interface IActionState : IPlayerState
    {
        ActionStateType StateType { get; }
    }

    public static class ActionTransitionRules
    {
        private static readonly Dictionary<ActionStateType, ActionStateType[]> _allowedTransitions =
            new()
            {
            { ActionStateType.Idle, new[] { ActionStateType.Attacking, ActionStateType.Hurt } },
            { ActionStateType.Attacking, new[] { ActionStateType.Idle, ActionStateType.Hurt } },
            { ActionStateType.Hurt, new[] { ActionStateType.Idle, ActionStateType.Attacking, ActionStateType.Death } },
            { ActionStateType.Death, Array.Empty<ActionStateType>() },
            };

        public static bool CanTransition(ActionStateType from, ActionStateType to)
        {
            if (!_allowedTransitions.TryGetValue(from, out var nextStates))
                return false;

            return nextStates.Contains(to);
        }
    }
}
