using UnityEngine;
using System;
using System.Text;

public class StringHelper
{
	public static string GetTextPlurality (int amount, string text, bool isUpperCase = false)
	{
		if (amount != 1)
			return (text + (isUpperCase ? "S" : "s"));
		else
			return text;
	}

	public static string GetRoundedString (int amount)
	{
		float mil10 = 10000000f;
		float mil = 1000000f;
		float k10 = 10000f;
		float k = 1000f;
		
		if (amount >= mil10)
			return "10 mil";
		else if (amount >= mil)
			return ((amount / mil).ToString ("N1") + " mil");
		else if (amount >= k10)
			return "10k";
		else if (amount >= k)
			return ((amount / k).ToString ("N1") + "k");
		else
			return amount.ToString ();
	}

	public static string GetStringWithLeadingZeroes (int val, int digits)
	{
		string valStr = val.ToString ();
		StringBuilder sb = new StringBuilder ();
		for (int i = 0; i < digits - valStr.Length; i++) {
			sb.Append ("0");
		}
		sb.Append (valStr);
		return sb.ToString ();
	}

	public static string GetSignedString (float value, string stringFormat)
	{
		return GetSignedString (value, stringFormat, "", "");
	}

	public static string GetSignedString (float value, 
	                                     string stringFormat, 
	                                     string positiveHexColor,
	                                     string negativeHexColor)
	{
		return GetSignedString (value, stringFormat, positiveHexColor, negativeHexColor, "", "");
	}

	public static string GetSignedString (float value, 
	                                     string stringFormat, 
	                                     string positiveHexColor,
	                                     string negativeHexColor,
	                                     string prefix,
	                                     string suffix)
	{
		string signStr = "";
		string hexColor = "";
		float absValue = value;
		if (value > 0) {
			signStr = "+";
			hexColor = positiveHexColor;
		} else if (value < 0) {
			signStr = "-";
			absValue = -value;
			hexColor = negativeHexColor;
		}

		StringBuilder sb = new StringBuilder ();
		if (string.IsNullOrEmpty (hexColor)) {
			sb.AppendFormat ("{0}{1}{2}{3}", prefix, signStr, absValue.ToString (stringFormat), suffix);
		} else {
			sb.AppendFormat ("[{0}]{1}{2}{3}{4}[-]", hexColor, prefix, signStr, absValue.ToString (stringFormat), suffix);
		}
		
		return sb.ToString ();
	}

	public static string GetOrdinal (int cardinal)
	{
		string[] suffixes = { "th", "st", "nd", "rd"  };
		int lastTwoDigits = cardinal % 100;
		int lastDigit = cardinal % 10;
		if (lastTwoDigits >= 11 && lastTwoDigits <= 13)
			return cardinal + suffixes [0];
		return cardinal + suffixes [lastDigit <= 3 ? lastDigit : 0]; 
	}

	public static string GetTrimmedFloatString (float value)
	{
		string floatStr = value.ToString ();
		if (floatStr.Contains (".")) {
			while (floatStr [floatStr.Length - 1] == '0') {
				floatStr = floatStr.Remove (floatStr.Length - 1);
			}
			
			if (floatStr [floatStr.Length - 1] == '.') {
				floatStr = floatStr.Remove (floatStr.Length - 1);
			} else {
				string[] floatStrArray = floatStr.Split (new char[] { '.' }, StringSplitOptions.None);
				if (floatStrArray.Length == 2) {
					floatStr = int.Parse (floatStrArray [0]).ToString ("N0") + "." + floatStrArray [1];
				}
			}
		}
		return floatStr;
	}

	public static string GetFormattedTimespan (TimeSpan timeSpan)
	{
		string mStr = StringHelper.GetStringWithLeadingZeroes (timeSpan.Minutes, 2);
		string sStr = StringHelper.GetStringWithLeadingZeroes (timeSpan.Seconds, 2);
		string msStr = StringHelper.GetStringWithLeadingZeroes (timeSpan.Milliseconds, 3);
		if (msStr.Length > 3)
			msStr = msStr.Substring (0, 3);
		StringBuilder sb = new StringBuilder ();
		sb.AppendFormat ("{0}:{1}.{2}", mStr, sStr, msStr);
		return sb.ToString ();
	}

	public static string GetFormattedLength (float length, float threshold = 1000, string smallerUnit = "m", string largerUnit = "km")
	{
		if (length < threshold)
			return length + " " + smallerUnit;
		return (length / threshold).ToString ("n2") + " " + largerUnit;
	}

	public static string EncloseInHex (string text, string hexString)
	{
		StringBuilder sb = new StringBuilder ();
		sb.AppendFormat ("[{0}]{1}[-]", hexString, text);
		return sb.ToString ();
	}

	public static string GetFormattedDuration (int seconds, bool includeSeconds = true)
	{
		TimeSpan span = new TimeSpan (0, 0, seconds);
		if (span.TotalSeconds == 0f) {
			return "";
		} else if (span.TotalMinutes < 1f) {
			return span.Seconds.ToString ("N0") + " " + GetTextPlurality (span.Seconds, "second");
		} else if (span.TotalHours < 1f) {
			string str = span.Minutes.ToString ("N0") + " " + GetTextPlurality (span.Minutes, "minute");
			if (span.Seconds > 0 && includeSeconds)
				str += " and " + GetFormattedDuration (span.Seconds);
			return str;
		} else {
			string str = span.Hours.ToString ("N0") + " " + GetTextPlurality (span.Hours, "hour");
			if (span.Minutes > 0 && includeSeconds)
				str += " and " + GetFormattedDuration ((span.Minutes * 60) + span.Seconds);
			return str;
		}
	}
}
