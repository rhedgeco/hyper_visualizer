using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(InputField))]
    public class InputFieldIntLimiter : MonoBehaviour
    {
        private InputField field;

        [SerializeField] private int minValue;
        [SerializeField] private int maxValue;

        private void Awake()
        {
            field = GetComponent<InputField>();
            field.onValueChanged.AddListener(ValueChanged);
            field.onEndEdit.AddListener(EndEdit);
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
        }
    }
}