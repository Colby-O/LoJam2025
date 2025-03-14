using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using LoJam.Logic;

public class UIPusher : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _daemonCountText; 

    [SerializeField] private List<Image> _arrowImages; 
    [SerializeField] private GameObject _firewallGameObject; 

    private FirewallController _firewallController;
    private const int _maxArrows = 4; 

 
    private readonly int[] _thresholds = { 0, 2, 4, 6, 8 };

    private void Start()
    {
        _firewallController = _firewallGameObject.GetComponent<FirewallController>();

    }

    private void Update()
    {
        if (_firewallController != null)
        {
            int leftCount = _firewallController.GetDaemonCount(Side.Left);
            int rightCount = _firewallController.GetDaemonCount(Side.Right);
            int difference = rightCount - leftCount; 

            
            _daemonCountText.text = $"Left: {leftCount} | Right: {rightCount}";

           
            int arrowCount = 0;
            for (int i = 0; i < _thresholds.Length; i++)
            {
                if (Mathf.Abs(difference) >= _thresholds[i])
                    arrowCount = i;
                else
                    break;
            }

            
            for (int i = 0; i < _arrowImages.Count; i++)
            {
                if (i < arrowCount)
                {
                    _arrowImages[i].gameObject.SetActive(true);
                    _arrowImages[i].rectTransform.localRotation = Quaternion.Euler(0, 0, difference > 0 ? 0 : 180); 
                }
                else
                {
                    _arrowImages[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
