using Game.Domain.Entities;
using Game.Events;
using Game.Input.Commands;
using UnityEngine;

namespace Game.Domain.StateMachine
{
    public class MovingState : IPlayerState
    {
        readonly PlayerStateMachine _stateMachine;
        readonly PlayerEntity _playerEntity;
        readonly IEventBus _eventBus;

        public MovingState(PlayerEntity playerEntity, PlayerStateMachine stateMachine, IEventBus eventBus)
        {
            _playerEntity = playerEntity;
            _stateMachine = stateMachine;
            _eventBus = eventBus;
        }

        public void Enter()
        {
            Debug.Log("Enter Moving");
            //_eventBus.Publish(new PlayerAnimationEvent("Run"));
        }

        public void Exit() { }

        public void HandleCommand(InputCommand cmd)
        {
            if ( cmd is MovementCommand move)
            {
                HandleMovement(move);
            }
            else if ( cmd is AttackCommand atk)
            {
                _stateMachine.ChangeState<AttackingState>();
                _eventBus.Publish(new PlayerAttackEvent { Type = atk.Type });// ToDo: quitar por que ya no va aqui si no en la entidad
            }
        }

        void HandleMovement( MovementCommand move)
        {
            if (move.Direction.sqrMagnitude > 0.01f)
                _playerEntity.Move(move.Direction, move.Timestamp);
            else
            {
                _stateMachine.ChangeState<IdleState>();
            }
        }

        /// <summary>
        /// solo par animaciones dependientes del tiempo
        /// </summary>
        /// <param name="dt"></param>
        public void Update(float dt) { }
    }
}
