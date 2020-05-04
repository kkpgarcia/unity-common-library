using UnityEngine;
using System;
using System.Collections;

namespace Common.Utils
{
	public interface IRoutineHandler
	{
		Coroutine StartCoroutine(IEnumerator method);
	}
}
