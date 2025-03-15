using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using LoJam.Logic;
using LoJam;
using LoJam.MonoSystem;

public class UIPusher : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _daemonCountText; 

    [SerializeField] private List<Image> _arrowImages; 
    private FirewallController _firewallController;
    private const int _maxArrows = 4; 

    private int leftCount = 0;
    private int rightCount = 0;
 
    private readonly int[] _thresholds = { 0, 1, 2, 3, 4 };

    public void UpdateDameonText()
    {
        if (_firewallController != null)
        {
            leftCount = _firewallController.GetDaemonCount(Side.Left);
            rightCount = _firewallController.GetDaemonCount(Side.Right);
            //_daemonCountText.text = $"Left: {leftCount} | Right: {rightCount}";
        }

        int difference = rightCount - leftCount; 

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
                _arrowImages[i].GetComponent<Image>().enabled = true;
                _arrowImages[i].rectTransform.localRotation = Quaternion.Euler(0, 0, difference > 0 ? 180 : 0); 
            }
            else
            {
                _arrowImages[i].GetComponent<Image>().enabled = false;
                
            }
        }
    }

    private void Start()
    {
        _firewallController = LoJamGameManager.GetMonoSystem<IGridMonoSystem>().GetFirewallController();
        _firewallController.OnDameonChange.AddListener(UpdateDameonText);

        _daemonCountText.text = $"Left: 0 | Right: 0";
    }

    private void Update()
    {
        /*
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
        */
    }
}
