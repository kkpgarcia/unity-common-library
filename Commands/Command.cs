using Common.Services;

namespace Common.Commands
{
	public abstract class Command : ICommand
	{
		private readonly ICommandBinder _commandBinder;
		private readonly CommandService _commandService;

		private object _payload;

		public object Payload
		{
			get => _payload;
			set => _payload = value;
		}

		protected abstract bool IsUndoable { get; }
		public virtual bool IsPooled { get => !IsUndoable; }
		private bool _isOnRedo = false;

		public Command()
		{
			this._commandBinder = ServiceLocator.GetService<ICommandBinder>();
			this._commandService = ServiceLocator.GetService<CommandService>();
		}

		public virtual void Start() { }

		public void SetOnRedo()
		{
			this._isOnRedo = true;
		}

		public virtual void Execute()
		{
			if (IsUndoable && !_isOnRedo)
			{
				this._commandService.Insert(this);
			}

			if (this._isOnRedo) this._isOnRedo = false;
		}
		public virtual void Retain() { }
		public virtual void Undo() { }

		public virtual void Release()
		{
			this._commandBinder.ReleaseCommand(this);
		}
	}
}
