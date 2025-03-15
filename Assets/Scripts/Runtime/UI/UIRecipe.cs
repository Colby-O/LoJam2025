using TMPro;
using UnityEngine;

namespace LoJam
{
    public class UIRecipe : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void SetText(string text)
        {
            _text.text = text;
        }
    }
}
