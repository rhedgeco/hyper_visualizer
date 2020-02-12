using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HyperScripts.UI
{
    [RequireComponent(typeof(InputField))]
    public class InputFieldIntLimiter : MonoBehaviour
    {
        private InputField field;

        [SerializeField] private int minValue;
        [SerializeField] private int maxValue;

        private ValueConfirmedEvent confirmedEvent = new ValueConfirmedEvent();

        protected ValueConfirmedEvent ConfirmedEvent => confirmedEvent;

        protected class ValueConfirmedEvent : UnityEvent<int>
        {
        }

        protected void Awake()
        {
            field = GetComponent<InputField>();
            field.onValueChanged.AddListener(ValueChanged);
            field.onEndEdit.AddListener(EndEdit);
        }

        public int GetValue()
        {
            int v = int.Parse(field.text);
            return v;
        }

        private void ValueChanged(string text)
        {
            // do nothing
        }

        private void EndEdit(string text)
        {
            int value;
            if (!int.TryParse(text, out value))
            {
                field.text = "" + minValue;
                return;
            }

            if (value > maxValue) value = maxValue;
            if (value < minValue) value = minValue;

            field.text = "" + value;
            ConfirmedEvent.Invoke(value);
        }
    }
}