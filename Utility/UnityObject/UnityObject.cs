using UnityEngine;
using System.Collections;

public static class UnityObject
{
	public static T Instantiate<T> (this Object original) where T : Object
	{
		return GameObject.Instantiate (original) as T;
	}

	public static T Instantiate<T> (this Object original, Vector3 position, Quaternion rotation) where T : Object
	{
		return GameObject.Instantiate (original, position, rotation) as T;
	}

	public static T Instantiate<T> (this Object original, string name) where T : Object
	{
		T obj = GameObject.Instantiate (original) as T;
		obj.name = name;
		return obj;
	}

	public static T Instantiate<T> (this Object original, string name, Vector3 position, Quaternion rotation) where T : Object
	{
		T obj = GameObject.Instantiate (original, position, rotation) as T;
		obj.name = name;
		return obj;
	}
}
