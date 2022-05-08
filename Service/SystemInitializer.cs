using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Parallel;
using UnityEngine;

namespace Common.Services
{
	public class SystemInitNode
	{
		public Type Type;
		public Type Intrface;
		public object Instance;
		public bool isMonoBehaviour;
		public object[] Params;

		public Action<object> OnInitialize;
		public bool ForceInstance = false;

		private SystemInitializer _initializer;

		public SystemInitNode(SystemInitializer initializer)
		{
			this._initializer =  initializer;
		}

		public SystemInitNode Start<T>()
		{
			this.Type = typeof (T);
			this.isMonoBehaviour = Type.IsSubclassOf(typeof(MonoBehaviour));

			return this;
		}

		public SystemInitNode As<T> ()
		{
			Intrface = typeof(T);
			return this;
		}

		public SystemInitNode Parameters(params object[] param)
		{
			this.Params = param;
			return this;
		}

		public void Finally(Action<object> callback)
		{
			this.OnInitialize(callback);

			this.ForceInstance = true;

			this._initializer.Register(this);
		}

		public void AutoRegister(bool permanent = true)
		{
			OnInitialize = (object instance) =>
				ServiceLocator.RegisterService(Intrface == null ? this.Type : Intrface, instance, permanent ? ServiceLocator.Persistency.PERMANENT : ServiceLocator.Persistency.TEMPORARY);

			this.ForceInstance = true;

			this._initializer.Register(this);
		}
	}

	public class SystemInitializer
	{
		public enum InstantiateMode
		{
			NONE,
			FORCED
		}

		private readonly Queue<SystemInitNode> _initializeQueue;
		private readonly GameObject _monoContainer;

		public SystemInitializer(GameObject monoContainer)
		{
			_initializeQueue = new Queue<SystemInitNode>();
			_monoContainer = monoContainer;
		}

		public SystemInitNode Add<T>()
		{
			SystemInitNode node = new SystemInitNode(this);

			return node.Start<T>();
		}

		public void Register(SystemInitNode node)
		{
			_initializeQueue.Enqueue(node);
		}


		[Obsolete]
		public void Add<T>(Action<object> callback, InstantiateMode mode = InstantiateMode.NONE)
		{
			Add(typeof(T), null, new object[] { }, callback, mode);
		}

		[Obsolete]
		private void Add(Type type, object instance, object[] param, Action<object> callback, InstantiateMode mode)
		{
			SystemInitNode node = new SystemInitNode(this)
			{
				Type = type,
				Instance = instance,
				isMonoBehaviour = type.IsSubclassOf(typeof(MonoBehaviour)),
				Params = param,
				OnInitialize = callback,
				ForceInstance = mode == InstantiateMode.FORCED
			};

			_initializeQueue.Enqueue(node);
		}

		public async Task Execute()
		{
			while (_initializeQueue.Count > 0)
			{
				SystemInitNode node = _initializeQueue.Dequeue();
				await InitializeSystem(node);
			}
		}

		private async Task InitializeSystem(SystemInitNode node)
		{
			Debug.Log("SYSTEM INITIALIZER: Initializing " + node.Type.ToString());

			//Do it in the main thread instead
			if (node.isMonoBehaviour)
			{
				if (node.Instance == null)
				{
					//This really need to have an async operation
					// this.PostNotification(MainThreadDispatcher.MainThreadEnqueueEvent, new InfoEventArgs<Action>(() =>
					// {
					// 	Debug.LogWarning(
					// 		"SYSTEM INITIALIZER: Instance of " + node.Type.ToString() + " is not provided.");
					//
					// 	if (node.ForceInstance)
					// 	{
					// 		Debug.LogWarning("SYSTEM INITIALIZER: Creating instance of type " + node.Type.ToString());
					// 		node.Instance = _monoContainer.AddComponent(node.Type);
					// 	}
					// }));

					await MainThreadDispatcher.Instance().EnqueueAsync(() =>
					{
						Debug.LogWarning(
							"SYSTEM INITIALIZER: Instance of " + node.Type.ToString() + " is not provided.");

						if (node.ForceInstance)
						{
							Debug.LogWarning("SYSTEM INITIALIZER: Creating instance of type " + node.Type.ToString());
							node.Instance = _monoContainer.AddComponent(node.Type);
						}
					});
				}
			}
			else
			{
				object instance = Activator.CreateInstance(node.Type, node.Params);
				node.Instance = instance;
			}

			if (node.Type.GetInterface("ISystemInitializable") == null)
			{
				Debug.LogWarning("SYSTEM INITIALIZER: Instance of " + node.Type.ToString() + " is not derived from ISystemInitializable and therefore will not be automatically initialized.");
			}
			else
			{
				ISystemInitializable system = (ISystemInitializable) node.Instance;
				system.Initialize();

				while (!system.IsInitialized) await Task.Yield();
			}


			Debug.Log("Initialized " + node.Type.ToString() + ": " + ((node.Instance == null) ? "FAILED" : "SUCCESS"));

			node.OnInitialize?.Invoke(node.Instance);
		}

		public bool IsAllSystemsInitialized()
		{
			return _initializeQueue.Count == 0;
		}
	}
}
