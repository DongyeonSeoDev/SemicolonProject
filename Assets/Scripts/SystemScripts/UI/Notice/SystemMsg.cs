using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Water
{
    public class SystemMsg : MonoBehaviour
    {
        [SerializeField] RectTransform rectTrm;
        [SerializeField] private CanvasGroup cvsg;
        [SerializeField] private Text systemText;
        private float disableTime;

        Vector3 origin, target;

        private void Awake()
        {
            origin = rectTrm.anchoredPosition;
            target = new Vector2(origin.x, origin.y - 300f);
        }

        public void Set(string msg, int fontSize, float existTime)
        {
            systemText.text = msg;
            systemText.fontSize = fontSize;
            cvsg.alpha = 0;
            transform.SetAsLastSibling();
            rectTrm.anchoredPosition = origin;

            disableTime = Time.unscaledTime + existTime;

            cvsg.DOFade(1, 0.35f).SetEase(Ease.OutCirc).SetUpdate(true);
            rectTrm.DOAnchorPos(target, 0.35f).SetEase(Ease.OutBack).SetUpdate(true);
        }

        private void Update()
        {
            if(disableTime < Time.unscaledTime)
            {
                rectTrm.DOAnchorPos(origin, 0.35f).SetEase(Ease.InBack).SetUpdate(true);
                cvsg.DOFade(0, 0.35f).SetEase(Ease.InCirc).SetUpdate(true).OnComplete(()=> gameObject.SetActive(false));
                disableTime = Time.unscaledTime + 3;
            }
        }
    }
}
