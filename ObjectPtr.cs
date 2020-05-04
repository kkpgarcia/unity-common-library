using System.Collections.Generic;

namespace Common.ObjectPointer
{
	public class ObjectPtr<T> where T : class
	{
		public T value = null;

		public ObjectPtr ()
		{
		}

		public ObjectPtr (T src)
		{
			value = src;
		}
	}

	public class NetworkPtr<T> where T : class
	{
		public List<T> value = new List<T> ();

		public NetworkPtr ()
		{
		}

		public NetworkPtr (List<T> src)
		{
			value = src;
		}
	}

	public class DownloadPtr<T> where T : class
	{
		public T value = null;

		public DownloadPtr ()
		{
		}

		public DownloadPtr (T src)
		{
			value = src;
		}
	}
}
