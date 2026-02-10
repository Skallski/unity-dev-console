using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DevConsole.Window.Flex
{
    public class DevConsoleWindowRepositionHandler : DevConsoleWindowFlexBase
    {
        private readonly bool _allowReposition = true;
        private readonly bool _resetPositionOnOpen = true;
        
        private bool _isDraggable;
        private Vector2 _dragBounds;
        private Vector2 _defaultPosition;

        protected override void Awake()
        {
            base.Awake();
            
            _defaultPosition = Window.anchoredPosition;
        }

        private void OnEnable()
        {
            if (_resetPositionOnOpen)
            {
                float offsetWidth = _parentCanvas.pixelRect.width / 4;
                Window.offsetMin = new Vector2(offsetWidth, Window.offsetMin.y);
                Window.offsetMax = new Vector2(-offsetWidth, Window.offsetMax.y);
                Window.anchoredPosition = new Vector2(Window.anchoredPosition.x, _defaultPosition.y);
            }
        }

        private void Start()
        {
            _dragBounds = _parentCanvas.pixelRect.size * 0.5f;
        }

        [UsedImplicitly]
        public void OnPointerDown()
        {
            _isDraggable = true;
        }

        [UsedImplicitly]
        public void OnPointerUp()
        {
            _isDraggable = false;
        }

        [UsedImplicitly]
        public void OnDrag(BaseEventData eventData)
        {
            if (_allowReposition == false)
            {
                return;
            }
            
            if (_isDraggable)
            {
                if (eventData is PointerEventData pointerEventData)
                {
                    Window.anchoredPosition += pointerEventData.delta / _parentCanvas.scaleFactor;
                }
            }

            Vector2 pos = Window.anchoredPosition;

            Window.anchoredPosition = new Vector2(
                Mathf.Clamp(pos.x, -_dragBounds.x, _dragBounds.x),
                Mathf.Clamp(pos.y, -_dragBounds.y, _defaultPosition.y)
            );
        }
    }
}