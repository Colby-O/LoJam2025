using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace LoJam
{
    public class SettingsMenu : View
    {
        [SerializeField] private RectTransform _settingsPanel;
        [SerializeField] private Button _backButton;

        private Vector2 hiddenPosition = new Vector2(0, -Screen.height); // Offscreen, below
        private Vector2 visiblePosition = Vector2.zero; // On-screen, centered

        void Start()
        {
            Hide();
            _settingsPanel.anchoredPosition = hiddenPosition;
            _backButton.onClick.AddListener(OnBackClicked);
        }

        private void OnBackClicked()
        {
            LoJamGameManager.GetMonoSystem<IUIMonoSystem>().PopView();
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
