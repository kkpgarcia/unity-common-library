using System;
using System.Collections;
using UnityEngine;

namespace Common.Reactive
{
    [System.Serializable]
    public class Observable<T> : Maybe<T>, IObservable<T>
    {
        [SerializeField]
        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                this._dataStream.Dispatch(_value);
            }
        }

        private IObservableStream<T> _dataStream;

        public Observable(T value)
        {
            this._dataStream = new NotificationStream<T>();
            this._value = value;
        }

        public IObservableStream<T> GetDataStream()
        {
            return _dataStream;
        }

        public override R CheckMatch<R>(Func<T, R> someFunc, Func<R> noneFunc) => someFunc(_value);
        public override void Iter(Action<T> someAction, Action noneAction) => someAction (_value);
        public override bool Equals(object other, IEqualityComparer comparer) => other is Observable<T> s && comparer.Equals(Value, s.Value);
        public override int GetHashCode(IEqualityComparer comparer) => "Observable ".GetHashCode() ^ comparer.GetHashCode(Value);
    }
}
