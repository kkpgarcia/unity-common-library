using System;
using Common.Events;
using Common.Bindings;

namespace Common.Commands
{
	public class NotificationCommandBinder : CommandBinder
	{
		public override void ResolveBinding(IBinding binding, object key)
		{
			base.ResolveBinding(binding, key);

			if (!this.ContainsKey(key)) return;

			string notifName = (string) key;

			//This uses the sender param to pass the key instead
			this.AddObserver(ReactTo, notifName);
		}

		public void Remove()
		{
			foreach (Tuple<object, IBinding> binding in _bindings)
			{
				string notifName = (string) binding.Item1;
				this.RemoveObserver(ReactTo, notifName);
			}
		}
	}
}
