namespace Common.Bindings
{
	public class Binding : IBinding
	{
		protected Binder.BindingResolver _resolver;
		protected object _key;
		protected object _value;

		public object Key { get => _key; }
		public object Value { get => _value; }

		public Binding(Binder.BindingResolver resolver)
		{
			this._resolver = resolver;
		}

		public virtual IBinding Bind<T>()
		{
			return Bind(typeof(T));
		}

		public virtual IBinding Bind(object key)
		{
			this._key = key;
			return this;
		}

		public virtual IBinding To<T>()
		{
			return To(typeof(T));
		}

		public virtual IBinding To(object o)
		{
			_value = o;
			if (_resolver != null)
				_resolver(this);
			return this;
		}
	}
}
