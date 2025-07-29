using UnityEngine;

namespace DevConsole.Window.Flex
{
    public abstract class DevConsoleWindowFlexBase : MonoBehaviour
    {
        [SerializeField] protected Canvas _parentCanvas;

        [Space] 
        [SerializeField] protected bool _allow = true;
        [SerializeField] protected bool _resetOnOpen = true;

        protected RectTransform Window { get; private set; }
        
        private static Canvas FindParentCanvas(Transform current)
        {
            for (int i = 0; i < 100; i++)
            {
                Canvas canvas = current.GetComponent<Canvas>();
                if (canvas != null)
                {
                    return canvas;
                }

                current = current.parent;
            }
            
            return null;
        }
        
#if UNITY_EDITOR
        private void Reset()
        {
            if (_parentCanvas == null)
            {
                _parentCanvas = FindParentCanvas(transform);
            }
        }
#endif

        protected virtual void Awake()
        {
            if (_parentCanvas == null)
            {
                _parentCanvas = FindParentCanvas(transform);
            }
            
            Window = GetComponent<RectTransform>();
        }
    }
}