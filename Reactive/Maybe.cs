using System;
using System.Collections;
using System.Collections.Generic;

namespace Common.Reactive
{
    public abstract class Maybe<T> : IStructuralEquatable, IEquatable<Maybe<T>>
    {
        protected Maybe()
        {
        }
        
        public static Maybe<T> Some(T value) =>
            new Options.Some(value);
        
        public static explicit operator Maybe<T>(T value) =>
            Some(value);
        
        public static Maybe<T> None { get; } = new Options.None();

        public abstract R CheckMatch<R>(Func<T, R> someFunc, Func<R> noneFunc);

        public abstract void Iter(Action<T> someAction, Action noneAction);

        public Maybe<R> Map<R>(Func<T, R> map) =>
            CheckMatch(
                v => Maybe<R>.Some(map(v)),
                () => Maybe<R>.None);

        public R Fold<R>(Func<R, T, R> foldFunc, R seed) => CheckMatch(t => foldFunc(seed, t), () => seed);
        public R TryGet<R>(Func<T, R> foldFunc, R seed) => Fold((_, t) => foldFunc(t), seed);
        public T TryGetDefault(T defaultValue) => Fold((_, t) => t, defaultValue);
        public static Maybe<T> Return(T value) => Some(value);

        public Maybe<R> Bind<R>(Func<T, Maybe<R>> map) =>
            CheckMatch(v => map(v).CheckMatch(
                    r => Maybe<R>.Some(r),
                    () => Maybe<R>.None),
                () => Maybe<R>.None);

        bool IEquatable<Maybe<T>>.Equals(Maybe<T> other) =>
            Equals(other as object);

        public abstract bool Equals(object other, IEqualityComparer comparer);

        public abstract int GetHashCode(IEqualityComparer comparer);

        public static class Options
        {
            public sealed class Some : Maybe<T>
            {
                private T _value;
                private T Value { get => _value; }

                public Some(T value)
                {
                    this._value = value;
                }

                public override R CheckMatch<R>(Func<T, R> func, Func<R> exFunc) => func(_value);
                public override void Iter(Action<T> action, Action exAction) => action(_value);
                public override string ToString() => _value.ToString();

                public bool Equals(Some other)
                {
                    if (ReferenceEquals(null, other)) return false;
                    if (ReferenceEquals(this, other)) return true;
                    return EqualityComparer<T>.Default.Equals(_value, other._value);
                }

                public override bool Equals(object other, IEqualityComparer comparer) =>
                    other is Some s && comparer.Equals(Value, s.Value);

                public override int GetHashCode() =>
                    "Some ".GetHashCode() ^ Value.GetHashCode();

                public override int GetHashCode(IEqualityComparer comparer) =>
                    "Some ".GetHashCode() ^ comparer.GetHashCode(Value);
            }

            public sealed class None : Maybe<T>
            {
                public override R CheckMatch<R>(Func<T, R> someFunc, Func<R> noneFunc) =>
                    noneFunc();

                public override void Iter(Action<T> someAction, Action noneAction) =>
                    noneAction();

                public override string ToString() =>
                    "None";

                public override bool Equals(object obj) =>
                    obj is None;

                public override int GetHashCode() =>
                    "None".GetHashCode();

                public override bool Equals(object other, IEqualityComparer comparer) =>
                    Equals(other);

                public override int GetHashCode(IEqualityComparer comparer) =>
                    GetHashCode();
            }
        }

        
    }
}