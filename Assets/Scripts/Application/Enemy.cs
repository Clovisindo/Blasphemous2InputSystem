using Game.Core;
using Game.Core.Orchestrator;
using Game.Events;
using UnityEngine;
using static Game.Events.PlayerEvents.PlayerEvents;

namespace Game.Application.Enemies
{
    public class Enemy : MonoBehaviour, ICoreDependent
    {
        public int damage = 10;
        public float stun = 0.5f;
        IEventBus _eventBus;

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

            var direction = (view.transform.position - transform.position).normalized;
            var intent = new DamageIntentEvent(view.GetPlayerId(), damage, stun, direction, gameObject);//editando este evento y extendiendo los enemigos, podriamos hacer distintos tipos de daño, resistencias en dominio segun el daño,etc
            _eventBus.Publish(intent);
        }
    }
}
