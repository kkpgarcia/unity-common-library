namespace Common.Bindings
{
	public interface ICommandBinding : IBinding
	{
		new ICommandBinding Bind<T>();
		new ICommandBinding Bind(object key);
		new ICommandBinding To<T>();
		new ICommandBinding To(object o);
	}
}
