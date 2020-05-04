using UnityEngine;
using System;

public class FuzzyTime {
	
	public int endTime = 0;
	public string timeUnit = "";
	public string prefix = "";
	public string suffix = "";
	
	public bool isPast { get; private set; }
	
	public FuzzyTime(DateTime dateTime)
	{
		UpdateTime(dateTime);
	}

	public void UpdateTime(DateTime dateTime)
	{
		if (dateTime == DateTime.MinValue) dateTime = DateTime.MinValue.AddMinutes(5);
		TimeSpan tSpan = dateTime - new TimeSpan(0, 5, 0) - DateTime.UtcNow;

		isPast = (tSpan.Minutes < 0); //(tSpan.Minutes >= 0);
		
		int absDays = Mathf.Abs(tSpan.Days);
		int absHours = Mathf.Abs(tSpan.Hours);
		int absMinutes = Mathf.Abs(tSpan.Minutes);
		
		if (absDays >= 30) {
			endTime = Mathf.RoundToInt(absDays / 30f);
			timeUnit = "day";
		} else if (absDays > 0) {
			endTime = absDays;
			timeUnit = "day";
		} else if (absHours > 0) {
			endTime = absHours;
			timeUnit = "hour";
		} else if (absMinutes >= 45) {
			endTime = 1;
			timeUnit = "hour"; 
		} else if (absMinutes >= 10) {
			endTime = absMinutes;
			timeUnit = "min";
		} else {
			prefix = "<";
			endTime = 10;
			timeUnit = "min";
		}
		
		if (isPast) suffix = "ago";
		
		timeUnit = StringHelper.GetTextPlurality(endTime, timeUnit);
	}
	
	public override string ToString() {
		return (prefix + " " + endTime.ToString() + " " + timeUnit + " " + suffix).Trim();	
	}

	public string ToAbsString()
	{
		return (prefix + " " + endTime.ToString() + " " + timeUnit).Trim();	
	}
}


