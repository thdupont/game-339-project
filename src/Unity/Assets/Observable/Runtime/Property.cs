using System;
using System.Collections.Generic;

namespace Observable.Runtime
{
    public class Property<T> : IEquatable<T>
    {
        protected T _value;
        protected Action _event;
        protected IEqualityComparer<T> EqualityComparer { get; }

        public T Value
        {
            get => _value;
            internal set => SetAndNotifyIfChanged(value);
        }

        public T PreviousValue { get; private set; }

        public event Action ChangeEvent
        {
            add => AddAndCall(value);
            remove => _event -= value;
        }

        public Property(T initialValue = default, IEqualityComparer<T> equalityComparer = null)
        {
            _value = initialValue;
            _event = null;
            EqualityComparer = equalityComparer ?? EqualityComparer<T>.Default;
        }

        public Property(IEqualityComparer<T> equalityComparer)
        {
            _value = default;
            _event = null;
            EqualityComparer = equalityComparer;
        }

        private void SetAndNotifyIfChanged(T value)
        {
            if (EqualityComparer.Equals(_value, value))
            {
                return;
            }

            SetAndNotify(value);
        }

        private void SetAndNotify(T value)
        {
            PreviousValue = _value;
            _value = value;
            _event?.Invoke();
        }

        private void AddAndCall(Action action)
        {
            _event += action;
            action?.Invoke();
        }

        public bool Equals(T other)
        {
            return EqualityComparer.Equals(_value, other);
        }

        public override bool Equals(object obj)
        {
            if (obj is T other)
            {
                return Equals(other);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return EqualityComparer.GetHashCode(_value);
        }
    }
}

namespace Observable.Runtime.MutableProperty
{
    public static class MutablePropertyExtensions
    {
        public static void SetValue<T>(this Property<T> property, T value)
        {
            property.Value = value;
        }
    }
}