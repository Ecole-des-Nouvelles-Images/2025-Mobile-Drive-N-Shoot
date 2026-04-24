using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class UIFirstSelectedButton : MonoBehaviour
    {
        [Header("First Selected Button")]
        [SerializeField] private GameObject _firstSelectedButton;

        public void SelectFirstButton(GameObject firstSelectedButton)
        {
            if (EventSystem.current)
            {
                EventSystem.current.SetSelectedGameObject(firstSelectedButton);
            }
        }

        private void OnEnable()
        {
            StartCoroutine(SelectFirstDelayed());
        }

        private IEnumerator SelectFirstDelayed()
        {
            yield return null;
            if (_firstSelectedButton && EventSystem.current)
            {
                EventSystem.current.SetSelectedGameObject(_firstSelectedButton);
            }
        }
    }
}