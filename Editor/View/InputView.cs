using System;
using UnityEngine.UIElements;

namespace SettingsManagement.Editor
{
    public abstract class InputView
    {
        public InputFieldAttribue FieldAttribue { get; set; }

        public string DisplayName { get; set; }

        public Type ValueType { get; internal set; }

        public event Action<object> ValueChanged;

        public abstract void SetValue(object value);

        public abstract VisualElement CreateView();


        protected void OnValueChanged(object newValue)
        {
            ValueChanged?.Invoke(newValue);
        }
    }
}
