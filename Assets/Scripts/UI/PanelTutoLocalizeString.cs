using Core;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace UI
{
    public class PanelTutoLocalizeString : MonoBehaviour
    {
        [Header("Localize String")]
        [SerializeField] private LocalizeStringEvent _localizeSpriteEventLeft;
        [SerializeField] private LocalizeStringEvent _localizeSpriteEventRight;

        private void OnEnable()
        {
            if (GameManager.Instance.StickPos != 0)
            {
                _localizeSpriteEventLeft.StringReference.SetReference("StringTables", "shoot");
                _localizeSpriteEventRight.StringReference.SetReference("StringTables", "drive");
            }
            else
            {
                _localizeSpriteEventLeft.StringReference.SetReference("StringTables", "drive");
                _localizeSpriteEventRight.StringReference.SetReference("StringTables", "shoot");
            }
            
            _localizeSpriteEventLeft.RefreshString();
            _localizeSpriteEventRight.RefreshString();
        }
    }
}
