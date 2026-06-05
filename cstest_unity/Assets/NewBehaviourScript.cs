using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void WebSocketTest_Connect(string url);
#endif

    static NewBehaviourScript()
    {
        // Redirect Console output to Unity's Debug.Log
        Console.SetOut(new DebugLogWriter() { IsError = false });
        Console.SetError(new DebugLogWriter() { IsError = true });
    }

    // Start is called before the first frame update
    void Start()
    {
        const string url = "ws://127.0.0.1:8080/ws";

#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log($"WebSocketTest_Connect: {url}");
        WebSocketTest_Connect(url);
#else
        Debug.Log($"WebSocketTest is implemented by WebGL jslib. Build as WebGL to connect to {url}");
#endif
    }

    private sealed class DebugLogWriter : TextWriter
    {
        public bool IsError { get; init; } = false;
        public Action<object> Print => IsError ? Debug.LogError : Debug.Log;
        public override Encoding Encoding => Encoding.UTF8;
        public override void WriteLine(string value) => Print.Invoke(value);
        public override void Write(string value) => Print.Invoke(value);
    }

}
