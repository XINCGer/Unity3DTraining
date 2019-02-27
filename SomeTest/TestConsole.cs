using UnityEngine;
using UnityEngine.UI;

class TestConsole : MonoBehaviour
{
    public Text TestTxt;
    private string mContent;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    /// <summary>
    /// Records a log from the log callback.
    /// </summary>
    /// <param name="message">Message.</param>
    /// <param name="stackTrace">Trace of where the message came from.</param>
    /// <param name="type">Type of message (error, exception, warning, assert).</param>
    void HandleLog(string message, string stackTrace, LogType type)
    {
        if (type == LogType.Exception || type == LogType.Error)
        {
            mContent += message + "\n" + stackTrace + "\n";
            show();
        }
    }

    private void show()
    {
        TestTxt.text = mContent;
    }
}