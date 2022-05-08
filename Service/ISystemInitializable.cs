namespace Common.Services
{
	public interface ISystemInitializable
	{
		bool IsInitialized { get; }
		void Initialize();
	}
}