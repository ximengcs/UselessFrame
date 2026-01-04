using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace QFSW.QC.FloatButtons
{
    public class FloatButton : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {

        private RectTransform popupTransform;

        // Background image that will change color to indicate an alert
        private Image backgroundImage;

        // Canvas group to modify visibility of the popup
        private CanvasGroup canvasGroup;

        private QuantumConsole console;

#pragma warning disable 0649
        [SerializeField]
        private bool popupAvoidsScreenCutout;

        [SerializeField]
        private RectTransform canvasTf;

        [SerializeField]
        private TextMeshProUGUI newInfoCountText;
        [SerializeField]
        private TextMeshProUGUI newWarningCountText;
        [SerializeField]
        private TextMeshProUGUI newErrorCountText;

        [SerializeField]
        private Color alertColorInfo;
        [SerializeField]
        private Color alertColorWarning;
        [SerializeField]
        private Color alertColorError;
#pragma warning restore 0649

        // Number of new debug entries since the log window has been closed
        private int newInfoCount = 0, newWarningCount = 0, newErrorCount = 0;
        private int thisInfoCount = 0, thisWarningCount = 0, thisErrorCount = 0;

        private Color normalColor;

        private bool isPopupBeingDragged = false;
        private Vector2 normalizedPosition;

        // Coroutines for simple code-based animations
        private IEnumerator moveToPosCoroutine = null;

        public bool IsVisible { get; private set; }

        private void Awake()
        {
            popupTransform = (RectTransform)transform;
            backgroundImage = GetComponent<Image>();
            canvasGroup = GetComponent<CanvasGroup>();

            normalColor = backgroundImage.color;
            Vector2 pos = popupTransform.anchoredPosition;
            if (pos.x != 0f || pos.y != 0f)
                normalizedPosition = pos.normalized; // Respect the initial popup position set in the prefab
            else
                normalizedPosition = new Vector2(0.5f, 0f); // Right edge by default
        }

        private void Start()
        {
            console = QuantumConsole.Instance;
            console.OnDeactivate += Show;
            console.OnActivate += Hide;
            Application.logMessageReceivedThreaded += LogHandler;
            ResetValues();
            UpdatePosition(true);
        }

        private void OnDestroy()
        {
            console.OnDeactivate -= Show;
            console.OnActivate -= Hide;
            Application.logMessageReceivedThreaded -= LogHandler;
        }

        private void LogHandler(string condition, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Log:
                    thisInfoCount++;
                    break;

                case LogType.Warning:
                    thisWarningCount++;
                    break;

                default:
                    thisErrorCount++;
                    break;
            }
        }

        private void LateUpdate()
        {
            if (thisInfoCount > 0 || thisWarningCount > 0 || thisErrorCount > 0)
            {
                NewLogsArrived(thisInfoCount, thisWarningCount, thisErrorCount);
                thisInfoCount = 0;
                thisWarningCount = 0;
                thisErrorCount = 0;
            }
        }

        public void NewLogsArrived(int newInfo, int newWarning, int newError)
        {
            if (newInfo > 0)
            {
                newInfoCount += newInfo;
                newInfoCountText.text = newInfoCount.ToString();
            }

            if (newWarning > 0)
            {
                newWarningCount += newWarning;
                newWarningCountText.text = newWarningCount.ToString();
            }

            if (newError > 0)
            {
                newErrorCount += newError;
                newErrorCountText.text = newErrorCount.ToString();
            }

            if (newErrorCount > 0)
                backgroundImage.color = alertColorError;
            else if (newWarningCount > 0)
                backgroundImage.color = alertColorWarning;
            else if (newInfoCount > 0)
                backgroundImage.color = alertColorInfo;
            else
                backgroundImage.color = normalColor;
        }

        private void ResetValues()
        {
            newInfoCount = 0;
            newWarningCount = 0;
            newErrorCount = 0;

            newInfoCountText.text = "0";
            newWarningCountText.text = "0";
            newErrorCountText.text = "0";

            backgroundImage.color = normalColor;
        }

        // A simple smooth movement animation
        private IEnumerator MoveToPosAnimation(Vector2 targetPos)
        {
            float modifier = 0f;
            Vector2 initialPos = popupTransform.anchoredPosition;

            while (modifier < 1f)
            {
                modifier += 4f * Time.unscaledDeltaTime;
                popupTransform.anchoredPosition = Vector2.Lerp(initialPos, targetPos, modifier);

                yield return null;
            }
        }

        // Popup is clicked
        public void OnPointerClick(PointerEventData data)
        {
            if (isPopupBeingDragged)
                return;
            // Hide the popup and show the log window
            if (!console.IsActive)
            {
                console.Activate();
            }
            ResetValues();
        }

        // Hides the log window and shows the popup
        public void Show()
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
            IsVisible = true;

            // Reset the counters
            ResetValues();

            // Update position in case resolution was changed while the popup was hidden
            UpdatePosition(true);
        }

        // Hide the popup
        public void Hide()
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0f;

            IsVisible = false;
            isPopupBeingDragged = false;
        }

        public void OnBeginDrag(PointerEventData data)
        {
            isPopupBeingDragged = true;

            // If a smooth movement animation is in progress, cancel it
            if (moveToPosCoroutine != null)
            {
                StopCoroutine(moveToPosCoroutine);
                moveToPosCoroutine = null;
            }
        }

        // Reposition the popup
        public void OnDrag(PointerEventData data)
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTf, data.position, data.pressEventCamera, out localPoint))
                popupTransform.anchoredPosition = localPoint;
        }

        // Smoothly translate the popup to the nearest edge
        public void OnEndDrag(PointerEventData data)
        {
            isPopupBeingDragged = false;
            UpdatePosition(false);
        }

        // There are 2 different spaces used in these calculations:
        // RectTransform space: raw anchoredPosition of the popup that's in range [-canvasSize/2, canvasSize/2]
        // Safe area space: Screen.safeArea space that's in range [safeAreaBottomLeft, safeAreaTopRight] where these corner positions
        //                  are all positive (calculated from bottom left corner of the screen instead of the center of the screen)
        public void UpdatePosition(bool immediately)
        {
            Vector2 canvasRawSize = canvasTf.rect.size;
            // Calculate safe area bounds
            float canvasWidth = canvasRawSize.x;
            float canvasHeight = canvasRawSize.y;

            float canvasBottomLeftX = 0f;
            float canvasBottomLeftY = 0f;

            if (popupAvoidsScreenCutout)
            {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
                Rect safeArea = Screen.safeArea;

                int screenWidth = Screen.width;
                int screenHeight = Screen.height;

                canvasWidth *= safeArea.width / screenWidth;
                canvasHeight *= safeArea.height / screenHeight;

                canvasBottomLeftX = canvasRawSize.x * (safeArea.x / screenWidth);
                canvasBottomLeftY = canvasRawSize.y * (safeArea.y / screenHeight);
#endif
            }

            // Calculate safe area position of the popup
            // normalizedPosition allows us to glue the popup to a specific edge of the screen. It becomes useful when
            // the popup is at the right edge and we switch from portrait screen orientation to landscape screen orientation.
            // Without normalizedPosition, popup could jump to bottom or top edges instead of staying at the right edge
            Vector2 pos = canvasRawSize * 0.5f + (immediately ? new Vector2(normalizedPosition.x * canvasWidth, normalizedPosition.y * canvasHeight) : (popupTransform.anchoredPosition - new Vector2(canvasBottomLeftX, canvasBottomLeftY)));

            // Find distances to all four edges of the safe area
            float distToLeft = pos.x;
            float distToRight = canvasWidth - distToLeft;

            float distToBottom = pos.y;
            float distToTop = canvasHeight - distToBottom;

            float horDistance = Mathf.Min(distToLeft, distToRight);
            float vertDistance = Mathf.Min(distToBottom, distToTop);

            // Find the nearest edge's safe area coordinates
            Vector2 halfSize = popupTransform.sizeDelta * 0.5f;
            if (horDistance < vertDistance)
            {
                if (distToLeft < distToRight)
                    pos = new Vector2(halfSize.x, pos.y);
                else
                    pos = new Vector2(canvasWidth - halfSize.x, pos.y);
                pos.y = Mathf.Clamp(pos.y, halfSize.y, canvasHeight - halfSize.y);
            }
            else
            {
                if (distToBottom < distToTop)
                    pos = new Vector2(pos.x, halfSize.y);
                else
                    pos = new Vector2(pos.x, canvasHeight - halfSize.y);
                pos.x = Mathf.Clamp(pos.x, halfSize.x, canvasWidth - halfSize.x);
            }

            pos -= canvasRawSize * 0.5f;
            normalizedPosition.Set(pos.x / canvasWidth, pos.y / canvasHeight);

            // Safe area's bottom left coordinates are added to pos only after normalizedPosition's value
            // is set because normalizedPosition is in range [-canvasWidth / 2, canvasWidth / 2]
            pos += new Vector2(canvasBottomLeftX, canvasBottomLeftY);

            // If another smooth movement animation is in progress, cancel it
            if (moveToPosCoroutine != null)
            {
                StopCoroutine(moveToPosCoroutine);
                moveToPosCoroutine = null;
            }

            if (immediately)
                popupTransform.anchoredPosition = pos;
            else
            {
                // Smoothly translate the popup to the specified position
                moveToPosCoroutine = MoveToPosAnimation(pos);
                StartCoroutine(moveToPosCoroutine);
            }
        }
    }
}