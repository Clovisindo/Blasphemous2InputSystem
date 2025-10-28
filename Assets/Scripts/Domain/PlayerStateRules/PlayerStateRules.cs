using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Utilities;

namespace Game.Domain.StateMachine
{
    public static class PlayerStateRules
    {
        private static readonly Dictionary<(MovementStateType, ActionStateType), bool> _rules = new()
        {
            // attack rules
            {(MovementStateType.Idle, ActionStateType.Attacking), true},
            {(MovementStateType.Moving, ActionStateType.Attacking), true},
            {(MovementStateType.Jumping,ActionStateType.Attacking), true}, // air attack allowed
            {(MovementStateType.Falling,ActionStateType.Attacking), true},
            {(MovementStateType.Climb, ActionStateType.Attacking), false},
            {(MovementStateType.Dash,  ActionStateType.Attacking), false},
            {(MovementStateType.Hurt,  ActionStateType.Attacking), false},
            {(MovementStateType.Death, ActionStateType.Attacking), false},

             // dash rules
            {(MovementStateType.Idle, ActionStateType.Dashing), true},
            {(MovementStateType.Moving, ActionStateType.Dashing), true},
            {(MovementStateType.Jumping, ActionStateType.Dashing), false},
            {(MovementStateType.Climb, ActionStateType.Dashing), false},
            {(MovementStateType.Death, ActionStateType.Dashing), false},
            {(MovementStateType.Hurt, ActionStateType.Dashing), false},
        };

        public static bool CanCombine(MovementStateType move, ActionStateType action)
            => _rules.TryGetValue((move, action), out var ok) ? ok : false;// si no existe la regla, falso
    }
}
