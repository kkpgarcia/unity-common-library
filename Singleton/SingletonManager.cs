using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Common.Singleton
{
	public class SingletonManager : MonoSingleton<SingletonManager>
	{
		public MonoSingletonBase[] initOrder;
		public List<MonoSingletonBase> registry = new List<MonoSingletonBase> ();

		public static void Register (MonoSingletonBase obj)
		{
			SingletonManager inst = Instance;

			if (inst.registry.IndexOf (obj) < 0)
				inst.registry.Add (obj);
		}

		public override bool Init ()
		{
			if (initOrder != null) {
				Register (this);

				foreach (var st in initOrder) {
					Register (st);
				}
			}

			return true;
		}

		public override void OnDispose ()
		{
			for (int i = registry.Count - 1; i >= 0; --i) {
				if (registry [i] != this)
					registry [i].Dispose ();
			}

			registry = null;
		}

		private void OnApplicationQuit ()
		{
			Dispose ();
		}
	}
}