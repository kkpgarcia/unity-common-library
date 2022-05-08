using System;
using UnityEngine;

namespace Common.Inputs
{
	public abstract class Manipulator : IManipulator
	{
		protected Manipulatable _manipulatable;
		private KeyCode _key;
		private Func<KeyCode, bool> _inputCheker;
		public Manipulator(Manipulatable manipulatable, KeyCode key, Func<KeyCode, bool> inputChecker)
		{
			this._manipulatable = manipulatable;
			this._key = key;
			this._inputCheker = inputChecker;
		}

		public virtual bool CheckInput()
		{
			return _inputCheker(_key);
		}

		public abstract void Start(GameObject[] objs);
		public abstract void End();
	}
}
