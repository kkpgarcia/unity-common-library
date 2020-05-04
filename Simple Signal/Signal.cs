using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Signals
{
    public class Signal : BaseSignal
    {
        public event Action Listener = delegate { };
        public event Action OnceListener = delegate { };

        public void AddListener(Action callback)
        {
            Listener = this.AddUnique(Listener, callback);
        }

        public void AddOnce(Action callback)
        {
            OnceListener = this.AddUnique(OnceListener, callback);
        }

        public void RemoveListener(Action callback)
        {
            Listener -= callback;
        }

        public override List<Type> GetTypes()
        {
            return new List<Type>();
        }

        public void Dispatch()
        {
            Listener();
            OnceListener();
            OnceListener = delegate { };

            base.Dispatch(null);
        }

        private Action AddUnique(Action listeners, Action callback)
        {
            if (!listeners.GetInvocationList().Contains(callback))
            {
                listeners += callback;
            }
            return listeners;
        }
    }
    public class Signal<T> : BaseSignal
    {
        public event Action<T> Listener = delegate { };
        public event Action<T> OnceListener = delegate { };

        public void AddListener(Action<T> callback)
        {
            Listener = this.AddUnique(Listener, callback);
        }

        public void AddOnce(Action<T> callback)
        {
            OnceListener = this.AddUnique(OnceListener, callback);
        }

        public void RemoveListener(Action<T> callback)
        {
            Listener -= callback;
        }

        public override List<Type> GetTypes()
        {
            List<Type> retv = new List<Type>();
            retv.Add(typeof(T));
            return retv;
        }

        public void Dispatch(T t)
        {
            Listener(t);
            OnceListener(t);
            OnceListener = delegate { };
            object[] outv = { t };
            base.Dispatch(outv);
        }

        private Action<T> AddUnique(Action<T> listeners, Action<T> callback)
        {
            if (!listeners.GetInvocationList().Contains(callback))
            {
                listeners += callback;
            }
            return listeners;
        }
    }

    public class Signal<T, U> : BaseSignal
    {
        public event Action<T, U> Listener = delegate { };
        public event Action<T, U> OnceListener = delegate { };

        public void AddListener(Action<T, U> callback)
        {
            Listener = this.AddUnique(Listener, callback);
        }

        public void AddOnce(Action<T, U> callback)
        {
            OnceListener = this.AddUnique(OnceListener, callback);
        }

        public void RemoveListener(Action<T, U> callback)
        {
            Listener -= callback;
        }

        public override List<Type> GetTypes()
        {
            List<Type> retv = new List<Type>();
            retv.Add(typeof(T));
            retv.Add(typeof(U));
            return retv;
        }

        public void Dispatch(T t, U u)
        {
            Listener(t, u);
            OnceListener(t, u);
            OnceListener = delegate { };
            object[] outv = { t, u };
            base.Dispatch(outv);
        }

        private Action<T, U> AddUnique(Action<T, U> listeners, Action<T, U> callback)
        {
            if (!listeners.GetInvocationList().Contains(callback))
            {
                listeners += callback;
            }
            return listeners;
        }
    }

    public class Signal<T, U, V> : BaseSignal
    {
        public event Action<T, U, V> Listener = delegate { };
        public event Action<T, U, V> OnceListener = delegate { };

        public void AddListener(Action<T, U, V> callback)
        {
            Listener = this.AddUnique(Listener, callback);
        }

        public void AddOnce(Action<T, U, V> callback)
        {
            OnceListener = this.AddUnique(OnceListener, callback);
        }

        public void RemoveListener(Action<T, U, V> callback)
        {
            Listener -= callback;
        }

        public override List<Type> GetTypes()
        {
            List<Type> retv = new List<Type>();
            retv.Add(typeof(T));
            retv.Add(typeof(U));
            retv.Add(typeof(V));
            return retv;
        }

        public void Dispatch(T t, U u, V v)
        {
            Listener(t, u, v);
            OnceListener(t, u, v);
            OnceListener = delegate { };
            object[] outv = { t, u, v };
            base.Dispatch(outv);
        }

        private Action<T, U, V> AddUnique(Action<T, U, V> listeners, Action<T, U, V> callback)
        {
            if (!listeners.GetInvocationList().Contains(callback))
            {
                listeners += callback;
            }
            return listeners;
        }
    }

    public class Signal<T, U, V, W> : BaseSignal
    {
        public event Action<T, U, V, W> Listener = delegate { };
        public event Action<T, U, V, W> OnceListener = delegate { };

        public void AddListener(Action<T, U, V, W> callback)
        {
            Listener = this.AddUnique(Listener, callback);
        }

        public void AddOnce(Action<T, U, V, W> callback)
        {
            OnceListener = this.AddUnique(OnceListener, callback);
        }

        public void RemoveListener(Action<T, U, V, W> callback)
        {
            Listener -= callback;
        }

        public override List<Type> GetTypes()
        {
            List<Type> retv = new List<Type>();
            retv.Add(typeof(T));
            retv.Add(typeof(U));
            retv.Add(typeof(V));
            retv.Add(typeof(W));
            return retv;
        }

        public void Dispatch(T t, U u, V v, W w)
        {
            Listener(t, u, v, w);
            OnceListener(t, u, v, w);
            OnceListener = delegate { };
            object[] outv = { t, u, v, w };
            base.Dispatch(outv);
        }

        private Action<T, U, V, W> AddUnique(Action<T, U, V, W> listeners, Action<T, U, V, W> callback)
        {
            if (!listeners.GetInvocationList().Contains(callback))
            {
                listeners += callback;
            }
            return listeners;
        }
    }
}
