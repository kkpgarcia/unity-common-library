using UnityEngine;
using System;

using strange.extensions.context.impl;

namespace Common.Singleton
{
	public abstract class MonoSingletonBase : MonoBehaviour
	{
		protected abstract void Awake ();

		public abstract bool IsPermanent { get; }

		public abstract bool Init ();

		public abstract void Dispose ();

		public abstract void OnDispose ();

	}

	public abstract class MonoSingletonBase<T> : MonoSingletonBase where T : MonoSingletonBase<T>
	{
		protected bool disposed = false;

		public bool Disposed { get { return disposed; } }

		protected static T instance;

		public static bool HasInstance { get { return instance; } }

		public static T Instance {
			get {
				if (!HasInstance) {
					FindInScene ();
					if (HasInstance)
						instance.OnFoundInstance ();
					else {
						Type t = typeof(T);
						GameObject go = new GameObject (t.ToString (), t);
						instance = go.GetComponent<T> ();
						if (instance.IsPermanent) {
							instance.name = "~[" + instance.name + "]";
							DontDestroyOnLoad (instance);
						} else
							instance.name = "~" + instance.name;
					}
				}

				if (!HasInstance) {
					Debug.LogError ("Problem during the creation of " + typeof(T));
					return null;
				}

				return instance;
			}
		}

		protected override sealed void Awake ()
		{
			if (HasInstance) {
				if (IsPermanent)
					Dispose ();
			} else {
				instance = this as T;
				OnFoundInstance ();
			}
		}

		protected static void FindInScene ()
		{
			Type type = typeof(T);
			instance = FindObjectOfType (type) as T;

			if (!instance) {
				string oname = "~" + type + "";
				GameObject go = GameObject.Find (oname);
				if (go) {
					instance = go.GetComponent<T> ();
				}
			}
		}

		protected void OnFoundInstance ()
		{
			if (!Init ())
				Dispose ();
			else if (IsPermanent)
				DontDestroyOnLoad (this);
			else
				SingletonManager.Register (instance);
		}

		public static void DestroyInstance ()
		{
			if (HasInstance)
				DestroyImmediate (instance.gameObject);
		}

		public override bool Init ()
		{
			return true;
		}

		public override void OnDispose ()
		{
		}
	}

	public abstract class MonoSingleton<T> : MonoSingletonBase<T> where T: MonoSingleton<T>
	{
		public override sealed bool IsPermanent { get { return false; } }

		public override sealed void Dispose ()
		{
			if (disposed)
				return;

			OnDispose ();

			disposed = true;

			if (instance == this) {
				instance = null;
			}

			Destroy (this);
			GC.SuppressFinalize (this);
		}

		void OnDisable ()
		{
			if (!SingletonManager.HasInstance && !IsPermanent)
				Dispose ();
		}
	}

	public class InjectableMonoSingleton<T> : MonoSingleton<T> where T: InjectableMonoSingleton<T>
	{
		public override bool Init ()
		{
			ContextView contextView = GameObject.FindObjectOfType<ContextView> ();
			MVCSContext context = (MVCSContext)contextView.context;
			context.injectionBinder.injector.Inject (this, false);

			return base.Init ();
		}
	}

	public abstract class PermanentMonoSingleton<T> : MonoSingletonBase<T> where T : PermanentMonoSingleton<T>
	{
		public override sealed bool IsPermanent { get { return true; } }

		public override sealed void Dispose ()
		{
			if (disposed)
				return;

			OnDispose ();

			if (instance == this)
				instance = null;

			disposed = true;
			DestroyImmediate (gameObject);
		}
	}
}