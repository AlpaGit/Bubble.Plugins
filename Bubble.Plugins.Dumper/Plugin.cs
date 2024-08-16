#define CPP

using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;

namespace Bubble.Plugins.Dumper;

[BepInPlugin(GUID, NAME, VERSION)]
public class Plugin : BasePlugin
{
    private const string NAME = "Bubble.Plugins.Dumper";
    private const string VERSION = "1.0.0";
    public const string AUTHOR = "Alpa";
    private const string GUID = "com.bubble.plugins.dumper";

    public static HarmonyLib.Harmony Harmony { get; } = new HarmonyLib.Harmony(GUID);
    private const string IL2CPP_LIBS_FOLDER = "interop";

    new internal static ManualLogSource Log;
    
    public static string UnhollowedModulesFolder
        => Path.Combine(Paths.BepInExRootPath, IL2CPP_LIBS_FOLDER);

    public override void Load()
    {
        Log = base.Log;
        Log.LogInfo($"Plugin {GUID} is loaded!");
        
        AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;

        foreach (var file in Directory.GetFiles(UnhollowedModulesFolder, "*.dll"))
        {
            try
            {
                Assembly.LoadFrom(file);
            }
            catch (Exception e)
            {
                Log.LogError($"Failed to load assembly {file}: {e}");
            }
        }
        
        DumperBehaviour.Setup();
    }

    private void OnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
    {
        Log.LogInfo($"Assembly loaded: {args.LoadedAssembly.FullName}");
    }
}