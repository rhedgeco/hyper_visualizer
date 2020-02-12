using System;
using System.Collections;
using System.IO;
using SFB;
using UMod;
using UnityEngine;

namespace HyperScripts.Managers
{
    public static class ModManager
    {
        internal static IEnumerator LoadModAsync(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Could not open file.");
            
            Uri uri = new Uri($"file:///{path}");
            
            OverlayManager.Loading.StartLoading($"Loading Visualizer\n{Path.GetFileName(path)}");
            ModAsyncOperation<ModHost> host = Mod.LoadAsync(uri);

            while (!host.IsDone)
            {
                OverlayManager.Loading.UpdateLoading(host.Progress);
                yield return null;
            }
            
            OverlayManager.Loading.EndLoading();

            StatusManager.UpdateStatus(!host.IsSuccessful
                ? "Error loading Visualizer."
                : $"Visualizer loaded {Path.GetFileNameWithoutExtension(path)}");
        }
    }
}