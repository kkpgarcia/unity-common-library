using UnityEngine;

namespace Common.Inputs
{
	public interface IManipulator
	{
		bool CheckInput();
		void Start(GameObject[] objs);
		void End();

	}
}
