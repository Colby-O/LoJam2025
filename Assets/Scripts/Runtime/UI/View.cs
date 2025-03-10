using UnityEngine;

namespace LoJam
{
    public class View : MonoBehaviour
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

        public virtual void Show() => gameObject.SetActive(true);
        public virtual void Hide() => gameObject.SetActive(false);

        public virtual void OnPush() { }
        public virtual void OnPop() { }
    }
}
