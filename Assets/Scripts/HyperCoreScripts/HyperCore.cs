using UnityEngine.Events;

namespace HyperCoreScripts
{
    public class HyperCore
    {
        internal static HyperEvent BeginFrame { get; } = new HyperEvent();
        internal static HyperEvent UpdateFrame { get; } = new HyperEvent();
        internal static HyperEvent EndFrame { get; } = new HyperEvent();
        
        public static long CurrentFrame { get; internal set; }
        public static float DeltaTime { get; internal set; }

        public enum UpdateType
        {
            Early,
            Default,
            Late
        }

        public static void ConnectFrameUpdate(UnityAction<HyperValues> method,
            UpdateType updateType = UpdateType.Default)
        {
            switch (updateType)
            {
                case UpdateType.Early:
                    BeginFrame.AddListener(method);
                    break;
                case UpdateType.Late:
                    EndFrame.AddListener(method);
                    break;
                default:
                    UpdateFrame.AddListener(method);
                    break;
            }
        }
    }
}