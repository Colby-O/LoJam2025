using LoJam.Core;
using LoJam.MonoSystem;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

namespace LoJam.Logic
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        private void FitOrthoCamera()
        {
            float aspect = (float)Screen.width / Screen.height;
            _camera.orthographicSize = (GameManager.GetMonoSystem<IGridMonoSystem>().GetBounds().y + GameManager.GetMonoSystem<IGridMonoSystem>().GetTileSize().y / 2f) / aspect;

            float targetWidth = (GameManager.GetMonoSystem<IGridMonoSystem>().GetBounds().x + GameManager.GetMonoSystem<IGridMonoSystem>().GetTileSize().x / 2f) / 2f;
            float currentWidth = _camera.orthographicSize * aspect;

            if (currentWidth < targetWidth) _camera.orthographicSize = targetWidth / aspect;
        } 

        private void Start()
        {
            if (_camera == null) _camera = GetComponent<Camera>();

            _camera.transform.position = new Vector3
            (
                (GameManager.GetMonoSystem<IGridMonoSystem>().GetBounds().x / 2f) * GameManager.GetMonoSystem<IGridMonoSystem>().GetTileSize().x - GameManager.GetMonoSystem<IGridMonoSystem>().GetTileSize().x / 2f, 
                (GameManager.GetMonoSystem<IGridMonoSystem>().GetBounds().y / 2f) * GameManager.GetMonoSystem<IGridMonoSystem>().GetTileSize().y - GameManager.GetMonoSystem<IGridMonoSystem>().GetTileSize().y / 2f,
                -10
            );

            FitOrthoCamera();
        }
    }
}
