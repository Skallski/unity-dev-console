using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DevConsole.Window.Flex
{
    public class DevConsoleWindowResizeHandler : DevConsoleWindowFlexBase
    {
        private bool _isResizable;
        private Vector2 _onPtrDownMousePos;
        private Vector2 _onPtrDownWindowPos;
        private Vector2 _onPtrDownWindowSize;
        
        private Vector2 _windowScale;
        private Vector2 _windowSizeMax;

        protected override void Awake()
        {
            base.Awake();

            _windowScale = Window.localScale;

            Rect canvasRect = ((RectTransform)_parentCanvas.transform).rect;
            _windowSizeMax = new Vector2(
                canvasRect.width / _windowScale.x, 
                (canvasRect.height / _windowScale.y) - Window.anchoredPosition.y
            );
        }

        private void OnEnable()
        {
            if (_resetOnOpen)
            {
                Window.sizeDelta = _windowSizeMax;
            }
        }

        [UsedImplicitly]
        public void OnPointerDown(BaseEventData eventData)
        {
            if (_allow == false)
            {
                return;
            }
            
            _isResizable = true;
            if (eventData is PointerEventData pointerEventData)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _parentCanvas.transform as RectTransform, 
                    pointerEventData.position, 
                    _parentCanvas.worldCamera, 
                    out _onPtrDownMousePos
                );

                Rect windowRect = Window.rect;
                _onPtrDownWindowSize = new Vector2(windowRect.width, windowRect.height);
                
                _onPtrDownWindowPos = Window.anchoredPosition;
            }
        }
        
        [UsedImplicitly]
        public void OnPointerUp()
        {
            if (_allow == false)
            {
                return;
            }
            
            _isResizable = false;
        }

        [UsedImplicitly]
        public void OnDrag(BaseEventData eventData)
        {
            if (_allow == false)
            {
                return;
            }
            
            if (_isResizable && eventData is PointerEventData pointerEventData)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _parentCanvas.transform as RectTransform, 
                    pointerEventData.position, 
                    _parentCanvas.worldCamera, 
                    out Vector2 localMousePosition
                );

                Vector2 dragDelta = (localMousePosition - _onPtrDownMousePos) / _windowScale;
                Vector2 newSize = _onPtrDownWindowSize - dragDelta;

                Vector2 newSizeClamped = new Vector2(
                    Mathf.Clamp(newSize.x, _windowSizeMax.x * 0.5f, _windowSizeMax.x),
                    Mathf.Clamp(newSize.y, _windowSizeMax.y * 0.5f, _windowSizeMax.y * 1.5f)
                );

                Window.sizeDelta = newSizeClamped;
     
                float heightDelta = newSizeClamped.y - _onPtrDownWindowSize.y;
                Window.anchoredPosition = new Vector2(_onPtrDownWindowPos.x, _onPtrDownWindowPos.y - heightDelta);
            }
        }
    }
}