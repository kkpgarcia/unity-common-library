using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Bindings
{
	public class Binder : IBinder
	{
		protected HashSet<Tuple<object, IBinding>> _bindings;

		public delegate void BindingResolver(IBinding binding);

		public Binder()
		{
			_bindings = new HashSet<Tuple<object, IBinding>>();
		}

		public IBinding Bind<T>()
		{
			return Bind(typeof(T));
		}

		public IBinding Bind(object value)
		{
			IBinding binding = new Binding(Resolve);
			binding.Bind(value);
			return binding;
		}

		public IBinding GetBinding<T>()
		{
			return GetBinding(typeof(T));
		}

		public IBinding GetBinding(object key)
		{
			IEnumerable<IBinding> selectedBinding = _bindings
				.Where(x => x.Item1 == key)
				.Select(x => x.Item2);

			return selectedBinding.FirstOrDefault();
		}

		public void Unbind<T>()
		{
			Unbind(typeof(T));
		}

		public void Unbind(IBinding binding)
		{
			Unbind(binding.Key);
		}

		public virtual void Unbind(object key)
		{
			Tuple<object, IBinding> selectedBinding = _bindings
				.FirstOrDefault(x => x.Item1 == key);

			if (selectedBinding != null)
			{
				this._bindings.Remove(selectedBinding);
			}
		}

		public virtual void ResolveBinding(IBinding binding, object key)
		{
			IEnumerable<Tuple<object, IBinding>> selectedBinding = _bindings
				.Where(x => x.Item1 == key);

			Tuple<object, IBinding> exsitingBinding = selectedBinding.FirstOrDefault();

			if (exsitingBinding != null)
			{
				//Conflicting bindings are not allowed
				throw new Exception("There are conflicting bindings for " + key + "");
			}

			this._bindings.Add(new Tuple<object, IBinding>(key, binding));
		}

		protected virtual void Resolve(IBinding binding)
		{
			this.ResolveBinding(binding, binding.Key);
		}

		protected bool ContainsKey(object key)
		{
			Tuple<object, IBinding> selectedBinding = _bindings
				.FirstOrDefault(x => x.Item1 == key);

			return selectedBinding != null;
		}
	}


}
