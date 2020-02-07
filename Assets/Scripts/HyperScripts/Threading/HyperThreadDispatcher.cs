using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace HyperScripts.Threading
{
    public class HyperThreadDispatcher : MonoBehaviour
    {
        private static HyperThreadDispatcher _instance;
        
        private static List<GeneralThreadWorker> _workers = new List<GeneralThreadWorker>();

        private void Awake()
        {
            // Set up Singleton
            if (_instance == null) _instance = this;
            if (_instance != this) Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            for (int i = _workers.Count - 1; i >= 0; i--)
            {
                _workers[i].UpdateWorkerMainState();
            }
        }

        public static void StartWorker(GeneralThreadWorker worker)
        {
            Thread t = new Thread(worker.ThreadRunnerWrapper);
            worker.ThreadCallbackStart();
            _workers.Add(worker);
            t.Start();
        }

        internal static void DetachWorker(GeneralThreadWorker worker)
        {
            _workers.Remove(worker);
        }
    }
}