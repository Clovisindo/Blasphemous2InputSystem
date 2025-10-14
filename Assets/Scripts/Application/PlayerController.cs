using Game.Core;
using Game.Domain.StateMachine;
using Game.Events;
using Game.Input;
using Game.Input.Commands;
using UnityEngine;

namespace Game.Application
{

    public class PlayerController : MonoBehaviour
    {
        PlayerStateMachine _stateMachine;
        InputBuffer _buffer;
        IInputService _input;
        IEventBus _eventBus;
        Animator _anim;

        // Opcional: evitar encolar idéntico movimiento cada frame
        Vector2 _lastEnqueuedMovement = Vector2.zero;
        [Tooltip("Si true: solo encola movimiento si cambia o ha pasado el intervalo")]
        public bool optimizeMovementEnqueue = true;
        public float movementEnqueueInterval = 1f / 60f; // en segundos (p. ej. 60 Hz)

        private void Awake()
        {
            _input = Bootstrapper.Container.Resolve<IInputService>();
            _buffer = Bootstrapper.Container.Resolve<InputBuffer>();
            _stateMachine = Bootstrapper.Container.Resolve<PlayerStateMachine>();
            _eventBus = Bootstrapper.Container.Resolve<IEventBus>();
            _anim = GetComponent<Animator>();

            _eventBus.Subscribe<PlayerMoveEvent>(OnMove);
            _eventBus.Subscribe<PlayerAnimationEvent>(OnAnimation);
        }

        private void Update()
        {
            // 1) Procesar comandos puntuales en cola (ataques, etc.)
            while (_input.TryDequeue(out var cmd))
            {
                _buffer.AddCommand(cmd);
                _stateMachine.ProcessCommand(cmd);
            }

            // 2) Movimiento continuo: leer estado cacheado y generar MovementCommand por frame
            _stateMachine.Update(Time.deltaTime);

            var combo = _buffer.DetectCombo();
            if (combo != ComboType.None) _stateMachine.ExecuteCombo(combo);
        }
        void OnMove(PlayerMoveEvent ev)
        {
            transform.position += (Vector3)ev.MovementDelta;
        }

        void OnAnimation(PlayerAnimationEvent ev)
        {
            Debug.Log("Aplicando animacion en jugador.");
            //_anim.SetTrigger(ev.TriggerName);
        }
    }
}