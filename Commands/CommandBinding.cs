using Common.Bindings;

namespace Common.Commands
{
	public class CommandBinding : Binding, ICommandBinding
	{
		public CommandBinding (Binder.BindingResolver resolver) : base(resolver)
		{
		}

		public new ICommandBinding Bind<T>()
		{
			return base.Bind<T>() as ICommandBinding;
		}

		public new ICommandBinding Bind(object key)
		{
			return base.Bind(key) as ICommandBinding;
		}

		public new ICommandBinding To<T>()
		{
			return base.To<T>() as ICommandBinding;
		}

		public new ICommandBinding To(object o)
		{
			return base.To(o) as ICommandBinding;
		}
	}
}
