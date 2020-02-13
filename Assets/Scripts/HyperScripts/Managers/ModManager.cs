using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using hyper_engine;
using UMod;
using UMod.Scripting;
using UMod.Scripting.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

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
            HyperCoreRuntime.coreChange.PurgeUpdates();
            ModAsyncOperation<ModHost> request = Mod.LoadAsync(uri);
            while (!request.IsDone)
            {
                OverlayManager.Loading.UpdateLoading(request.Progress);
                yield return null;
            }

            OverlayManager.Loading.EndLoading();

            StatusManager.UpdateStatus(!request.IsSuccessful
                ? "Error loading Visualizer."
                : $"Visualizer loaded {Path.GetFileNameWithoutExtension(path)}");

            if (!request.IsSuccessful) SceneManager.LoadScene(1);
        }
    }
}