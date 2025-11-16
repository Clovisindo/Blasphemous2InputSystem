using System.Collections.Generic;
using static Utilities;

namespace Game.Domain.StateMachine
{
    public static class PlayerStateRules
    {
        //ahora mismo le estamos dando el uso a estas reglas de comprobar cuando entra el estado action
        //no sirve para mientras esta el estado action haciendose, comprobar los otros cambios de move
        //Entendemos que queremos que haya algunos movimientos y transiciones, y si no usamos los capables para limitarlo
        private static readonly Dictionary<(MovementStateType, ActionStateType), bool> _rules = new()
        {
            // attack rules
            {(MovementStateType.Idle, ActionStateType.Attacking), true},
            {(MovementStateType.Moving, ActionStateType.Attacking), true},
            {(MovementStateType.Jumping,ActionStateType.Attacking), true}, // air attack allowed
            {(MovementStateType.Climb, ActionStateType.Attacking), false},
            {(MovementStateType.Dash,  ActionStateType.Attacking), false},

             // hurt rules
            {(MovementStateType.Idle, ActionStateType.Hurt), true},
            {(MovementStateType.Moving, ActionStateType.Hurt), true},
            {(MovementStateType.Jumping,ActionStateType.Hurt), true},
            {(MovementStateType.Climb, ActionStateType.Hurt), true},
            {(MovementStateType.Dash,  ActionStateType.Hurt), false},

             // death rules
            {(MovementStateType.Idle, ActionStateType.Death), true},
            {(MovementStateType.Moving, ActionStateType.Death), true},
            {(MovementStateType.Jumping,ActionStateType.Death), true},
            {(MovementStateType.Climb, ActionStateType.Death), true},
            {(MovementStateType.Dash,  ActionStateType.Death), true},
        };

        public static bool CanCombine(MovementStateType move, ActionStateType action)
            => _rules.TryGetValue((move, action), out var ok) ? ok : false;// si no existe la regla, falso
    }
}
