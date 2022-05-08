

using System;
using System.Collections.Generic;
using System.Linq;
using Common.Bindings;

namespace Common.Commands
{
	public class CommandBinder : Binder, ICommandBinder
	{
		protected HashSet<ICommand> _commandPool;
		protected HashSet<ICommand> _activeCommands;

		public CommandBinder()
		{
			_commandPool = new HashSet<ICommand>();
			_activeCommands = new HashSet<ICommand>();
		}

		public ICommand InvokeCommand(Type commandType, object data)
		{
			ICommand command = CreateCommand(commandType, data);
			_activeCommands.Add(command);
			ExecuteCommand(command);
			return command;
		}

		protected void ExecuteCommand(ICommand command)
		{
			command.Start();
			command.Execute();
		}

		public new ICommandBinding Bind<T>()
		{
			return Bind(typeof(T));
		}

		public new ICommandBinding Bind(object value)
		{
			ICommandBinding binding = new CommandBinding(Resolve);
			binding.Bind(value);
			return binding;
		}

		public new ICommandBinding GetBinding<T>()
		{
			return base.GetBinding<T>() as ICommandBinding;
		}

		public new ICommandBinding GetBinding(object key)
		{
			return base.GetBinding(key) as ICommandBinding;
		}

		public void ReleaseCommand(ICommand command)
		{
			if (_activeCommands.Contains(command))
			{
				_activeCommands.Remove(command);
			}
		}

		protected virtual ICommand CreateCommand(object commandType, object data)
		{
			ICommand newCommand = GetCommand((Type)commandType);
			newCommand.Payload = data;
			return newCommand;
		}

		private ICommand GetCommand(Type type)
		{
			ICommand pooledCommand = _commandPool.FirstOrDefault(x => x.GetType() == type);

			if (pooledCommand == null)
			{
				pooledCommand = Activator.CreateInstance(type) as ICommand;

				if (pooledCommand.IsPooled)
					_commandPool.Add(pooledCommand);
			}

			return pooledCommand;
		}

		/**
		 * RactTo() is a hack to make it adhere to the notification center requirements (object sender, object args)
		 *
		 * In order to pass the binding key to this function, PostCommand() (NotificationExtensions.cs) passes
		 * the notification name to the NotificationCenter as both event name, and sender.
		 *
		 * With this, it allows us to pass the event name as the sender which will be used as the key to get the command binding
		 * for the event.
		 *
		 * ICommandBinding has the following bindings:
		 *
		 *			Key: NotificationName, Value: ICommand
		 *
		 */
		public virtual void ReactTo(object key, object data)
		{
			ICommandBinding binding = GetBinding(key);

			if (binding != null)
			{
				Type commandType = binding.Value as Type;
				ICommand command = InvokeCommand(commandType, data);
				ReleaseCommand(command);
			}
		}
	}
}
