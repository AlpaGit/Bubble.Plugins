using System;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Unity.IL2CPP.UnityEngine;
using Il2CppInterop.Runtime.Injection;
using QFSW.QC;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bubble.Plugins.Dumper;

public class LoggingBehaviour : MonoBehaviour
{
    private QuantumConsole? _quantumConsole;
    private bool _isProcessing;
    
    public LoggingBehaviour(IntPtr ptr) : base(ptr) { }

    public static void Setup()
    {
        ClassInjector.RegisterTypeInIl2Cpp<LoggingBehaviour>();
        
        GameObject obj = new("LoggingBehaviour");
        DontDestroyOnLoad(obj);
        
        obj.hideFlags = HideFlags.HideAndDontSave;
        obj.AddComponent<LoggingBehaviour>();
    }

    private void Awake()
    {
        Plugin.Log.LogInfo("LoggingBehaviour is ready!");
    }

    private void GetConsole()
    {
        _quantumConsole = GetGameObjectByName("Quantum Console (SRP)")?.GetComponent<QuantumConsole>();
        
        if (_quantumConsole == null)
        {
            Plugin.Log.LogError("Quantum Console not found!");
            return;
        }

        _quantumConsole.gameObject.SetActive(true);
    }

    private void Update()
    {
        if(_quantumConsole == null)
            GetConsole();
        
        if (Keyboard.current.f2Key.isPressed && !_isProcessing && _quantumConsole != null)
        {
            _isProcessing = true;
            _quantumConsole.InvokeCommand("/loglevel Debug");
            _quantumConsole.InvokeCommand("/sosenable");
            _isProcessing = false;
        }
    }
    
    public static GameObject? GetGameObjectByName(string name)
    {
        return Resources
            .FindObjectsOfTypeAll<GameObject>()
            .FirstOrDefault(go => go.name == name);
    }


}