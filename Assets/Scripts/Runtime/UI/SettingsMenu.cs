using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

namespace LoJam
{
    public class SettingsMenu : View
    {
        [SerializeField] private RectTransform _settingsPanel;

        private Vector2 hiddenPosition = new Vector2(0, -Screen.height); // Offscreen, below
        private Vector2 visiblePosition = Vector2.zero; // On-screen, centered

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Hide();
            _settingsPanel.anchoredPosition = hiddenPosition;
        }

        // Update is called once per frame
        void Update()
        {
        }

        public override void OnPush()
        {
            _settingsPanel.DOAnchorPos(visiblePosition, 0.66f).SetEase(Ease.OutBack);
        }

        public override void OnPop()
        {
            base.OnPush();
        }
    }
}
