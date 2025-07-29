using System;
using JetBrains.Annotations;
using UnityEngine;

namespace DevConsole.Window
{
    public class DevConsoleController : MonoBehaviour
    {
        [SerializeField] private KeyCode _consoleOpenKeycode = KeyCode.Tilde;
        [SerializeField] private GameObject _content;

        public static event Action OnOpen;
        public static event Action OnClose;
        
        internal bool IsOpened => _content.activeSelf;
        
#if UNITY_EDITOR
        private void Reset()
        {
            if (_content == null)
            {
                for (int i = 0, c = transform.childCount; i < c; i++)
                {
                    Transform child = transform.GetChild(i);
                    if (child.name.Equals("Content"))
                    {
                        _content = child.gameObject;
                        break;
                    }
                }
            }
        }
#endif
        
        private void Start()
        {
            if (IsOpened)
            {
                _content.SetActive(false);
            }
        }

        private void Update()
        {
            if (IsOpened)
            {
                return;
            }

            if (Input.GetKeyDown(_consoleOpenKeycode))
            {
                Open();
            }
        }
        
        [ContextMenu(nameof(Open))]
        private void Open()
        {
            if (IsOpened == false)
            {
                _content.SetActive(true);
                OnOpen?.Invoke();
            }
        }

        [ContextMenu(nameof(Close))]
        [UsedImplicitly]
        public void Close()
        {
            if (IsOpened)
            {
                _content.SetActive(false);
                OnClose?.Invoke();
            }
        }
    }
}