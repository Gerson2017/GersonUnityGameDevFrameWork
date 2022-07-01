using System;


namespace HotGersonFrame
{
    /// <summary>
    /// 可以比较的属性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HotBindableProperty<T>
    {
        public HotBindableProperty(T value = default(T))
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


        public override string ToString()
        {
            return Value.ToString();
        }




        public static implicit operator T(HotBindableProperty<T> property)
        {
            return property.Value;
        }

        private event Action<T> OnValueChanged;

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
            if (OnValueChanged != null)
            {
                Delegate[] delegates = OnValueChanged.GetInvocationList();
                if (delegates!=null)
                {
                    for (int i = 0; i < delegates.Length; i++)
                        OnValueChanged -= (Action<T>)delegates[i];
                }
            }
        }

    }

}



