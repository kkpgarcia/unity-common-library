using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Common.Services
{
	public static class ServiceLocator
	{
		public enum Persistency
		{
			TEMPORARY,
			PERMANENT
		}

		private static readonly Dictionary<Type, object> _permanentServices = new Dictionary<Type, object>();
		private static readonly Dictionary<Type, object> _temporaryServices = new Dictionary<Type, object>();

		public static int Count
		{
			get => _temporaryServices.Count + _permanentServices.Count;
		}

		public static void Clean()
		{
			_temporaryServices.Clear();
			_permanentServices.Clear();
		}

		public static void Flush()
		{
			_temporaryServices.Clear();
		}

		public static bool ContainsService<T>(Persistency persistency = Persistency.PERMANENT)
		{
			return ContainsService(typeof(T), persistency);
		}

		public static bool ContainsService(Type type, Persistency persistency = Persistency.PERMANENT)
		{
			if (persistency == Persistency.PERMANENT)
				return _permanentServices.ContainsKey(type);

			return _temporaryServices.ContainsKey(type);
		}

		public static void RegisterService<T>(object service, Persistency persistency)
		{
			RegisterService(typeof(T), service, persistency);
		}

		public static void RegisterService(Type interfaceType, object instance, Persistency persistency)
		{
			Assert.IsFalse(ContainsService(interfaceType, persistency), "ServiceLocator: Cannot register the same type of service twice.");
			if (persistency == Persistency.PERMANENT)
			{
				_permanentServices.Add(interfaceType, instance);
			}
			else
			{
				_temporaryServices.Add(interfaceType, instance);
			}

			if (interfaceType.GetInterface("ISystemOnRegistered") == null)
			{
				Debug.LogWarning("SERVICE LOCATOR: Instance of " + interfaceType.ToString() + " is not derived from ISystemOnRegistered and therefore will not be automatically initialized.");
			}
			else
			{
				ISystemOnRegistered system = (ISystemOnRegistered) instance;
				system.OnRegistered();
			}
		}

		public static void DeregisterService(Type interfaceType)
		{
			if (_temporaryServices.ContainsKey(interfaceType)) _temporaryServices.Remove(interfaceType);
			if (_permanentServices.ContainsKey(interfaceType)) _permanentServices.Remove(interfaceType);
		}

		public static void DeregisterService<T>()
		{
			DeregisterService(typeof(T));
		}

		public static T GetService<T>()
		{
			if (_temporaryServices.ContainsKey(typeof(T))) return (T) _temporaryServices[typeof(T)];
			if (_permanentServices.ContainsKey(typeof(T))) return (T) _permanentServices[typeof(T)];

			Debug.LogError("Service Locator: The requested service " + typeof(T) + " is not registered to the locator");

			return default;
		}
	}
}
