using UnityEngine;
using System.Collections;

namespace Common.Animation {
	public class AudioSourceVolumeTweener : Tweener 
	{
		public AudioSource Source 
		{
			get 
			{
				if (source == null)
					source = GetComponent<AudioSource>();
				return source;
			}
			set
			{
				source = value;
			}
		}
		protected AudioSource source;

		protected override void OnUpdate () 
		{
			base.OnUpdate ();
			Source.volume = currentValue;
		}
	}
}
