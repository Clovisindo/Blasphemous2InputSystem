using Game.Core.Installers;
using Game.Core.Orchestrator;
using Game.Domain.Entities;
using Game.Events;
using Game.Settings;
using System;
using UnityEngine;
using static Game.Events.PlayerEvents.PlayerEvents;

namespace Game.Core
{
    public class Bootstrapper : MonoBehaviour
    {
        public PlayerInputActions inputActionsAsset;
        public static IContainer Container { get; private set; }

        void Awake()
        {
            Debug.Log("[Bootstrapper] Iniciando núcleo del juego...");

            Container = new SimpleContainer();

            inputActionsAsset = new PlayerInputActions();
            inputActionsAsset.Enable();

            EventInstaller.Install(Container);
            InputInstaller.Install(Container,inputActionsAsset);
            GameplayInstaller.Install(Container);

            Debug.Log("[Bootstrapper] Núcleo inicializado correctamente");

            // Notificar inicialización
            CoreOrchestrator.NotifyCoreReady();
        }
    }
}
