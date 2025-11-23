using Game.Domain.Entities;
using Game.Events;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using static Game.Events.PlayerEvents.PlayerEvents;

public class PlayerEntityTests
{
    PlayerEntity _entity;
    IEventBus _eventBus;
    PlayerStats _stats;

    [SetUp]
    public void Setup()
    {
        _stats = new PlayerStats(1f, 100, 10, 5f, 5f, 1f, 1f);
        _eventBus = Substitute.For<IEventBus>();
        _entity = new PlayerEntity(_stats, _eventBus);
    }


    //-----MOVEMENT-----

    [TestCase(1f, 0f, TestName = "Movement_WhenReceiveRightDirection_ThenMoveRight")]
    [TestCase(-1f, 0f, TestName = "Movement_WhenReceiveLeftDirection_ThenMoveleft")]
    public void Movement_WhenReceiveDirection_ThenMove(float x, float y)
    {
        Vector2 direction = new( x, y);
        Vector2 expectedPosition = new(x, y);
        float dt = 1f;

        _entity.Movement.Move(direction, dt);

        Assert.AreEqual(expectedPosition, _entity.Position);
        Assert.AreEqual(direction, _entity.FacingDirection);
        _eventBus.Received(1).Publish(Arg.Is<PlayerMovement>(ev =>
        ev.PlayerId == _entity.Id &&
        ev.Position == _entity.Position)
            );
    }

    [TestCase(0f, 1f, TestName = "Movement_WhenReceiveUpDirection_ThenDontMove")]
    [TestCase(0f, -1f, TestName = "Movement_WhenReceiveDowntDirection_ThenDontMove")]
    public void Movement_WhenReceiveInvalidDirection_ThenDontMove(float x, float y)
    {
        Vector2 direction = new(x, y);
        Vector2 expectedPosition = _entity.Position;
        Vector2 initialFaceDirection = _entity.FacingDirection;
        float dt = 1f;

        _entity.Movement.Move(direction, dt);

        Assert.AreEqual(expectedPosition, _entity.Position);
        Assert.AreEqual(initialFaceDirection, _entity.FacingDirection);
        _eventBus.Received(1).Publish(Arg.Is<PlayerMovement>(ev =>
            ev.PlayerId == _entity.Id &&
            ev.Position == _entity.Position)
            );
    }

    [TestCase(1f, 0f, TestName = "Movement_WhenReceiveRightDirection_ThenDontMove")]
    [TestCase(-1f, 0f, TestName = "Movement_WhenReceiveLeftDirection_ThenDontMove")]
    public void Movement_WhenDontHaveCapability_ThenDontMove(float x, float y)
    {
        Vector2 direction = new(x, y);
        Vector2 initialPosition = _entity.Position;
        Vector2 initialFaceDirection = _entity.FacingDirection;
        float dt = 1f;
        _entity.Capabilities.Remove(MoveCapability.Move);

        _entity.Movement.Move(direction, dt);

        Assert.AreEqual(initialPosition, _entity.Position);
        Assert.AreEqual(initialFaceDirection, _entity.FacingDirection);
        _eventBus.Received(0).Publish(Arg.Any<PlayerMovement>());
    }

    [Test]
    public void Gravity_WhenJumpAndAplyGravity_ThenUpdatePositionAndLanded()
    {
        float gravity = _entity.Stats.JumpForce + 1f;
        float deltatime = 1f;
        float expectedY =  _entity.Stats.JumpForce - gravity;
        _entity.Movement.Jump();

        _entity.Movement.ApplyGravity(gravity, deltatime);

        Assert.AreEqual(new Vector2(0, expectedY), _entity.Position);
        _eventBus.Received(1).Publish(Arg.Is<PlayerMovement>(ev =>
            ev.PlayerId == _entity.Id &&
            ev.Position == _entity.Position)
            );
        Assert.IsTrue(_entity.Capabilities.Has(MoveCapability.IsGrounded));
    }

    [Test]
    public void Gravity_WhenJumpAndAplyGravity_ThenUpdatePositionButNotLanded()
    {
        float gravity = _entity.Stats.JumpForce - 1f;
        float deltatime = 1f;
        float expectedY = _entity.Stats.JumpForce - gravity;
        _entity.Movement.Jump();

        _entity.Movement.ApplyGravity(gravity, deltatime);

        Assert.AreEqual(new Vector2(0, expectedY), _entity.Position);
        _eventBus.Received(1).Publish(Arg.Is<PlayerMovement>(ev =>
            ev.PlayerId == _entity.Id &&
            ev.Position == _entity.Position)
            );
        Assert.IsTrue(!_entity.Capabilities.Has(MoveCapability.IsGrounded));
    }

    [Test]
    public void Jump_WhenJump_ThenVerifyJump()
    {
        _entity.Movement.Jump();

        Assert.IsTrue(!_entity.Capabilities.Has(MoveCapability.IsGrounded));
        Assert.AreEqual(_entity.Stats.JumpForce,_entity.Movement.VerticalVelocity);
        _eventBus.Received(1).Publish(Arg.Is<PlayerJumpStartedEvent>(ev =>
            ev.PlayerId == _entity.Id &&
            ev.JumpForce == _entity.Stats.JumpForce)
            );
    }
    [Test]
    public void Jump_WhenRemoveMoveCap_ThenVerifyDontJump()
    {
        _entity.Capabilities.Remove(MoveCapability.Move);

        _entity.Movement.Jump();

        Assert.IsTrue(_entity.Capabilities.Has(MoveCapability.IsGrounded));
        Assert.AreEqual(0f, _entity.Movement.VerticalVelocity);
        _eventBus.Received(0).Publish(Arg.Any<PlayerJumpStartedEvent>());
    }

    [Test]
    public void Dash_WhenDash_ThenVerifyDash()
    {
        _entity.Movement.Dash(Vector2.right, _entity.Stats.DashSpeed, 1f);

        Assert.AreEqual(Vector2.right,_entity.Position);
        _eventBus.Received(1).Publish(Arg.Is<PlayerMovement>(ev =>
            ev.PlayerId == _entity.Id &&
            ev.Position == _entity.Position)
            );
    }

    [Test]
    public void Dash_WhenRemoveDashCap_ThenVerifyDontDash()
    {
        _entity.Capabilities.Remove(MoveCapability.Dash);
        _entity.Movement.Dash(Vector2.right, _entity.Stats.DashSpeed, 1f);

        Assert.AreEqual(Vector2.zero, _entity.Position);
        _eventBus.Received(0).Publish(Arg.Any<PlayerMovement>());
    }

    [Test]
    public void Knockback_WhenApplKnockback_ThenVerifyKnockback()
    {
        _entity.Movement.SetKnockback(Vector2.left, _entity.Stats.KnockbackForce);

        _entity.Movement.ApplyKnockback(1f);

        Assert.AreEqual(Vector2.left,_entity.Position);
        _eventBus.Received(1).Publish(Arg.Is<PlayerMovement>(ev =>
            ev.PlayerId == _entity.Id &&
            ev.Position == _entity.Position)
            );

    }
    //-----COMBAT-----

    [Test]
    public void Attack_WhenStartAttack_ThenVerifyAttack()
    {
        _entity.Combat.StartAttack(Utilities.AttackType.Light);

        Assert.IsTrue(_entity.Capabilities.Has(MoveCapability.IsAttacking));
        _eventBus.Received(1).Publish(Arg.Is<PlayerAttackStarted>(ev =>
            ev.PlayerId == _entity.Id &&
            ev.AttackType == Utilities.AttackType.Light)
            );
    }
    [Test]
    public void Attack_WhenStopAttack_ThenVerifyStopped()
    {
        _entity.Combat.StopAttack();

        Assert.IsTrue(!_entity.Capabilities.Has(MoveCapability.IsAttacking));
        _eventBus.Received(1).Publish(Arg.Is<PlayerAttackFinished>(ev => ev.PlayerId == _entity.Id));
    }
    
    //-----HEALTH -----

    [Test]
    public void Health_WhenTakeDamage_ThenVerifyUpdate()
    {
        _entity.Health.TakeDamage(_entity.Stats.MaxHealth - 10);

        Assert.AreEqual(_entity.Stats.MaxHealth - 10, _entity.Stats.CurrentHealth);
    }

    [Test]
    public void Health_WhenStartHurt_ThenVerifyHurt()
    {
        _entity.Health.StartHurt(_entity.Id);

        _eventBus.Received(1).Publish(Arg.Is<PlayerHurtAnimStart>(ev => ev.PlayerId == _entity.Id));
    }

    [Test]
    public void Health_WhenStopHurt_ThenVerifyHurt()
    {
        _entity.Health.StopHurt(_entity.Id);

        _eventBus.Received(1).Publish(Arg.Is<PlayerHurtAnimEnd>(ev => ev.PlayerId == _entity.Id));
    }

    //-----DAMAGE CONTROLLER -----
    //no son necesarios

}
