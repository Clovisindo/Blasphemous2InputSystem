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
        float _lastEnqueueTime = 0f;
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
            //CheckMovement();

            _stateMachine.Update(Time.deltaTime);

            var combo = _buffer.DetectCombo();
            if (combo != ComboType.None) _stateMachine.ExecuteCombo(combo);
        }
        // ToDo: no me gusta usar el unscaledTime ni que esta logica esté en el playerController, repasar posible cambio
        private void CheckMovement()
        {
            //var movement = _input.GetCurrentMovement();

            //if (movement.sqrMagnitude > 0.0001f)
            //{
            //    bool shouldEnqueue = true;
            //    if (optimizeMovementEnqueue)
            //    {
            //        var now = Time.unscaledTime;
            //        // encolar solo si el movimiento cambió o ha pasado el intervalo
            //        if ((movement - _lastEnqueuedMovement).sqrMagnitude < 0.0001f && (now - _lastEnqueueTime) < movementEnqueueInterval)
            //        {
            //            shouldEnqueue = false;
            //        }
            //        else
            //        {
            //            _lastEnqueuedMovement = movement;
            //            _lastEnqueueTime = now;
            //        }
            //    }

            //    if (shouldEnqueue)
            //    {
            //        var moveCmd = new MovementCommand(movement, Time.unscaledTime);
            //        _buffer.AddCommand(moveCmd);
            //        _stateMachine.ProcessCommand(moveCmd);
            //    }
            //}
            //else
            //{
            //    // Si antes se movía y ahora no, avisar al state machine con un MovementCommand(0)
            //    if (_lastEnqueuedMovement.sqrMagnitude > 0.0001f)
            //    {
            //        var zeroCmd = new MovementCommand(Vector2.zero, Time.unscaledTime);
            //        _stateMachine.ProcessCommand(zeroCmd);
            //        _lastEnqueuedMovement = Vector2.zero;
            //    }
            //}
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