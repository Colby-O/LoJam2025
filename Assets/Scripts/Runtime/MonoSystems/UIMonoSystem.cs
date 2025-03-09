using LoJam.Core;
using UnityEngine;
using System.Collections.Generic;

namespace LoJam
{

    public class UIMonoSystem : MonoBehaviour, IUIMonoSystem
    {
        [SerializeField] private List<UIElement> _uiElements;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _uiElements = new List<UIElement>(FindObjectsByType<UIElement>(FindObjectsSortMode.None));
        }

        // Update is called once per frame
        void Update()
        {

        }

        public UIElement GetElementByName(string name)
        {
            return _uiElements.Find(element => element.name == name);
        }
    }
}
