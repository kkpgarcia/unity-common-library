using UnityEngine;
using System.Collections.Generic;
using System;

public class KeyFrameConverter : JsonFx.Json.JsonConverter
{	
	public override bool CanConvert (Type type)
	{
		return type == typeof(Keyframe);
	}
	
	public override object ReadJson (Type type, Dictionary<string,object> values)
	{
		Keyframe kf = new Keyframe(
			CastFloat(values["time"]),
			CastFloat(values["value"]),
			CastFloat(values["inTangent"]),
			CastFloat(values["outTangent"])
			);
		kf.tangentMode = (int)values["tangentMode"]; // can immediately typecast to int

		return kf;
	}
	
	public override Dictionary<string,object> WriteJson (Type type, object value)
	{
		Keyframe kf = (Keyframe)value;
		return new Dictionary<string, object>()
		{
			{"time", kf.time },
			{"value", kf.value },
			{"inTangent", kf.inTangent },
			{"outTangent", kf.outTangent },
			{"tangentMode", kf.tangentMode },
		};
	}
}
