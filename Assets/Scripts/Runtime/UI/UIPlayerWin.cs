using DG.Tweening;
using LoJam.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace LoJam
{
    public class UIPlayerWin : View
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        [SerializeField] private RectTransform _transform;
        [SerializeField] private Image _image;

        [SerializeField] private Sprite _leftWinSprite;
        [SerializeField] private Sprite _rightWinSprite;

        private Vector2 hiddenPosition = new Vector2(0, -Screen.height / 2.0f); // Offscreen, below
        private Vector2 visiblePosition = Vector2.zero; // On-screen, centered

        void Start()
        {
            Hide();
            _transform.anchoredPosition = hiddenPosition;
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public override void OnPush()
        {
            _transform.DOAnchorPos(visiblePosition, 1.0f).SetEase(Ease.OutBack);
        }

        public void SetWinner(Side side)
        {
            _image.sprite = side == Side.Left ? _leftWinSprite : _rightWinSprite;
        }
    }
}
