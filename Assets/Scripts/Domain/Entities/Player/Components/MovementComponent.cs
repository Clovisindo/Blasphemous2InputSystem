using Game.Events;
using UnityEngine;
using static Game.Events.PlayerEvents.PlayerEvents;

namespace Game.Domain.Entities.Player
{
    public class MovementComponent
    {
        readonly PlayerEntity _player;
        readonly IEventBus _eventBus;
        public Vector2 KnockbackVelocity { get; private set; }
        public float VerticalVelocity { get; private set; }
        public MovementComponent(PlayerEntity player, IEventBus eventBus)
        {
            _player = player;
            _eventBus = eventBus;
            _player.Capabilities.Add(MoveCapability.Move);
            _player.Capabilities.Add(MoveCapability.Jump);
            _player.Capabilities.Add(MoveCapability.Dash);
            _player.Capabilities.Add(MoveCapability.IsGrounded);
        }

        public void Move(Vector2 direction, float deltaTime)
        {
            if (!_player.Capabilities.Has(MoveCapability.Move))
                return;

            float horizontalMove = direction.x;
            _player.SetPosition( _player.Position + deltaTime * horizontalMove * _player.Stats.Speed * Vector2.right);
            if (horizontalMove != 0)
                _player.Face(new Vector2(Mathf.Sign(horizontalMove), 0));
            _eventBus.Publish(new PlayerMovement(_player.Id, _player.Position));
        }

        public void ApplyGravity(float gravity, float deltaTime)
        {
            VerticalVelocity = gravity == 0 ? 0 : VerticalVelocity - gravity * deltaTime;
            _player.SetPosition(_player.Position + deltaTime * VerticalVelocity * Vector2.up);
            _eventBus.Publish(new PlayerMovement(_player.Id, _player.Position));
            HasLanded();
        }

        // ToDo: temporal para testeo cuando haya fisicas y escenario
        public void HasLanded()
        {
            // Simplificación: leer del controller o de colisiones por evento?
            if (_player.Position.y <= 0)
                _player.Capabilities.Add(MoveCapability.IsGrounded);
        }

        //ToDo: llevarse estos metodos y publicar eventos al PlayerDomainService
        public void Jump()
        {
            if (!_player.Capabilities.Has(MoveCapability.Move))
                return;

            _player.Capabilities.Remove(MoveCapability.IsGrounded);
            VerticalVelocity = _player.Stats.JumpForce;
            _eventBus.Publish(new PlayerJumpStartedEvent(_player.Id, _player.Stats.JumpForce));
        }

        public void Dash(Vector2 direction, float dashSpeed, float deltaTime)
        {
            if (!_player.Capabilities.Has(MoveCapability.Dash))
                return;

            direction.y = 0f;
            direction.Normalize();

            _player.SetPosition( _player.Position + direction * dashSpeed * deltaTime);//ToDo: hay que rehacer todos los movimientos con interpolacion
            _eventBus.Publish(new PlayerMovement(_player.Id, _player.Position));
        }

        public void SetKnockback(Vector2 direction, float force)
        {
            KnockbackVelocity = direction.normalized * force;
        }

        public void ApplyKnockback(float deltaTime)
        {
            _player.SetPosition( _player.Position + KnockbackVelocity.x * deltaTime * Vector2.right);
            _eventBus.Publish(new PlayerMovement(_player.Id, _player.Position));
        }

        public void StartDash()
        {
            //_eventBus.Publish(new PlayerDashStarted(Id));

        }
        public void StopDash()
        {
            //_eventBus.Publish(new PlayerDashEnded(Id));
        }
    }
}
