using System;
using UnityEngine;

namespace Common.CustomTime
{
	/**
	 * A class representing a single time frame of reference.
	 * This is used for instances where parts of the program need to run in normal time, but other parts need to run in different time scales.
	 * For example, in a speed up tower defense game.
	 */
	public class TimeReference
	{
		
		private String name;

		// this is the expected output time scale whatever the global time scale is
		private float timeScale;

		// when this is true, it updates Time.timeScale directly and uses it when resolving the delta time
		private bool affectsGlobalTimeScale;

		/**
		 * Constructor
		 */
		public TimeReference (String name)
		{
			this.name = name;
			this.timeScale = 1;
			this.affectsGlobalTimeScale = false;
		}

		/**
		 * Returns the name.
		 */
		public String Name {
			get {
				return name;
			}
		}

		/**
		 * Time scale property.
		 */
		public float TimeScale {
			get {
				if (Comparison.TolerantEquals (UnityEngine.Time.timeScale, 0)) {
					// just in case it happens, just return the timeScale that was set
					return this.timeScale;
				}

				// we divide by global time scale so that the perceived value in this.timeScale remains the same whatever the value of the global time scale is
				return this.timeScale / UnityEngine.Time.timeScale;
			}
			
			set {
				if (this.affectsGlobalTimeScale) {
					UnityEngine.Time.timeScale = value;
				}

				this.timeScale = value;
			}
		}

		/**
		 * Returns the delta time for this time reference.
		 */
		public float DeltaTime {
			get {
				// a time scale of zero means a zero deltaTime as well
				// but the timeScale in this timeReference may not be zero
				// so it expects a non-zero delta time
				// we return the fixed delta time instead
				if (Comparison.TolerantEquals (UnityEngine.Time.timeScale, 0)) {
					return UnityEngine.Time.fixedDeltaTime * this.timeScale;
				}

				// note here that we are accessing the TimeScale as property because it has a special calculation in it
				return UnityEngine.Time.deltaTime * TimeScale;
			}
		}

		private static TimeReference DEFAULT_INSTANCE;

		/**
		 * Returns a default instance that can be used by any class.
		 */
		public static TimeReference GetDefaultInstance ()
		{
			if (DEFAULT_INSTANCE == null) {
				DEFAULT_INSTANCE = new TimeReference ("Default");
			}
			
			return DEFAULT_INSTANCE;
		}

		public bool AffectsGlobalTimeScale {
			get {
				return affectsGlobalTimeScale;
			}

			set {
				affectsGlobalTimeScale = value;
			}
		}

	}
}

