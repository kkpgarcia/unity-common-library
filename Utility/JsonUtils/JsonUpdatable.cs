using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Common.Utils
{
	///<summary>
	/// Updates portions of the object as specified in the Json while keeping the other parts untouched
	/// Can also update class instances from a Dictionary<string, object>
	/// or a value type with another value type instance
	/// 
	/// Notes:
	/// 1. Supported generic containers : IList and IDictionary types
	/// 2. Does not support non generic containers (e.g. ArrayList, Hashtable) but can be added later if really necessary
	///    In that case, a method for finding the best Type is needed
	/// 3. Structs will need to be handled separately
	/// 4. Generally can only handle members declared as concrete types (if a member is declared as an interface or an abstract type, an error will be thrown)
	///    We can handle this later if we really need to by including the actual type of the object in the json
	///</summary>
	public static class JsonUpdatable
	{
		enum DataType
		{
			UNSUPPORTED,
			ARRAY,
			ILIST, // non array IList
			IDICTIONARY,
			STRINGKEYVALUEPAIR, // special case
			STRING,
			CLASS, // String, IList, IDictionary implementations are classes too but we treat them separately
			DATETIME, // this is an immutable value type object, maybe we need to add a DateTime converter?
			VALUETYPE,
		}

		const float SimilarityThreshold = 0.5f; // how similar a string needs to be to an existing member's name to throw a warning about a possible incorrect spelling
		public static bool AddListElementIfMissing = true; // might be for dev only

		#region Exposed

		public static object UpdateObjectWithJson(this object o, ref object parentObject, string json, bool listElementAsProperty = true)
		{
			object data;
			if (o is string) data = json;
			else data = JsonUtil.Deserialize<object>(json);
			return o.UpdateObjectValue(ref parentObject, data, listElementAsProperty);
		}
		
		// We use ref in case parentObject is value type such as numbers, strings, structs
		public static object UpdateObjectValue(this object o, ref object parentObject, object data, bool listElementAsProperty = true)
		{
			if (data == null)
			{
				parentObject = null;
				return null;
			}
			
			Type parentObjectType = parentObject.GetType();
			Type dataObjectType = data.GetType();
			
			DataType dataType = GetDataType(parentObjectType, dataObjectType);
			
			//Debug.Log(dataType + ":\n" + parentObjectType + "\n" + dataObjectType);
			
			switch(dataType)
			{
			case DataType.ARRAY:
				parentObject = HandleArray(parentObject, data, parentObjectType, dataObjectType, listElementAsProperty);
				break;
			case DataType.ILIST:
				HandleIList(parentObject, data, parentObjectType, dataObjectType, listElementAsProperty);
				break;
			case DataType.IDICTIONARY:
				HandleIDictionary(parentObject, data, parentObjectType, dataObjectType, listElementAsProperty);
				break;
			case DataType.STRINGKEYVALUEPAIR:
				Dictionary<string, string> dataAsDict = data as Dictionary<string, string>;
				if (dataAsDict != null && dataAsDict.Count == 1)
				{
					parentObject = new KeyValuePair<string, string>(dataAsDict.Keys.First(), dataAsDict.Values.First());
				}
				break;
			case DataType.STRING:
				parentObject = data;
				break;
			case DataType.CLASS:
				UpdateClassInstance(parentObject, data, listElementAsProperty);
				break;
			case DataType.DATETIME:
				// this won't work as DateTime instances are immutable, need to set the value at the property or field level
				DateTime parsedDateTime = DateTime.Parse(data as string);
				if (parsedDateTime.Kind != DateTimeKind.Utc) parsedDateTime = parsedDateTime.ToUniversalTime();
				parentObject = parsedDateTime;
				//throw new InvalidOperationException("DateTime instances cannot be modified since they are immutable.");
				break;
			case DataType.VALUETYPE:
				HandleValueType(ref parentObject, data, parentObjectType, dataObjectType);
				break;
			default:
				Debug.LogWarning("Unsupported object-data pair:" + parentObject.GetType() + " " + data.GetType());
				break;
			}

			return parentObject;
		}

		#endregion

		
		#region Main Implementation

		static DataType GetDataType(Type parentObjectType, Type dataObjectType)
		{
			if (parentObjectType.IsArray) return DataType.ARRAY;
			if (parentObjectType.GetInterface(typeof(IList).ToString()) != null) return DataType.ILIST;
			if (parentObjectType.GetInterface(typeof(IDictionary).ToString()) != null) return DataType.IDICTIONARY;
			if (parentObjectType == typeof(KeyValuePair<string, string>)) return DataType.STRINGKEYVALUEPAIR;
			if (parentObjectType == typeof(string)) return DataType.STRING;
			if (parentObjectType.IsClass) return DataType.CLASS;
			if (parentObjectType == typeof(DateTime)) return DataType.DATETIME;
			if (parentObjectType.IsValueType) return DataType.VALUETYPE;
			
			return DataType.UNSUPPORTED;
		}
		
		static void HandleValueType(ref object parentObject, object data, Type parentObjectType, Type dataObjectType)
		{
			if (parentObjectType == dataObjectType)
			{
				parentObject = data;
			}

			else if (parentObjectType.IsEnum)
			{
				if (parentObjectType == dataObjectType || Enum.GetUnderlyingType(parentObjectType) == dataObjectType)
				{
					parentObject = Enum.ToObject(parentObjectType, data);
					//Debug.Log("Getting Enum counterpart from underlying type " + parentObjectType + ":" + data);
				}
				else if (data is string)
				{
					parentObject = Enum.Parse(parentObjectType, data as string);
					//Debug.Log("Parsing enum from string " + data);
				}
			}

			else if (parentObject is bool && data is string)
			{
				parentObject = bool.Parse(data as string);
			}
			else if (ReflectionUtil.IsNumericType(parentObjectType) && ReflectionUtil.IsNumericType(dataObjectType))
			{
				parentObject = Convert.ChangeType(data, parentObjectType);
			}
			else if (ReflectionUtil.IsNumericType(parentObjectType) && dataObjectType == typeof(string))
			{
				parentObject = Convert.ChangeType(double.Parse(data as string), parentObjectType);
			}
			else
			{
				Debug.LogWarning("Could not apply " + data.GetType() + "\nto type:" + parentObject.GetType());
				Debug.Log(data.JSerialize());
			}
		}
		
		static Array HandleArray(object parentObject, object data, Type parentObjectType, Type dataObjectType, bool listElementAsProperty = true)
		{
			Array parentArray = parentObject as Array;
			IList dataList = data as IList;
			Type elementType = parentObjectType.GetElementType();

			if (listElementAsProperty)
			{
				IList parentList = ReflectionUtil.CreateList(parentArray);
				HandleIList(parentList, data, parentList.GetType(), dataObjectType, listElementAsProperty);
				parentArray = ReflectionUtil.CreateArray(parentList);
			}
			else
			{
				// blindly apply the json provided

				if (dataList.Count != parentArray.Length)
				{
					parentArray = ReflectionUtil.Resize(parentArray, dataList.Count);
				}
				for (int x = 0; x < dataList.Count; x++)
				{
					object element = Activator.CreateInstance(elementType);
					object currData = dataList[x];
					element = parentObject.UpdateObjectValue(ref element, currData, listElementAsProperty);
					parentArray.SetValue(element, x);
				}
			}

			return parentArray;
		}
		
		static void HandleIList(object parentObject, object data, Type parentObjectType, Type dataObjectType, bool listElementAsProperty = true)
		{
			IList parentList = parentObject as IList;
			Type genericType = parentObjectType.GetGenericArguments()[0];
			IList dataList = data as IList;

			if (listElementAsProperty)
			{
				for (int x = 0; x < dataList.Count; x++)
				{
					object currentData = dataList[x];
					object dataIdValue = GetPropertyValue(currentData, "Id");
					if (dataIdValue == null) continue;
					object equivalentObject = FindObjectByPropertyOrFieldValue(parentList, "Id", dataIdValue);
					if (equivalentObject == null)
					{
						if (AddListElementIfMissing)
						{
							equivalentObject = ReflectionUtil.CreateObjectOfType(genericType);
							parentList.Add(equivalentObject);
						}
						else continue;
					}

					int index = parentList.IndexOf(equivalentObject);
					//Debug.Log("$%^&*()(*&^% " + x + "\n" + equivalentObject.JSerialize() + ", " + currentData.JSerialize() + ", ");
					// just making sure that the right value is applied (e.g. DateTime is immutable and therefore a new one is created)
					parentList[index] = equivalentObject.UpdateObjectValue(ref equivalentObject, currentData, listElementAsProperty);
				}
			}
			else
			{
				for (int x = 0; x < dataList.Count; x++)
				{
					object newElement = ReflectionUtil.CreateObjectOfType(genericType);
					//Debug.Log("created object of type " + genericType);
					newElement = newElement.UpdateObjectValue(ref newElement, dataList[x], listElementAsProperty);
					// update element first before adding it, otherwise, the new value won't apply
					parentList.Add(newElement);
				}
			}
		}

		static object GetPropertyValue(object obj, string propertyName)
		{
			PropertyInfo pi = obj.GetType().GetProperty(propertyName);
			if (pi != null) return pi.GetValue(obj, null);
			IDictionary<string, object> objDict = obj as IDictionary<string, object>;
			if (objDict != null)
			{
				if (objDict.ContainsKey(propertyName)) return objDict[propertyName];
			}
			return null;
		}

		/// <summary>
		/// Finds the equivalent object by member name and value
		/// Note that equality by value is determined by the object.Equals() method
		/// </summary>
		static object FindObjectByPropertyOrFieldValue(IList list, string memberName, object value)
		{
			foreach(object item in list)
			{
				Type type = item.GetType();
				PropertyInfo propertyInfo = type.GetProperty(memberName);
				if (propertyInfo != null)
				{
					object propValue = propertyInfo.GetValue(item, null);
					if (propValue.Equals(value)) return item;
				}
				else
				{
					FieldInfo fieldInfo = type.GetField(memberName);
					if (fieldInfo == null) continue;
					object fieldVal = fieldInfo.GetValue(item);
					if (fieldVal.Equals(value)) return item;
				}
			}
			return null;
		}

		static void HandleIDictionary(object parentObject, object data, Type parentObjectType, Type dataObjectType, bool listElementAsProperty = true)
		{
			Type keyType = parentObjectType.GetGenericArguments()[0];
			Type valueType = parentObjectType.GetGenericArguments()[1];
			
			IDictionary parentDictionary = parentObject as IDictionary;
			IDictionary dataDictionary = data as IDictionary;

			if (parentObjectType == dataObjectType)
			{
				foreach (object key in dataDictionary.Keys)
				{
					parentDictionary[key] = dataDictionary[key];
				}
				return;
			}

			foreach(object keyData in dataDictionary.Keys)
			{
				object valueData = dataDictionary[keyData];
				//Debug.Log("processing key data " + keyData.GetType() + ":" + keyData.JSerialize() + "\nvalue data "+valueData.GetType()+":" + valueData.JSerialize());
				
				object actualKey = null;
				object actualValue = null;
				int keyLength = 0;
				int valueLength = 0;

				if (keyType.IsArray)
				{
					ICollection keyDataCollection = keyData as ICollection;
					keyLength = keyDataCollection.Count;
				}
				if (valueType.IsArray)
				{
					ICollection valueDataCollection = valueData as ICollection;
					valueLength = valueDataCollection.Count;
				}
				actualKey = ReflectionUtil.CreateObjectOfType(keyType, keyLength);
				actualValue = ReflectionUtil.CreateObjectOfType(valueType, valueLength);

				//Debug.Log("Actual Key Value Types: <" + keyType + ", " + valueType + ">");
				// just making sure that the right value is applied (e.g. DateTime is immutable and therefore a new one is created)
				actualKey = parentObject.UpdateObjectValue(ref actualKey, keyData, listElementAsProperty);
				actualValue = parentObject.UpdateObjectValue(ref actualValue, valueData, listElementAsProperty);
				
				parentDictionary[actualKey] = actualValue;
			}
		}

		// Updates class instances that are NEITHER collections NOR strings
		static void UpdateClassInstance(object parentObject, object data, bool listElementAsProperty = true)
		{
			Type parentObjectType = parentObject.GetType();
			if (parentObjectType.GetInterface(typeof(ICollection).ToString()) != null ||
			    parentObjectType == typeof(string))
			{
				string msg = "This method must not be used for updating collections or strings! Use UpdateObject instead.";
				throw new InvalidOperationException(msg);
			}
			
			if (parentObjectType.IsAbstract)
			{
				string msg = "Parent object type is abstract! I need a concrete type.";
				throw new Exception(msg);
			}

			if (data.GetType() != typeof(Dictionary<string, object>))
			{
				string msg = "This method only supports parsing of data in the form Dictionary<string, object>. Provided:" + data.GetType();
				throw new InvalidOperationException(msg);
			}
			
			Dictionary<string, object> dataDict = (Dictionary<string, object>) data;
			foreach(string name in dataDict.Keys)
			{
				//Debug.Log("Updating class member " + parentObjectType + "." + name);
				
				object currentValue = dataDict[name];
				Type currentValueType = currentValue == null ? null : currentValue.GetType();
				object memberValue = null;
				
				PropertyInfo propertyInfo = null;
				FieldInfo fieldInfo = parentObjectType.GetField(name);
				Type memberType = null;
				if (fieldInfo == null)
				{
					propertyInfo = parentObjectType.GetProperty(name);
					if (propertyInfo == null)
					{
						MemberInfo similarMember = ReflectionUtil.FindSimilarMember(parentObjectType, name, SimilarityThreshold);
						if (similarMember == null)
						{
							Debug.LogWarning(parentObjectType + "." + name + " not found");
						}
						else
						{
							Debug.LogWarning(parentObjectType + "." + name +" not found - Did you mean " + similarMember.Name + "?");
						}

						continue;
					}
					memberValue = propertyInfo.GetValue(parentObject, null);
					memberType = propertyInfo.PropertyType;
				}
				else
				{
					memberValue = fieldInfo.GetValue(parentObject);
					memberType = fieldInfo.FieldType;
				}

				if (memberType == null)
					continue;

				bool memberIsCollection = memberType.GetInterface(typeof(ICollection).ToString()) != null;
				bool memberIsList = memberType.GetInterface(typeof(IList).ToString()) != null;

				bool updateMemberWithDataValue = true; // update the member with the value in the dictionary
				bool updateMemberWithNewValue = false; // if true, we create a new instance to be assigned to the member
				
				if ((memberValue == null && currentValue != null))
				{
					updateMemberWithNewValue = true;
				}
				
				if (ReflectionUtil.IsNumericType(memberType) && ReflectionUtil.IsNumericType(currentValueType))
				{
					// convert to the appropriate number type if necessary
					currentValue = Convert.ChangeType(currentValue, memberType);
				}
				else if (memberType.IsEnum)
				{
					// need to convert the data (most likely a string) to a suitable type
					HandleValueType(ref memberValue, currentValue, memberType, currentValueType);
					currentValue = memberValue;
				}
				else if (memberType == typeof(DateTime))
				{
					DateTime parsedDateTime = DateTime.Parse((string)currentValue);
					if (parsedDateTime.Kind != DateTimeKind.Utc) parsedDateTime = parsedDateTime.ToUniversalTime();
					currentValue = parsedDateTime;
				}
				else if (memberIsCollection)
				{
					updateMemberWithNewValue = memberIsList ? !listElementAsProperty : true;
					updateMemberWithDataValue = false;
				}
				
				else if (memberType == currentValueType) {}
				else
				{
					updateMemberWithDataValue = false;
				}
				
				if (updateMemberWithNewValue)
				{
					int arrayLength = 0;
					if (memberIsCollection && currentValue != null)
					{
						// collection may not be an array at all, but the method will ignore this param if it isn't anyway, all is well
						arrayLength = (currentValue as ICollection).Count;
					}
					memberValue = ReflectionUtil.CreateObjectOfType(memberType, arrayLength);
					
					if (fieldInfo != null) fieldInfo.SetValue(parentObject, memberValue);
					else if (propertyInfo != null) propertyInfo.SetValue(parentObject, memberValue, null);
				}
				
				if (updateMemberWithDataValue)
				{
					if (fieldInfo != null) fieldInfo.SetValue(parentObject, currentValue);
					else if (propertyInfo != null) propertyInfo.SetValue(parentObject, currentValue, null);
				}
				else
				{
					//Debug.Log(parentObjectType + "." + name);
					parentObject.UpdateObjectValue(ref memberValue, currentValue, listElementAsProperty);
				}
			}
		}

		#endregion
	}
}