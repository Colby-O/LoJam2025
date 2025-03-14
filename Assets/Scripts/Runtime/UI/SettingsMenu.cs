using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace LoJam
{
    public class SettingsMenu : View
    {
        [SerializeField] private RectTransform _settingsPanel;
        [SerializeField] private Button _backButton;

        private Vector2 hiddenPosition = new Vector2(0, -Screen.height); // Offscreen, below
        private Vector2 visiblePosition = Vector2.zero; // On-screen, centered

        private void Start()
        {
            Hide();
            _settingsPanel.anchoredPosition = hiddenPosition;
            _backButton.onClick.AddListener(OnBackClicked);
        }

        private void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame) OnBackClicked();
        }

        private void OnBackClicked()
        {
            LoJamGameManager.GetMonoSystem<IUIMonoSystem>().PopView();
        }

        public override void OnPush()
        {
            _settingsPanel.DOAnchorPos(visiblePosition, 0.5f).SetEase(Ease.OutExpo);
        }

        public override void OnPop()
        {
            _settingsPanel.anchoredPosition = hiddenPosition;
        }
    }
}
