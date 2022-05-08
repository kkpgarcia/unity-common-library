namespace Common.Commands
{
	public interface ICommand
	{
		object Payload { get; set; }
		bool IsPooled { get; }
		void Start();
		void Execute();
		void Undo();
		void SetOnRedo();
		void Retain();
		void Release();
	}
}
