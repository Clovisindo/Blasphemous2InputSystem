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
        readonly PlayerStateMachine _stateMachines;
        readonly PlayerDomainService _playerDomainService;
        readonly InputBuffer _buffer;
        readonly PlayerEntity _playerEntity;
        readonly IEventBus _eventBus;

        public PlayerApplicationService(PlayerStateMachine stateMachines, PlayerDomainService playerDomainService, InputBuffer buffer, PlayerEntity playerEntity, IEventBus bus)
        {
            _stateMachines = stateMachines;
            _playerDomainService = playerDomainService;
            _buffer = buffer;
            _playerEntity = playerEntity;
            _eventBus = bus;
            _eventBus.Subscribe<DamageIntentEvent>(OnDamageIntent);
            _eventBus.Subscribe<PlayerDamagedEvent>(OnPlayerDamaged);
            _eventBus.Subscribe<PlayerDiedEvent>(OnPlayerDied);
        }
        public void Dispose()
        {
            _eventBus.Unsubscribe<DamageIntentEvent>(OnDamageIntent);
            _eventBus.Unsubscribe<PlayerDamagedEvent>(OnPlayerDamaged);
            _eventBus.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
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
            var detectedCombo = _buffer.DetectCombo();
            if (detectedCombo != ComboType.None)
                _stateMachines.ExecuteCombo(detectedCombo);
        }

        private void HandleAttackCommand(AttackCommand attackCmd)
        {
            var moveState = _stateMachines.Movement.CurrentStateType;

            if (PlayerStateRules.CanCombine(moveState, ActionStateType.Attacking))// reglas entre move y action
            {
                _buffer.AddCommand(attackCmd);// Añadimos al buffer antes de procesar
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

        private void OnDamageIntent(DamageIntentEvent intent)
        {
            if (intent.TargetPlayerId != _playerEntity.Id) return;

            if (_playerEntity.Flags.IsInvulnerable)//comprobamos invulnerable, escudos, fuego amigo etc
            {
                //_eventBus.Publish(new playerDamageIgnored());
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
