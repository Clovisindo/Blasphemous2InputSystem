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

            var intent = new DamageIntentEvent(view.GetPlayerId(), damage, stun, gameObject);
            _eventBus.Publish(intent);
        }
    }
}
