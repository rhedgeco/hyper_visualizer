using System;
using System.Threading;
using UnityEngine;

namespace HyperScripts.Threading
{
    public abstract class GeneralThreadWorker
    {
        public enum WorkerState
        {
            Idle,
            Running,
            WaitingClose,
            CloseError
        }

        public WorkerState State { get; private set; } = WorkerState.Idle;
        protected string StateMessage { get; set; } = "";
        protected float StateProgress { get; set; } = 0f;

        internal abstract void ThreadCallbackStart();

        protected abstract void ThreadCallbackUpdate();

        protected abstract void ThreadCallbackError();

        protected abstract void ThreadCallbackClosed();
        
        protected abstract void ThreadBody();

        internal void UpdateWorkerMainState()
        {
            switch (State)
            {
                case WorkerState.Idle:
                    break;
                case WorkerState.Running:
                    ThreadCallbackUpdate();
                    break;
                case WorkerState.WaitingClose:
                    ThreadCallbackClosed();
                    HyperThreadDispatcher.DetachWorker(this);
                    break;
                case WorkerState.CloseError:
                    ThreadCallbackError();
                    HyperThreadDispatcher.DetachWorker(this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal void ThreadRunnerWrapper()
        {
            try
            {
                State = WorkerState.Running;
                StateProgress = 0f;
                ThreadBody();
                StateProgress = 1f;
                State = WorkerState.WaitingClose;
            }
            catch (Exception e)
            {
                Debug.LogError($"Thread {Thread.CurrentThread.Name} closed prematurely, ERROR: {e.Message}");
                State = WorkerState.CloseError;
            }
        }
    }
}