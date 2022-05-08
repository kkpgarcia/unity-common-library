namespace Common.Bindings
{
	public interface IBinder
	{
		IBinding Bind<T>();
		IBinding Bind(object value);
		IBinding GetBinding<T>();
		IBinding GetBinding(object key);

		void Unbind<T>();
		void Unbind(object key);
		void Unbind(IBinding binding);

		void ResolveBinding(IBinding binding, object key);
	}
}
