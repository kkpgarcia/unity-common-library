using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Common.Utils
{
	public static class ReflectionUtil
	{
		public static object CreateObjectOfType (Type type, int arrayLength = 0, Type implementingType = null)
		{
			if (type == typeof(string))
				return "";
			
			if (type.IsArray) {
				Type elementType = type.GetElementType ();
				return Array.CreateInstance (elementType, arrayLength);
			}

			if (type == typeof(IDictionary)) {
				Type[] genericArgs = type.GetGenericArguments ();
				Type keyType = typeof(string);
				Type valueType = keyType;
				if (genericArgs.Length >= 2) {
					keyType = genericArgs [0];
					valueType = genericArgs [1];
				}
				return CreateDictionary (keyType, valueType);
			}
			if (type == typeof(IList)) {
				Type[] genericArgs = type.GetGenericArguments ();
				Type keyType = typeof(string);
				if (genericArgs.Length >= 1) {
					keyType = genericArgs [0];
				}
				return CreateList (keyType);
			}
			
			if (type.IsInterface || type.IsAbstract) {
				if (implementingType == null) {
					string msg = "The implementing type is unknown, cannot create an instance of of " + type;
					throw new MethodAccessException (msg);
				}
				if (implementingType.IsInterface || implementingType.IsAbstract) {
					string msg = "The implementing type is invalid, cannot create an instance of of " + implementingType;
					throw new MethodAccessException (msg);
				}
				
				return Activator.CreateInstance (implementingType);
			}
			
			return Activator.CreateInstance (type);
		}

		// Creates a List<T>, note that this does not handle non generic implementations or any other IList implementations
		public static IList CreateList (Type genericType)
		{
			Type baseListType = typeof(List<>);
			baseListType = baseListType.MakeGenericType (new Type[] { genericType });
			return Activator.CreateInstance (baseListType) as IList;
		}

		// Creates a Dictionary<T,U>, note that this does not handle non generic implementations or any other IDictionary implementations
		public static IDictionary CreateDictionary (Type keyType, Type valueType)
		{
			Type baseDictType = typeof(Dictionary<,>);
			baseDictType = baseDictType.MakeGenericType (new Type[] { keyType, valueType });
			return Activator.CreateInstance (baseDictType) as IDictionary;
		}

		// Resize method that does not require knowledge of the array element type
		public static Array Resize (Array array, int newSize)
		{
			Array newArray = Array.CreateInstance (array.GetType ().GetElementType (), newSize);
			Array.Copy (array, newArray, array.Length);
			return newArray;
		}

		public static IList CreateList (Array array)
		{
			IList parentList = CreateList (array.GetType ().GetElementType ());
			for (int x = 0; x < array.Length; x++)
				parentList.Add (array.GetValue (x));
			return parentList;
		}

		public static Array CreateArray (IList list)
		{
			Array array = Array.CreateInstance (list.GetType ().GetGenericArguments () [0], list.Count);
			for (int x = 0; x < array.Length; x++) {
				array.SetValue (list [x], x);
			}
			return array;
		}

		public static MemberInfo FindSimilarMember (Type type, string name, float similarityThreshold)
		{
			MemberInfo[] members = type.GetMembers ();
			Dictionary<MemberInfo, float> similarMembers = new Dictionary<MemberInfo, float> ();
			
			for (int x = 0; x < members.Length; x++) {
				MemberInfo currMember = members [x];
				switch (currMember.MemberType) {
				case MemberTypes.Field:
				case MemberTypes.Property:
					float similarity = name.GetSimilarity (currMember.Name);
					//if (similarity > 0.2f) Debug.Log(name + " " + currMember.Name + " " + similarity);
					if (similarity >= similarityThreshold) {
						similarMembers.Add (currMember, similarity);
					}
					break;
				}
			}
			
			float highestSimilarity = 0;
			MemberInfo mostSimilarMember = null;
			foreach (KeyValuePair<MemberInfo, float> pairs in similarMembers) {
				float currSimilarity = pairs.Value;
				if (currSimilarity > highestSimilarity) {
					highestSimilarity = currSimilarity;
					mostSimilarMember = pairs.Key;
				}
			}
			
			return mostSimilarMember;
		}
		
		// from http://stackoverflow.com/questions/1749966/c-sharp-how-to-determine-whether-a-type-is-a-number
		public static bool IsNumericType (Type type)
		{
			switch (Type.GetTypeCode (type)) {
			case TypeCode.Byte:
			case TypeCode.SByte:
			case TypeCode.UInt16:
			case TypeCode.UInt32:
			case TypeCode.UInt64:
			case TypeCode.Int16:
			case TypeCode.Int32:
			case TypeCode.Int64:
			case TypeCode.Decimal:
			case TypeCode.Double:
			case TypeCode.Single:
				return true;
			default:
				return false;
			}
		}
	}
}