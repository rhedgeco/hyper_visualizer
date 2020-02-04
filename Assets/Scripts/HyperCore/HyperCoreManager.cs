using UnityEngine;

namespace HyperCore
{
    public class HyperCoreManager : MonoBehaviour
    {
        public static HyperCore Core { get; } = new HyperCore();

        public static long CurrentFrame { get; private set; }

        public static void PushFrame(long frameNumber)
        {
            if (frameNumber == CurrentFrame) return;
            Core.UpdateFrame.Invoke(new HyperValues(0f,0f,new float[2],1f));
            CurrentFrame = frameNumber;
        }
    }
}