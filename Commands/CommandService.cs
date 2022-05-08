using System.Collections.Generic;

namespace Common.Commands
{
	public class CommandService
	{
		private const int MAX_COMMAND_STORAGE = 50;
		private StackList<ICommand> _undoStack;
		private StackList<ICommand> _redoStack;

		public CommandService()
		{
			_undoStack = new StackList<ICommand>();
			_redoStack = new StackList<ICommand>();
		}

		public void Clear()
		{
			_undoStack.Clear();
			_redoStack.Clear();
		}

		public void Undo()
		{
			ICommand command = this._undoStack.Pop();

			if (command != null)
			{
				command.Undo();
				this._redoStack.Push(command);
			}
		}

		public void Redo()
		{
			ICommand command = this._redoStack.Pop();
			if (command != null)
			{
				command.SetOnRedo();
				command.Execute();
				this._undoStack.Push(command);
			}
		}

		public void Insert(ICommand command)
		{
			this._undoStack.Push(command);
			this._redoStack.Clear();
		}

		private class StackList<T> : LinkedList<T>
		{
			public void Push(T item)
			{
				if (this.Count >= MAX_COMMAND_STORAGE)
				{
					LinkedListNode<T> last = this.Last;

					if (typeof(T).GetInterface("IStackReleased") != null)
					{
						IStackReleased toRelease = last.Value as IStackReleased;
						if (toRelease == null) return;
							toRelease.OnStackReleased();
					}

					this.RemoveLast();
				}

				this.AddFirst(item);
			}

			public T Pop()
			{
				T item = this.Count > 0 ? this.First.Value : default;

				if (item != null)
					this.RemoveFirst();
				return item;
			}
		}
	}
}
