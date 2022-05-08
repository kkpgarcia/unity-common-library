namespace Common.Reactive
{
	public interface IObservable<T>
	{
		T Value { get; set; }
		public IObservableStream<T> GetDataStream();
	}
}
