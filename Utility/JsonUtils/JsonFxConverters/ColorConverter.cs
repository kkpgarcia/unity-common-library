using UnityEngine;
using System.Collections.Generic;
using System;

public class ColorConverter : JsonFx.Json.JsonConverter
{
	public override bool CanConvert (Type type)
	{
		return type == typeof(Color);
	}
	
	public override object ReadJson (Type type, Dictionary<string,object> values)
	{
		return new Color(CastFloat(values["r"]),CastFloat(values["g"]),CastFloat(values["b"]),CastFloat(values["a"]));
	}
	
	public override Dictionary<string,object> WriteJson (Type type, object value)
	{
		Color c = (Color)value;
		return new Dictionary<string, object>()
		{
			{"r",c.r},
			{"g",c.g},
			{"b",c.b},
			{"a",c.a}
		};
	}
}
