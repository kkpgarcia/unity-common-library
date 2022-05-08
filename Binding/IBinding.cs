namespace Common.Bindings
{
	public interface IBinding
	{
		object Key { get; }
		object Value { get; }

		IBinding Bind<T>();
		IBinding Bind(object key);
		IBinding To<T>();
		IBinding To(object o);
	}
}
