using UnityEngine;
using System.Collections;

using Common.Singleton;

namespace Common.ResolutionUtility
{
	public class ResolutionReference : PermanentMonoSingleton<ResolutionReference>, IResolutionReference
	{
		public enum Resolution
		{
			SD,
			HD,
			UHD,
		}

		public const int HDResolutionCutOff = 960;
		public const int UHDResolutionCutoff = 2048;

		public static Resolution resolution { get; private set; }

		public void Start ()
		{
			int ScreenHeight = Screen.height;
			int ScreenWidth = Screen.width;

			if (ScreenWidth < HDResolutionCutOff)
				resolution = Resolution.SD;
			else {
				#if !UNITY_ANDROID
				if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPad3Gen)
					resolution = Resolution.SD;
				else
					resolution = Resolution.HD;
				#endif
			}
		}
	}
}