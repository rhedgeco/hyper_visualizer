using System;
using System.Collections;
using System.IO;
using UMod;

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
        }
    }
}