using UnityEngine;

namespace Game.Application
{
    public class FloatingTextFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target;     // el jugador
        [SerializeField] private Vector3 _offset = new(0, 2f, 0); // altura sobre el jugador
        private Camera _mainCamera;

        void Start()
        {
            _mainCamera = Camera.main;
        }

        void LateUpdate()
        {
            if (_target == null) return;

            // Seguir al jugador
            transform.position = _target.position + _offset;

            // Mirar hacia la cámara (opcional si es 3D)
            transform.rotation = _mainCamera.transform.rotation;
        }
    }
}
