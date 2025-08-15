using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    static NewBehaviourScript()
    {
        // Redirect Console output to Unity's Debug.Log
        Console.SetOut(new DebugLogWriter() { IsError = false });
        Console.SetError(new DebugLogWriter() { IsError = true });
    }

    // Start is called before the first frame update
    void Start()
    {
        var exitCode = netstd21.NetStd21.Test();
        Debug.Log($"Exit: {exitCode}");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private sealed class DebugLogWriter : TextWriter
    {
        public bool IsError { get; init; } = false;
        public Action<object> Print => IsError ? Debug.LogError : Debug.Log;
        public override Encoding Encoding => Encoding.UTF8;
        public override void WriteLine(string value) => Print(value);
        public override void Write(string value) => Print(value);
    }
}
