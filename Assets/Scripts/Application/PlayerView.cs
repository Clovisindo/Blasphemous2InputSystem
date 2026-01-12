using Game.Core;
using Game.Core.Orchestrator;
using Game.Domain.Entities;
using Game.Events;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Game.Events.PlayerEvents.PlayerEvents;
using static Utilities;

namespace Game.Application
{
    public class PlayerView: MonoBehaviour,ICoreDependent
    {
        Animator _anim;
        IEventBus _eventBus;
        Guid _playerId;

        //log estados
        [SerializeField] TextMeshProUGUI moveStateText;
        [SerializeField] TextMeshProUGUI actionStateText;
        MovementStateType currentMovementStateType;
        ActionStateType currentActionStateType;
        SpriteRenderer spriteRenderer;
        private static readonly Dictionary<MovementStateType, Color> _stateColors = new()
    {
        { MovementStateType.Idle, Color.white },
        { MovementStateType.Moving, Color.green },
        { MovementStateType.Jumping, Color.yellow },
        { MovementStateType.Dash, Color.cyan },
        { MovementStateType.Death, Color.black },
    };
        public Guid GetPlayerId() => _playerId;


        private void Awake()
        {
            CoreOrchestrator.Register(this);
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        public void OnCoreReady()
        {
            _eventBus = Bootstrapper.Container.Resolve<IEventBus>();
            _playerId = Bootstrapper.Container.Resolve<PlayerEntity>().Id;
            _eventBus.Subscribe<PlayerAttackStarted>(OnAttackStarted);
            _eventBus.Subscribe<PlayerAttackFinished>(OnAttackFinished);
            _eventBus.Subscribe<PlayerMovement>(OnMoved);
            _eventBus.Subscribe<PlayerDamagedEvent>(OnDamaged);
            _eventBus.Subscribe<PlayerHurtAnimStart>(OnHurtStarted);
            _eventBus.Subscribe<PlayerHurtAnimEnd>(OnHurtEnded);
            _eventBus.Subscribe<PlayerUpdateMoveStateView>(OnUpdateMoveState);
            _eventBus.Subscribe<PlayerUpdateActionStateView>(OnUpdateActionState);
        }

        private void OnUpdateMoveState(PlayerUpdateMoveStateView evt)
        {
            currentMovementStateType = evt.MovementStateType;
            SetDebugStateView();
        }

        private void OnUpdateActionState(PlayerUpdateActionStateView evt)
        {
            currentActionStateType = evt.ActionStateType;
            SetDebugStateView();
        }

        private void SetDebugStateView()
        {
            moveStateText.text = $"Estado movimiendo : {currentMovementStateType}.";
            actionStateText.text = $"Estado accion  : {currentActionStateType}.";
            if (_stateColors.TryGetValue(currentMovementStateType, out var color))
                spriteRenderer.color = color;
        }

        private void OnDamaged(PlayerDamagedEvent evt)
        {
            if (evt.PlayerId != _playerId) return;
            //_anim.SetTrigger("Hit");//ToDo
            //SFX, camera shake, particles...
        }

        private void OnHurtEnded(PlayerHurtAnimEnd evt)
        {
            if (evt.PlayerId != _playerId) return;
            //_anim.SetBool("IsHurt", true);
        }

        private void OnHurtStarted(PlayerHurtAnimStart evt)
        {
            if (evt.PlayerId != _playerId) return;
            //_anim.SetBool("IsHurt", false);
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
            transform.position = evt.Position;
            //aplicar direcion a la que se enfoca el personaje
            //_animator.SetFloat("Speed", evt.Direction.magnitude);
        }

        void OnDestroy()
        {
            _eventBus.Unsubscribe<PlayerAttackStarted>(OnAttackStarted);
            _eventBus.Unsubscribe<PlayerAttackFinished>(OnAttackFinished);
            _eventBus.Unsubscribe<PlayerMovement>(OnMoved);
            _eventBus.Unsubscribe<PlayerDamagedEvent>(OnDamaged);
            _eventBus.Unsubscribe<PlayerHurtAnimStart>(OnHurtStarted);
            _eventBus.Unsubscribe<PlayerHurtAnimEnd>(OnHurtEnded);
            _eventBus.Unsubscribe<PlayerUpdateMoveStateView>(OnUpdateMoveState);
            _eventBus.Unsubscribe<PlayerUpdateActionStateView>(OnUpdateActionState);
        }
    }
}
