using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Debugger : EditorWindow
{

    [MenuItem("Window/Debug Window")]
    public static void ShowWindow()
    {
        GetWindow<Debugger>("Debug Window");
    }
}
