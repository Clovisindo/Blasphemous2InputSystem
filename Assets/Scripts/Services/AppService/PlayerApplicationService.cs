using Game.Domain.Entities;
using Game.Domain.Services;
using Game.Domain.StateMachine;
using Game.Events;
using Game.Input;
using Game.Input.Commands;
using static Game.Events.PlayerEvents.PlayerEvents;
using static Utilities;
using Debug = UnityEngine.Debug;

namespace Game.Services.Application
{
    public class PlayerApplicationService : IPlayerApplicationService
    {
        readonly IPlayerStateMachine _stateMachines;
        readonly IPlayerDomainService _playerDomainService;
        readonly PlayerEntity _playerEntity;
        readonly IInputBuffer _inputBuffer;
        readonly IEventBus _eventBus;

        public PlayerApplicationService(IPlayerStateMachine stateMachines, IPlayerDomainService playerDomainService, IInputBuffer buffer, PlayerEntity playerEntity, IEventBus bus)
        {
            _stateMachines = stateMachines;
            _playerDomainService = playerDomainService;
            _playerEntity = playerEntity;
            _inputBuffer = buffer;
            _eventBus = bus;
            _eventBus.Subscribe<DamageIntentEvent>(OnDamageIntent);
            _eventBus.Subscribe<PlayerDamagedEvent>(OnPlayerDamaged);
            _eventBus.Subscribe<PlayerDiedEvent>(OnPlayerDied);
            _eventBus.Subscribe<MoveStateEndedEvent>(OnMoveStateEndEvent);
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<DamageIntentEvent>(OnDamageIntent);
            _eventBus.Unsubscribe<PlayerDamagedEvent>(OnPlayerDamaged);
            _eventBus.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
            _eventBus.Unsubscribe<MoveStateEndedEvent>(OnMoveStateEndEvent);
        }

        public void ProcessInputCommands(InputCommand command, float deltaTime)
        {
            switch (command)
            {
                // Movimiento (FSM de movimiento)
                case MovementCommand:
                case JumpCommand:
                case DashCommand:
                    _stateMachines.Movement.ProcessCommand(command);
                    break;

                // Ataque (FSM de acción)
                case AttackCommand attackCmd:
                    HandleAttackCommand(attackCmd);
                    break;

                default:
                    break;
            }

            // Aquí se pueden analizar combos, sin implementar de momento
            var detectedCombo = _inputBuffer.DetectCombo();
            if (detectedCombo != ComboType.None)
                _stateMachines.ExecuteCombo(detectedCombo);
        }

        private void HandleAttackCommand(AttackCommand attackCmd)
        {
            var moveState = _stateMachines.Movement.CurrentStateType;

            if (PlayerStateRules.CanCombine(moveState, ActionStateType.Attacking))// reglas entre move y action
            {
                _inputBuffer.AddCommand(attackCmd);// Añadimos al buffer antes de procesar
                _stateMachines.Action.ProcessCommand(attackCmd);//las reglas internas de transicion de action se las dejamos a la StateMachine
            }
            else
            {
                _eventBus.Publish(new PlayerActionBlockedEvent(_playerEntity.Id, ActionStateType.Attacking, moveState));
                Debug.Log($" La accion  de atacar está bloqueada con el comando de movimiento : {moveState}");
            }
        }

        public void Update(float deltaTime)
        {
            _stateMachines.Update(deltaTime);
        }
        /// <summary>
        /// Evento para transiciones desde estados autoconclusivos segun el inputBuffer
        /// </summary>
        /// <param name="evt"></param>
        //No comprobamos aqui reglas por que son solo las internas de movimiento que ya se hacen en IMovementStateMachine
        private void OnMoveStateEndEvent(MoveStateEndedEvent evt)
        {
            if (evt.PlayerId != _playerEntity.Id) return;
            var cmd = evt.InputCommand;
            _inputBuffer.Clear();

            switch (cmd)
            {
                case JumpCommand:
                    _stateMachines.Movement.ChangeState<JumpState>(MovementStateType.Jumping);
                    break;

                case DashCommand:
                    _stateMachines.Movement.ChangeState<DashState>(MovementStateType.Dash);
                    break;

                default:
                    _stateMachines.Movement.ChangeState<IdleState>(MovementStateType.Idle);
                    break;
            }
        }

        private void OnDamageIntent(DamageIntentEvent intent)
        {
            if (intent.TargetPlayerId != _playerEntity.Id) return;

            if (_playerEntity.DamageController.isInvulnerable)//comprobamos invulnerable, escudos, fuego amigo etc
            {
                _eventBus.Publish(new PlayerDamageIgnored());
                return;
            }
            var currentMoveState = _stateMachines.Movement.CurrentStateType;
            _playerDomainService.ApplyDamage(_playerEntity, currentMoveState, intent.Damage, intent.KnockbackDirection);//llamamos al app domain service para aplicar el daño
        }

        private void OnPlayerDied(PlayerDiedEvent evt)
        {
            if (evt.PlayerId != _playerEntity.Id) return;
            _stateMachines.Action.ChangeState<DeathActionState>(ActionStateType.Death);
        }

        private void OnPlayerDamaged(PlayerDamagedEvent evt)
        {
            if (evt.PlayerId != _playerEntity.Id) return;

            _stateMachines.Action.ChangeState<HurtActionState>(ActionStateType.Hurt);
        }
        

    }
}
