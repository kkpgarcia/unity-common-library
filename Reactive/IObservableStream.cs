namespace Common.Reactive
{
	public interface IObservableStream<T>
	{
		void Subscribe(System.Action<System.Object, System.Object> action);
		void Unsubscribe(System.Action<System.Object, System.Object> action);
		void Dispatch(T value);
	}
}
