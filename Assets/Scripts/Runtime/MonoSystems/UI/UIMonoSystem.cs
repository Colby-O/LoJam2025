using LoJam.Core;
using UnityEngine;
using System.Collections.Generic;

namespace LoJam
{

    public class UIMonoSystem : MonoBehaviour, IUIMonoSystem
    {
        [SerializeField] private Stack<View> _viewStack = new Stack<View>();
        [SerializeField] private Dictionary<string, View> _views = new Dictionary<string, View>();

        public void PushView(View view)
        {
            _viewStack.Push(view);
            view.Show();

            view.transform.SetAsLastSibling();
        }

        public void PopView()
        {
            if (_viewStack.Count == 0)
                return;

            View topView = _viewStack.Pop();
            topView.Hide();

            if (_viewStack.Count > 0)
            {
                View nextView = _viewStack.Peek();
                nextView.transform.SetAsLastSibling();
            }
        }

        public View GetTopView() => _viewStack.Count > 0 ? _viewStack.Peek() : null;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}
