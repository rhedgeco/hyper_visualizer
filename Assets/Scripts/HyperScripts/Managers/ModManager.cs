using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UMod;
using UMod.Scripting;

namespace HyperScripts.Managers
{
    public static class ModManager
    {
        public static List<Type> loadedHyperTypes;

        internal static IEnumerator LoadModAsync(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Could not open file.");

            Uri uri = new Uri($"file:///{path}");

            OverlayManager.Loading.StartLoading($"Loading Visualizer\n{Path.GetFileName(path)}");
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
            if (!request.IsSuccessful) yield break;

            ModHost host = request.Result;
            // TODO: Figure out how to load mods effectively
        }
    }
}