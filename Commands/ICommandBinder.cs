using Common.Bindings;

namespace Common.Commands
{
	public interface ICommandBinder : IBinder
	{
		void ReleaseCommand(ICommand command);

		new ICommandBinding Bind<T>();
		new ICommandBinding Bind(object value);
		new ICommandBinding GetBinding<T>();
		new ICommandBinding GetBinding(object key);
	}
}
