using System;
using System.Collections;

namespace Common.Task
{
	public class TaskDictionary
	{
		public ArrayList Keys = new ArrayList ();
		public ArrayList Values = new ArrayList ();

		public int Count { get { return Keys.Count; } }

		public void Add (object key, object value)
		{
			Keys.Add (key);
			Values.Add (value);
		}

		public object GetKey (int index)
		{
			if (Keys.Count <= index)
				return null;
			return Keys [index];
		}

		public object GetValue (int index)
		{
			if (Values.Count <= index)
				return null;
			return Values [index];
		}

		public void RemoveAt (int index)
		{
			if (Values.Count <= index)
				return;
			Keys.RemoveAt (index);
			Values.RemoveAt (index);
		}

		public void Clear ()
		{
			Keys.Clear ();
			Values.Clear ();
		}
	}
}

