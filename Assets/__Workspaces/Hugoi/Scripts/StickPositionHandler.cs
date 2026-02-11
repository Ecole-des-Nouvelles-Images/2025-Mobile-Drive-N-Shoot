using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.Game;

namespace __Workspaces.Hugoi.Scripts
{
    public class StickPositionHandler : MonoBehaviour
    {
        [Header("Referencecs")]
        [SerializeField] private Button _buttonLeft;
        [SerializeField] private Button _buttonRight;

        private void Start()
        {
            if (GameManager.Instance.StickPos == 0)
            {
                _buttonLeft.GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
                _buttonRight.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            }
            else
            {
                _buttonRight.GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
                _buttonLeft.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            }
        }

        private void OnEnable()
        {
            _buttonLeft.onClick.AddListener(() => StickPosition(0));
            _buttonRight.onClick.AddListener(() => StickPosition(1));
        }
        
        private void OnDisable()
        {
            _buttonLeft.onClick.RemoveAllListeners();
            _buttonRight.onClick.RemoveAllListeners();
        }

        public void StickPosition(int pos)
        {
            GameManager.Instance.StickPos = pos;

            if (pos == 0)
            {
                _buttonLeft.GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
                _buttonRight.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            }
            else
            {
                _buttonRight.GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
                _buttonLeft.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            }
            
            EventBus.OnChangedStickPos?.Invoke(pos);
        }
    }
}
