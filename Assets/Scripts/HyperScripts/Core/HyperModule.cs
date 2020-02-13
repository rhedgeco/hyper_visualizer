namespace HyperScripts.Core
{
    public abstract class HyperModule
    {
        public virtual void FrameUpdate(HyperValues values)
        {
        }
        
        public virtual void FrameLateUpdate(HyperValues values)
        {
        }
    }
}