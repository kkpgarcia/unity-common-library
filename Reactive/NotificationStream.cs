using System;
using Common.Events;

namespace Common.Reactive
{
	public class NotificationStream<T> : IObservableStream<T>, IDisposable
	{
		protected NotificationCenter _dispatcher;
		protected string _notificationName = "";

		public NotificationStream()
		{
			this._dispatcher = new NotificationCenter();
			this._notificationName = Guid.NewGuid().ToString();
		}

		public void Subscribe(System.Action<System.Object, System.Object> action)
		{
			this._dispatcher.AddObserver(action, this._notificationName);
		}

		public void Unsubscribe(System.Action<System.Object, System.Object> action)
		{
			this._dispatcher.RemoveObserver(action, this._notificationName);
		}

		public void Dispatch(T value)
		{
			if (!string.IsNullOrEmpty(_notificationName))
			{
				_dispatcher.PostNotification(_notificationName, this, new InfoEventArgs<T>(value));
			}
		}

		public void Dispose()
		{
			this._dispatcher.Clean();
		}
	}
}
