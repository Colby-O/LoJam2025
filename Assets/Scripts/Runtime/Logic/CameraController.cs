using LoJam.Core;
using LoJam.MonoSystem;
using UnityEngine;

namespace LoJam.Logic
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private float _padding = 1f;
        private void FitOrthoCamera()
        {
            float targetHeight= GameManager.GetMonoSystem<IGridMonoSystem>().GetBounds().y / 2f;
            float targetWidth = GameManager.GetMonoSystem<IGridMonoSystem>().GetBounds().x / 2f;

            float requiredSize = Mathf.Max(targetHeight, (targetWidth / _camera.aspect));

            _camera.orthographicSize = requiredSize * _padding;
            GameManager.GetMonoSystem<IGridMonoSystem>().FillBackground(_camera);
        } 

        private void Start()
        {
            if (_camera == null) _camera = GetComponent<Camera>();

            _camera.transform.position = new Vector3
            (
                (GameManager.GetMonoSystem<IGridMonoSystem>().GetBounds().x / 2f), 
                (GameManager.GetMonoSystem<IGridMonoSystem>().GetBounds().y / 2f),
                -10
            );

            FitOrthoCamera();
        }

        private void Update()
        {
        }
    }
}
