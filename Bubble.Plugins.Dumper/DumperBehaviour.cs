using System;
using System.IO;
using BepInEx;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bubble.Plugins.Dumper;

public class DumperBehaviour : MonoBehaviour
{
    private bool _isDumping;
    
    public DumperBehaviour(System.IntPtr ptr) : base(ptr) { }

    public static void Setup()
    {
        ClassInjector.RegisterTypeInIl2Cpp<DumperBehaviour>();
        
        GameObject obj = new("DumperBehaviour");
        DontDestroyOnLoad(obj);
        
        obj.hideFlags = HideFlags.HideAndDontSave;
        obj.AddComponent<DumperBehaviour>();
    }

    private void Update()
    {
        if (Keyboard.current.f1Key.isPressed && !_isDumping)
        {
            _isDumping = true;
            Console.WriteLine("Starting dump...");
            DumpProtocol();
            Console.WriteLine("Dump finished!");
            _isDumping = false;
        }
    }
    
    private void DumpProtocol()
    {
        var baseDir = Paths.GameRootPath + "/Protocol";
        Directory.CreateDirectory(baseDir);
        
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var ass in assemblies)
        {
            Plugin.Log.LogInfo(ass.FullName);
            
            if (ass.FullName == null || !ass.FullName.StartsWith("Ankama.Dofus.Protocol"))
                continue;
    
            var types = ass.GetTypes();
            foreach (var t in types)
            {
                if (!t.Name.EndsWith("Reflection"))
                    continue;
        
                // once we have the type we get the static Field "Descriptor"
                var descriptorProperty = t.GetProperty("Descriptor");

                if (descriptorProperty == null)
                    continue;

                var descriptor = descriptorProperty.GetValue(null);
                
                if(descriptor == null)
                    continue;
                
                var descriptorType = descriptor.GetType();
        
                var protoProperty = descriptorType.GetProperty("Proto");
                
                if(protoProperty == null)
                    continue;
                
                var proto = protoProperty.GetValue(descriptor);
        
                // we invoke the method "ToString()"
                if (proto == null)
                    continue;
                
                var toStringMethod = proto.GetType().GetMethod("ToString");

                if (toStringMethod == null)
                    continue;
                
                var res = (string)toStringMethod.Invoke(proto, [])!;

                File.WriteAllText(Path.Combine(baseDir, (t.FullName + ".json")), res);
            } 
        }
    }

}