using Game.Domain.Entities;
using Game.Domain.Services;
using Game.Domain.StateMachine;
using Game.Events;
using Game.Input;
using Game.Input.Commands;
using Game.Services.Application;
using NSubstitute;
using NUnit.Framework;
using System;
using UnityEngine;
using static Game.Events.PlayerEvents.PlayerEvents;
using static Utilities;

public class PlayerApplicationServiceTests 
{
    PlayerApplicationService _appService;
    IPlayerStateMachine _stateMachine;
    IMovementStateMachine _movementStateMachine;
    IActionStateMachine _actionStateMachine;
    IPlayerDomainService _domainService;
    IInputBuffer _inputBuffer;
    IEventBus _eventBus;
    PlayerEntity _entity;

    Action<MoveStateEndedEvent> _moveEndedHandler;
    Action<DamageIntentEvent> _DamageIntentHandler;
    Action<PlayerDiedEvent> _onPlayerDiedHandler;
    Action<PlayerDamagedEvent> _onPlayerDamagedHandler;
    DamageIntentEvent damageIntentEvent;
    DamageIntentEvent damageIntentEventWrongID;

    [SetUp]
    public void Setup()
    {
        InitSubstitutes();
        CaptureEventsHandlers();
        InitEntities();
        AssignTestVariablesValues();
    }

    private void AssignTestVariablesValues()
    {
        damageIntentEvent = new DamageIntentEvent(_entity.Id, 10, 1f, Vector2.left, new GameObject());
        damageIntentEventWrongID = new DamageIntentEvent(new Guid(), 10, 1f, Vector2.left, new GameObject());
        _stateMachine.Movement.Returns(_movementStateMachine);
        _stateMachine.Action.Returns(_actionStateMachine);
    }

    private void CaptureEventsHandlers()
    {
        _eventBus.When(bus => bus.Subscribe<MoveStateEndedEvent>(Arg.Any<Action<MoveStateEndedEvent>>()))
           .Do(ci =>
           {
               _moveEndedHandler = ci.Arg<Action<MoveStateEndedEvent>>();
           });
        _eventBus.When(bus => bus.Subscribe<DamageIntentEvent>(Arg.Any<Action<DamageIntentEvent>>()))
            .Do(ci =>
            {
                _DamageIntentHandler = ci.Arg<Action<DamageIntentEvent>>();
            });
        _eventBus.When(bus => bus.Subscribe<PlayerDiedEvent>(Arg.Any<Action<PlayerDiedEvent>>()))
            .Do(ci =>
            {
                _onPlayerDiedHandler = ci.Arg<Action<PlayerDiedEvent>>();
            });
        _eventBus.When(bus => bus.Subscribe<PlayerDamagedEvent>(Arg.Any<Action<PlayerDamagedEvent>>()))
            .Do(ci =>
            {
                _onPlayerDamagedHandler = ci.Arg<Action<PlayerDamagedEvent>>();
            });
    }

    private void InitEntities()
    {
        var playerStats = new PlayerStats(1f, 100, 10, 5f, 5f, 1f, 1f);
        _entity = new PlayerEntity(playerStats, _eventBus);
        _appService = new PlayerApplicationService(_stateMachine, _domainService, _inputBuffer, _entity, _eventBus);
    }

    private void InitSubstitutes()
    {
        _stateMachine = Substitute.For<IPlayerStateMachine>();
        _movementStateMachine = Substitute.For<IMovementStateMachine>();
        _actionStateMachine = Substitute.For<IActionStateMachine>();
        _domainService = Substitute.For<IPlayerDomainService>();
        _inputBuffer = Substitute.For<IInputBuffer>();
        _eventBus = Substitute.For<IEventBus>();
    }

    [TearDown]
    public void TearDown()
    {
        _appService.Dispose();
        _appService = null;
    }

    [Test]
    public void ProcessInputCommands_WhenMoveCommand_ThenProcessMoveAndCombo()
    {
        MovementCommand moveCmd = new(Vector2.right, 1f);

        _appService.ProcessInputCommands(moveCmd, 1f);

        _movementStateMachine.Received(1).ProcessCommand(Arg.Any<MovementCommand>());
        _inputBuffer.Received(1).DetectCombo();
        _stateMachine.DidNotReceive().ExecuteCombo(Arg.Any<ComboType>());
    }
    
    [Test]
    public void ProcessInputCommands_WhenActionCommand_ThenProcessActionAndCombo()
    {
        AttackCommand attkCmd = new(AttackType.Light,1f);

        _appService.ProcessInputCommands(attkCmd, 1f);

        _inputBuffer.Received(1).AddCommand(Arg.Is(attkCmd));
        _actionStateMachine.Received(1).ProcessCommand(Arg.Any<AttackCommand>());
        _eventBus.DidNotReceive().Publish(Arg.Any<PlayerActionBlockedEvent>());
        _inputBuffer.Received(1).DetectCombo();
        _stateMachine.DidNotReceive().ExecuteCombo(Arg.Any<ComboType>());
    }
    
    [Test]
    public void ProcessInputCommands_WhenActionCommandButRuleNotAllowed_ThenProcessActionAndCombo()
    {
        _stateMachine.Movement.CurrentStateType.Returns(MovementStateType.Dash);
        AttackCommand attkCmd = new(AttackType.Light, 1f);


        _appService.ProcessInputCommands(attkCmd, 1f);

        _inputBuffer.DidNotReceive().AddCommand(Arg.Is(attkCmd));
        _actionStateMachine.DidNotReceive().ProcessCommand(Arg.Any<AttackCommand>());
        _eventBus.Received(1).Publish(Arg.Is<PlayerActionBlockedEvent>(ev =>
           ev.PlayerId == _entity.Id &&
            ev.ActionStateType == ActionStateType.Attacking &&
            ev.CurrentMoveStateType == MovementStateType.Dash)
        );
        _inputBuffer.Received(1).DetectCombo();
        _stateMachine.DidNotReceive().ExecuteCombo(Arg.Any<ComboType>());
    }

    [Test]
    public void Update_WhenUpdateStateMachines_ThenVerify()
    {
        _appService.Update(1f);

        _stateMachine.Received(1).Update(Arg.Any<float>());
    }

    [Test]
    public void OnMovementStateEnd_WhenInvokeWrongId_ThenDoNothing()
    {
        _moveEndedHandler(new MoveStateEndedEvent(new Guid(), new JumpCommand(1f)));

        _movementStateMachine.DidNotReceive().ChangeState<JumpState>(Arg.Is(MovementStateType.Jumping));
        _inputBuffer.DidNotReceive().Clear();
    }

    [Test]
    public void OnMovementStateEnd_WhenInvokeEventJump_ThenVerifyParamenters()
    {
        _moveEndedHandler(new MoveStateEndedEvent(_entity.Id, new JumpCommand(1f)));
        _inputBuffer.Received(1).Clear();
        _movementStateMachine.Received(1).ChangeState<JumpState>(Arg.Is(MovementStateType.Jumping));
    }

    [Test]
    public void OnMovementStateEnd_WhenInvokeEventDash_ThenVerifyParamenters()
    {
        _moveEndedHandler(new MoveStateEndedEvent(_entity.Id, new DashCommand(1f)));
        _inputBuffer.Received(1).Clear();
        _movementStateMachine.Received(1).ChangeState<DashState>(Arg.Is(MovementStateType.Dash));
    }

    [Test]
    public void OnMovementStateEnd_WhenInvokeEventOther_ThenVerifyParamenters()
    {
        _moveEndedHandler(new MoveStateEndedEvent(_entity.Id, new MovementCommand(Vector2.right,1f)));
        _inputBuffer.Received(1).Clear();
        _movementStateMachine.Received(1).ChangeState<IdleState>(Arg.Is(MovementStateType.Idle));
    }

    [Test]
    public void OnDamageIntent_WhenInvokeWrongId_ThenDoNothing()
    {
        _DamageIntentHandler(damageIntentEventWrongID);

        _eventBus.DidNotReceive().Publish(Arg.Any<PlayerDamageIgnored>());
        _domainService.DidNotReceive().ApplyDamage(Arg.Any<PlayerEntity>(),Arg.Any<MovementStateType>(),Arg.Any<int>(),Arg.Any<Vector2>());
    }
    
    [Test]
    public void OnDamageIntent_WhenPlayerInmune_ThenInvokeDamageIgnored()
    {
        _entity.DamageController.AddInvulnerability(Game.Domain.Entities.Player.InvulnerableCapability.Shield);
        _DamageIntentHandler(damageIntentEvent);

        _eventBus.Received(1).Publish(Arg.Any<PlayerDamageIgnored>());
        _domainService.DidNotReceive().ApplyDamage(Arg.Any<PlayerEntity>(), Arg.Any<MovementStateType>(), Arg.Any<int>(), Arg.Any<Vector2>());
    }

    [Test]
    public void OnDamageIntent_WhenApplyDamage_ThenVerify()
    {
        _DamageIntentHandler(damageIntentEvent);

        _eventBus.DidNotReceive().Publish(Arg.Any<PlayerDamageIgnored>());
        _domainService.Received(1).ApplyDamage(_entity, Arg.Any<MovementStateType>(), damageIntentEvent.Damage, Vector2.left);

    }

    [Test]
    public void OnPlayerDied_WhenWrongID_ThenDoNothing()
    {
        _onPlayerDiedHandler(new PlayerDiedEvent(new Guid()));

        _actionStateMachine.DidNotReceive().ChangeState<DeathActionState>(Arg.Is(ActionStateType.Death));
    }

    [Test]
    public void OnPlayerDied_WhenInvoke_ThenVerify()
    {
        _onPlayerDiedHandler(new PlayerDiedEvent(_entity.Id));

        _actionStateMachine.Received(1).ChangeState<DeathActionState>(Arg.Is(ActionStateType.Death));
    }

    [Test]
    public void OnPlayerDamaged_WhenWrongId_ThenDoNothing()
    {
        _onPlayerDamagedHandler(new PlayerDamagedEvent(new Guid(), 10, false));

        _actionStateMachine.DidNotReceive().ChangeState<HurtActionState>(Arg.Is(ActionStateType.Hurt));
    }

    [Test]
    public void OnPlayerDamaged_WhenInvoke_ThenVerify()
    {
        _onPlayerDamagedHandler(new PlayerDamagedEvent(_entity.Id, 10, false));

        _actionStateMachine.Received(1).ChangeState<HurtActionState>(Arg.Is(ActionStateType.Hurt));
    }
}
