using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using JsonFx.Json;

public static class JsonUtil
{
	public static string JSerialize (this object o, JsonConverter converter)
	{
		return JSerialize (o, new List<JsonConverter> () { converter });
	}

	public static string JSerialize (this object o, List<JsonConverter> converters = null, bool prettyPrint = false)
	{
		System.Text.StringBuilder output = new System.Text.StringBuilder ();
		
		JsonWriterSettings settings = new JsonWriterSettings ();
		settings.PrettyPrint = false;
		if (converters != null)
			foreach (JsonConverter jc in converters)
				settings.AddTypeConverter (jc);
		
		settings.AddTypeConverter (new VectorConverter ());
		
		JsonWriter writer = new JsonWriter (output, settings);
		settings.PrettyPrint = prettyPrint;
		writer.Write (o);
		
		return output.ToString ();
		//return JsonConvert.SerializeObject(o, jSettings);
	}

	public static T Deserialize<T> (string serializedString, JsonConverter converter)
	{
		return Deserialize<T> (serializedString, new List<JsonConverter> () { converter });
	}

	public static T Deserialize<T> (string serializedString, List<JsonConverter> converters = null)
	{	
		JsonReaderSettings settings = new JsonReaderSettings ();
		if (converters != null)
			foreach (JsonConverter jc in converters)
				settings.AddTypeConverter (jc);
		
		settings.AddTypeConverter (new VectorConverter ());
		
		JsonReader reader = new JsonReader (serializedString, settings);
		
		T obj = reader.Deserialize<T> ();

		return obj;
	}
}
