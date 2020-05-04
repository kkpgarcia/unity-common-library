/*
Author: Kyle Garcia
Date Created: 2016
Last Updated: 2016

NOTE:

This library is outdated and was only used for Unity 5 and with StrangeIoC
implementation. This should not be used for production use.
*/

/*
Author: Kyle Garcia
Date Created: 2016
Last Updated: 2016

NOTE:

This library is outdated and was only used for Unity 5 and with StrangeIoC
implementation. This should not be used for production use.
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Common.Debugging
{
	public class FPSCounter : MonoBehaviour
	{
		public Text Text;
		public float UpdateInterval;

		private float DeltaTime = 0.0f;
		private float MiliSec;
		private float FPS;

		private void Start ()
		{
			StartCoroutine (UpdateRoutine ());
			//Application.targetFrameRate = 60;
		}

		IEnumerator UpdateRoutine ()
		{
			while (true) {
				DeltaTime += (Time.deltaTime - DeltaTime) * 0.1f;
				MiliSec = DeltaTime * 1000f;
				FPS = 1.0f / DeltaTime;

				if (UpdateInterval != 0)
					yield return new WaitForSeconds (UpdateInterval);
				else
					yield return null;

				UpdateText ();
			}
		}

		public void UpdateText ()
		{
			Text.text = string.Format ("{0:0.0} ms ({1:0.} fps)", MiliSec, FPS);
		}
	}
}