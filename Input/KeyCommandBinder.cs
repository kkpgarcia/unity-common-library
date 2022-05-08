using System;
using Common.Commands;
using Common.Services;
using Common.Bindings;
using Common.Events;

namespace Common.Inputs
{
	public class KeyCommandBinder : CommandBinder
	{
		public override void ResolveBinding(IBinding binding, object key)
		{
			KeyCommand keyCommand = Activator.CreateInstance(key as Type) as KeyCommand;

			base.ResolveBinding(binding, keyCommand.Event);

			if (!this.ContainsKey(keyCommand.Event)) return;

			KeyboardInputController inputController  = ServiceLocator.GetService<KeyboardInputController>();
			inputController.AddKeyCommand(keyCommand);

			this.AddObserver(ReactTo, keyCommand.Event);
		}

		public override void Unbind(object key)
		{
			KeyCommand keyCommand = Activator.CreateInstance(key as Type) as KeyCommand;

			base.Unbind(keyCommand.Event);

			KeyboardInputController inputController  = ServiceLocator.GetService<KeyboardInputController>();
			inputController.RemoveKeyCommand(key as Type);
		}
	}
}
