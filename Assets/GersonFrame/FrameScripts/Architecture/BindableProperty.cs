using System;


namespace GersonFrame
{
    /// <summary>
    /// 可以比较的属性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BindableProperty<T> 
    {

        public BindableProperty(T value = default(T))
        {
            mValue = value;
        }

        private T mValue;

        public T Value
        {
            get => mValue;
            set
            {
                if (mValue == null && value == null) return;
                if (mValue == null || !mValue.Equals(value))
                {
                    mValue = value;
                    OnValueChanged?.Invoke(value);
                }
            }
        }

        public static implicit operator T(BindableProperty<T> property)
        {
            return property.Value;
        }

        private event  Action<T> OnValueChanged;

        public void AddValueChangeEvt(Action<T> changeevt)
        {
            OnValueChanged += changeevt;
        }

        public void RemoveValueChangeEvt(Action<T> changeevt)
        {
            OnValueChanged -= changeevt;
        }

        public void ClearValueChangeEvt()
        {
            OnValueChanged = null;
        }

    }

}



