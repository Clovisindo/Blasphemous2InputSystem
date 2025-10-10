using Game.Core.Installers;
using Game.Settings;
using System;
using UnityEngine;

namespace Game.Core
{
    public class Bootstrapper : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]public PlayerSettingsSO playerSettings;
        
        public PlayerInputActions inputActionsAsset;
        public static IContainer Container { get; private set; }
        public static event Action<PlayerInputActions> OnCoreInitialized;

        void Awake()
        {
            Debug.Log("[Bootstrapper] Iniciando núcleo del juego...");

            Container = new SimpleContainer();

            Container.RegisterSingleton<PlayerSettingsSO>(playerSettings);

            inputActionsAsset = new PlayerInputActions();
            inputActionsAsset.Enable();

            EventInstaller.Install(Container);
            InputInstaller.Install(Container,inputActionsAsset);
            GameplayInstaller.Install(Container);

            Debug.Log("[Bootstrapper] Núcleo inicializado correctamente");

            // Notificar inicialización
            OnCoreInitialized?.Invoke(inputActionsAsset);
        }
    }
}
