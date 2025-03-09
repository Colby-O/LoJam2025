using UnityEngine;
using UnityEngine.UIElements;

namespace LoJam
{
    public class UIElement : MonoBehaviour
    {
        public void SetPosition(Vector2 position)
        {
            if (TryGetComponent<RectTransform>(out var transform))
                transform.anchoredPosition = position;
        }

        public Vector2 GetPosition()
        {
            if (TryGetComponent<RectTransform>(out var transform))
                return transform.anchoredPosition;

            return Vector2.zero;
        }

        public void SetOpacity(float alpha)
        {
            if (TryGetComponent<CanvasGroup>(out var group))
                group.alpha = Mathf.Clamp01(alpha);
        }

        public float GetOpacity()
        {
            if (TryGetComponent<CanvasGroup>(out var group))
                return group.alpha;

            return 0f;
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void EnableInteraction()
        {
            if (TryGetComponent<CanvasGroup>(out var group))
                group.interactable = true;
        }

        public virtual void DisableInteraction()
        {
            if (TryGetComponent<CanvasGroup>(out var group))
                group.interactable = false;
        }

        public virtual void EnableBlock()
        {
            if (TryGetComponent<CanvasGroup>(out var group))
                group.blocksRaycasts = true;
        }

        public virtual void DisableBlock()
        {
            if (TryGetComponent<CanvasGroup>(out var group))
                group.blocksRaycasts = false;
        }
    }
}
