using Game.Core;
using Game.Domain.StateMachine;
using Game.Events;
using Game.Input;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerStateMachine _stateMachine;
    InputBuffer _buffer;
    IInputService _input;
    IEventBus _eventBus;
    Animator _anim;

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
        while(_input.TryDequeue(out var cmd))
        {
            _buffer.AddCommand(cmd);
            _stateMachine.ProcessCommand(cmd);
        }

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
