using TMPro;
using UnityEngine;

namespace LoJam
{
    public class UITImer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _time;

        private void Update()
        {
            _time.text = LoJamGameManager.GetFormattedTime();
        }
    }
}
