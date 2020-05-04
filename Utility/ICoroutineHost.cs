using UnityEngine;
using System.Collections;

namespace Common.Utils
{
	public interface ICoroutineHost
	{
		Coroutine StartCoroutine (IEnumerator routine);
	}
}
