using Game.Core;
using Game.Core.Orchestrator;
using Game.Events;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Game.Events.PlayerEvents.PlayerEvents;

namespace Game.Application.Enemies
{
    public class Enemy : MonoBehaviour, ICoreDependent
    {
        private Dictionary<Guid, float> _nextAllowedHit = new();
        IEventBus _eventBus;
        public int damage = 10;
        public float stun = 0.5f;
        public float damageInterval = 0.5f;
        public float time_last_damage = 0f;
        

        private void Awake()
        {
            CoreOrchestrator.Register(this);
        }

        public void OnCoreReady()
        {
            _eventBus = Bootstrapper.Container.Resolve<IEventBus>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var view = other.GetComponent<PlayerView>();
            if (view == null) { return; }

            InvokeDamageIntent(view);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            time_last_damage += Time.deltaTime;
            var view = other.GetComponent<PlayerView>();
            if (view == null) { return; }

            //control para si tuvieramos otros jugadores o daño de fuego amigo entre enemigos
            var id = view.GetPlayerId();
            if (!_nextAllowedHit.TryGetValue(id, out var nextTime))
                nextTime = 0;

            if (time_last_damage >= nextTime)
            {
                InvokeDamageIntent(view);

                _nextAllowedHit[id] = damageInterval;
                time_last_damage = 0f;
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            var view = other.GetComponent<PlayerView>();
            if (view != null)
                _nextAllowedHit.Remove(view.GetPlayerId());
        }

        /// <summary>
        /// calculos necesarios e invocacion de evento damageIntent
        /// </summary>
        /// <param name="view"></param>
        private void InvokeDamageIntent(PlayerView view)
        {
            var direction = (view.transform.position - transform.position).normalized;
            var intent = new DamageIntentEvent(view.GetPlayerId(), damage, stun, direction, gameObject);//editando este evento y extendiendo los enemigos, podriamos hacer distintos tipos de daño, resistencias en dominio segun el daño,etc
            _eventBus.Publish(intent);
        }
    }
}
