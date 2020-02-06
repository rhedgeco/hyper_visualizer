using UnityEngine;
using UnityEngine.UI;

namespace HyperScripts.Managers
{
    [RequireComponent(typeof(Text))]
    public class StatusManager : MonoBehaviour
    {
        private static string text;
        private Text textObject;

        private void Awake()
        {
            textObject = GetComponent<Text>();
        }

        private void Update()
        {
            textObject.text = text;
        }

        public static void UpdateStatus(string status)
        {
            text = status;
        }
    }
}
