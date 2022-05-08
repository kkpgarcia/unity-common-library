using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Common.Inputs
{
	public class KeyboardInputController
	{
		private List<KeyCommand> _keyCommands;

		public void AddKeyCommand(KeyCommand command)
		{
			this._keyCommands.Add(command);
		}

		public void RemoveKeyCommand<T>() where T : KeyCommand
		{
			this.RemoveKeyCommand(typeof(T));
		}

		public void RemoveKeyCommand(Type type)
		{
			KeyCommand command = this._keyCommands.FirstOrDefault(x => x.GetType() == type);

			if (command != null)
			{
				this._keyCommands.Remove(command);
			}
		}

		private void Update()
		{
			foreach (KeyCommand keyCommand in _keyCommands)
			{
				if (keyCommand.InputCheck())
				{
					keyCommand.Send();
				}
			}
		}
	}
}
