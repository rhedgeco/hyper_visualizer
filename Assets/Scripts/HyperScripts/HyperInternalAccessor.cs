using hyper_engine;

namespace HyperScripts
{
    public class HyperInternalAccessor : InternalAccessor
    {
        public void SetTime(float time)
        {
            SetHyperCoreTime(time);
        }

        public void SetTotalTime(float time)
        {
            SetHyperCoreTotalTime(time);
        }

        public void InvokeUpdate(HyperValues values)
        {
            InvokeFrameUpdate(values);
        }
    }
}