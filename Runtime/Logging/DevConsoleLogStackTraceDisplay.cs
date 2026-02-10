using JetBrains.Annotations;
using UnityEngine;

namespace DevConsole.Logging
{
    public class DevConsoleLogStackTraceDisplay : MonoBehaviour
    {
        [SerializeField] private RectTransform _stackTraceTransform;
        [SerializeField] private TMPro.TextMeshProUGUI _stackTraceLabel;
        [SerializeField] private UnityEngine.UI.ScrollRect _scrollRect;
        [SerializeField] private GameObject _copyToClipboardButtonObj;

        private DevConsoleLogData _selectedLog;

        private void OnEnable()
        {
            DevConsoleLogger.OnClear += ClearStackTrace;
            DevConsoleLogLabel.OnLogClick += OnLogClick;
        }
        
        private void OnDisable()
        {
            DevConsoleLogger.OnClear -= ClearStackTrace;
            DevConsoleLogLabel.OnLogClick -= OnLogClick;
        }

        private void OnLogClick(DevConsoleLogData data)
        {
            if (data == null)
            {
                return;
            }

            _selectedLog?.SetSelected(false);

            if (data == _selectedLog)
            {
                _selectedLog = null;

                ClearStackTrace();
            }
            else
            {
                data.SetSelected(true);
                _selectedLog = data;

                ShowStackTrace();
            }
        }

        private void ShowStackTrace()
        {
            _stackTraceTransform.gameObject.SetActive(true);
            _stackTraceLabel.SetText(_selectedLog?.StackTrace ?? string.Empty);

            _scrollRect.verticalNormalizedPosition = 1f;
            
            _copyToClipboardButtonObj.SetActive(true);
        }

        private void ClearStackTrace()
        {
            _stackTraceLabel.SetText(string.Empty);
            _stackTraceTransform.gameObject.SetActive(false);
            
            _copyToClipboardButtonObj.SetActive(false);
        }

        [UsedImplicitly]
        public void CopyStackTraceToClipboard()
        {
            if (_selectedLog == null)
            {
                return;
            }
            
            GUIUtility.systemCopyBuffer = $"{_selectedLog.FormattedMessage}\n\n{_selectedLog.StackTrace}";
            Debug.Log("Log copied to clipboard.");
        }
    }
}