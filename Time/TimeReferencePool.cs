using System;
using System.Collections.Generic;

namespace Common.CustomTime
{
	/**
	 * Class that manages multiple TimeReference instances.
	 */
	public class TimeReferencePool
	{
		
		private IDictionary<string, TimeReference> instanceMap;

		private TimeReferencePool ()
		{
			// can't be instantiated, this is a singleton
			instanceMap = new Dictionary<string, TimeReference> ();
		}

		private static TimeReferencePool ONLY_INSTANCE = null;

		/**
		 * Returns the only TimeReferencePool instance.
		 */
		public static TimeReferencePool GetInstance ()
		{
			if (ONLY_INSTANCE == null) {
				ONLY_INSTANCE = new TimeReferencePool ();
			}
			
			return ONLY_INSTANCE;
		}

		/**
		 * Adds a TimeReference instance for the specified name.
		 */
		public TimeReference Add (string name)
		{
			TimeReference newTimeReference = new TimeReference (name);
			instanceMap [name] = newTimeReference;
			return newTimeReference;
		}

		/**
		 * Retrieves the TimeReference instance for this specified name.
		 */
		public TimeReference Get (string name)
		{
			if (string.IsNullOrEmpty (name)) {
				return TimeReference.GetDefaultInstance ();
			}

			return instanceMap [name];
		}

		/**
		 * Returns the default TimeReference instance.
		 */
		public TimeReference GetDefault ()
		{
			return TimeReference.GetDefaultInstance ();
		}
	}
}

