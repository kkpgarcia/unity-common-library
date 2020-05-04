using System;
using System.Collections.Generic;

namespace Common.Signals
{
    public interface IBaseSignal
    {
        void Dispatch(object[] args);
        void AddListener(Action<IBaseSignal, object[]> callback);
        void AddOnce(Action<IBaseSignal, object[]> callback);
        void RemoveListener(Action<IBaseSignal, object[]> callback);
        List<Type> GetTypes();
    }
}
