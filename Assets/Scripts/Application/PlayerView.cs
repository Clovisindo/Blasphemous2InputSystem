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
        Animator _anim;
        IEventBus _eventBus;
        Guid _playerId;
        public Guid GetPlayerId() => _playerId;


        private void Awake()
        {
            CoreOrchestrator.Register(this);
        }
        public void OnCoreReady()
        {
            _eventBus = Bootstrapper.Container.Resolve<IEventBus>();
            _playerId = Bootstrapper.Container.Resolve<PlayerEntity>().Id;
            _eventBus.Subscribe<PlayerAttackStarted>(OnAttackStarted);
            _eventBus.Subscribe<PlayerAttackFinished>(OnAttackFinished);
            _eventBus.Subscribe<PlayerMovement>(OnMoved);
            _eventBus.Subscribe<PlayerDamagedEvent>(OnDamaged);
            _eventBus.Subscribe<PlayerHurtStartedEvent>(OnHurtStarted);
            _eventBus.Subscribe<PlayerHurtEndedEvent>(OnHurtEnded);
        }

        private void OnDamaged(PlayerDamagedEvent evt)
        {
            if (evt.PlayerId != _playerId) return;
            //_anim.SetTrigger("Hit");//ToDo
            //SFX, camera shake, particles...
        }

        private void OnHurtEnded(PlayerHurtEndedEvent evt)
        {
            if (evt.PlayerId != _playerId) return;
            //_anim.SetBool("IsHurt", true);
        }

        private void OnHurtStarted(PlayerHurtStartedEvent evt)
        {
            if (evt.PlayerId != _playerId) return;
            //_anim.SetBool("IsHurt", false);
        }

        void OnDestroy()
        {
            _eventBus.Unsubscribe<PlayerAttackStarted>(OnAttackStarted);
            _eventBus.Unsubscribe<PlayerAttackFinished>(OnAttackFinished);
            _eventBus.Unsubscribe<PlayerMovement>(OnMoved);
            _eventBus.Unsubscribe<PlayerDamagedEvent>(OnDamaged);
            _eventBus.Unsubscribe<PlayerHurtStartedEvent>(OnHurtStarted);
            _eventBus.Unsubscribe<PlayerHurtEndedEvent>(OnHurtEnded);
        }

        private void OnAttackStarted(PlayerAttackStarted evt)
        {
            if (evt.PlayerId != _playerId) return;
            if(evt.AttackType != Utilities.AttackType.Heavy)
            {
                //ToDo Animacion
            }
        }

        private void OnAttackFinished(PlayerAttackFinished evt)
        {
            if (evt.PlayerId != _playerId) return;
        }

        private void OnMoved(PlayerMovement evt)
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
