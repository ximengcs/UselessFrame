
using UnityEngine;
using UnityEngine.EventSystems;

namespace QFSW.QC.FloatButtons
{
    public class RectMoveController : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private RectTransform target;

        [SerializeField]
        private RectTransform canvasTf;

        private Vector2 _lastPos;
        private Vector2 _orgPos;
        private bool _draging;

        private void Start()
        {
            _orgPos = target.anchoredPosition;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _lastPos = eventData.position;
            _draging = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 localPoint = eventData.position - _lastPos;
            localPoint.x = 0;
            localPoint.y *= canvasTf.sizeDelta.y / Screen.height;
            localPoint += target.anchoredPosition;
            localPoint.y = Mathf.Max(localPoint.y, _orgPos.y);
            target.anchoredPosition = localPoint;
            _lastPos = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _draging = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_draging)
            {
                target.anchoredPosition = _orgPos;
            }
        }
    }
}
