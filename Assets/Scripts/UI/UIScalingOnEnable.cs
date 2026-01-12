using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class UIScalingOnEnable : MonoBehaviour
    {
        private void OnEnable()
        {
            transform.DOScale(1f, 0.5f).SetEase(Ease.InOutSine);
        }

        private void OnDisable()
        {
            transform.localScale = Vector3.zero;
        }
    }
}