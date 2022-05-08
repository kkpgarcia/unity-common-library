namespace Common.Inputs
{
	public interface ISlot
	{
		void OnDragHover(Manipulatable manipulatable);
		void OnReceive(Manipulatable manipulatable);
	}
}
