using Game.Core;
using Game.Core.Orchestrator;
using Game.Domain.Entities;
using Game.Events;
using System;
using UnityEngine;
using static Game.Events.PlayerEvents.PlayerEvents;

namespace Game.Application
{
    public class PlayerView: MonoBehaviour,ICoreDependent
    {
        IEventBus _eventBus;
        Guid _playerId;


        private void Awake()
        {
            CoreOrchestrator.Register(this);
        }
        public void OnCoreReady()
        {
            _eventBus = Bootstrapper.Container.Resolve<IEventBus>();
            _playerId = Bootstrapper.Container.Resolve<PlayerEntity>().Id;
            _eventBus.Subscribe<AttackStarted>(OnAttackStarted);
            _eventBus.Subscribe<AttackFinished>(OnAttackFinished);
            _eventBus.Subscribe<Movement>(OnMoved);
        }

        void OnDestroy()
        {
            _eventBus.Unsubscribe<AttackStarted>(OnAttackStarted);
            _eventBus.Unsubscribe<AttackFinished>(OnAttackFinished);
            _eventBus.Unsubscribe<Movement>(OnMoved);
        }

        private void OnAttackStarted(AttackStarted started)
        {
            throw new NotImplementedException();
        }

        private void OnAttackFinished(AttackFinished finished)
        {
            throw new NotImplementedException();
        }

        private void OnMoved(Movement evt)
        {
            if (evt.PlayerId != _playerId) return;
            transform.position = evt.Direction;
            //aplicar direcion a la que se enfoca el personaje
            //_animator.SetFloat("Speed", evt.Direction.magnitude);
        }

        private void Update()
        {
            // Actualizar animaciones en base a _entity.IsAttacking, etc.??
        }

        
    }
}
