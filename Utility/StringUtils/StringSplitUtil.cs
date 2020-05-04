using System;
using System.Collections.Generic;

namespace Common.Utils
{
	[Serializable]
	public class SplitStringData
	{
		public string[] MainContent;
		public string[] EnclosedContent;
		public string OpeningString { get; private set; }
		public string ClosingString { get; private set; }
		public string Error = null;
		
		public SplitStringData(string openingString, string closingString)
		{
			OpeningString = openingString;
			ClosingString = closingString;
		}
	}

	// Method names in here are still confusing and don't exactly reveal their intended purpose, will fix this later
	public class StringSplitUtil
	{
		private static KeyValuePair<int, int> StartingPair = new KeyValuePair<int,int>(-1,-1);

		public static string[] SplitOutermost(string stringToSplit, string[] separators,
		                                           string openingString, string closingString,
		                                           string openingStringBypass1 = "\"", string closingStringBypass1 = "\"",
		                                           string openingStringBypass2 = "\\\"", string closingStringBypass2 = "\\\"")
		{
			SplitStringData tempStringData = SplitOutermost(stringToSplit, openingString, closingString, openingStringBypass1, closingStringBypass1, openingStringBypass2, closingStringBypass2);
			List<string> splitStringList = new List<string>();
			for( int x = 0; x < tempStringData.MainContent.Length; x++)
			{
				string[] currSplitString = tempStringData.MainContent[x].Split(separators, StringSplitOptions.RemoveEmptyEntries);
				splitStringList.AddRange(currSplitString);
				
				if (x < tempStringData.MainContent.Length-1)
				{
					if (tempStringData.EnclosedContent != null && tempStringData.EnclosedContent.Length > 0)
						splitStringList[splitStringList.Count-1] += openingString + tempStringData.EnclosedContent[x] + closingString;
				}
			}
			
			return splitStringList.ToArray();
		}

		public static string[] Split(string stringToSplit, string[] separators,
		                                           string openingString = "\"", string closingString = "\"",
		                                           string openingStringBypass = "\\\"", string closingStringBypass = "\\\"")
		{
			List<KeyValuePair<int,int>> indices = new List<KeyValuePair<int,int>>();
			int lastIndex = 0;
			bool ignore = false;
			for (int x = 0; x < stringToSplit.Length; x++)
			{
				if (ignore)
				{
					if (ShiftIndexIfOccurs(ref x, closingStringBypass, stringToSplit)) continue;
					if (ShiftIndexIfOccurs(ref x, closingString, stringToSplit))
					{
						ignore = false;
						continue;
					}
					continue;
				}

				// may not be needed since the opening string bypass will only occur AFTER opening string
				// adding this here though just to be thorough
				if (ShiftIndexIfOccurs(ref x, openingStringBypass, stringToSplit)) continue;
				if (ShiftIndexIfOccurs(ref x, openingString, stringToSplit))
				{
					ignore = true;
					continue;
				}

				for (int y = 0; y < separators.Length; y++)
				{
					string separator = separators[y];
					if (OccursAtIndex(x, separator, stringToSplit))
					{
						KeyValuePair<int, int> pair = new KeyValuePair<int, int>(lastIndex, x-1);
						indices.Add(pair);
						x += separator.Length-1;
						lastIndex = x+1;
						break;
					}
				}
			}
			if (indices.Count == 0)
			{
				return new string[] { stringToSplit };
			}

			KeyValuePair<int, int> lastPair = indices[indices.Count-1];
			int highestIndex = stringToSplit.Length-1;
			if (lastPair.Value != highestIndex)
			{
				indices.Add(new KeyValuePair<int, int>(lastIndex, highestIndex));
			}

			string[] splitString = new string[indices.Count];
			for (int x = 0; x < indices.Count; x++)
			{
				KeyValuePair<int, int> pair = indices[x];
				splitString[x] = stringToSplit.Substring(pair.Key, pair.Value-pair.Key+1);
			}

			return splitString;
		}

		static bool ShiftIndexIfOccurs(ref int x, string stringToCheck, string fullString)
		{
			if (OccursAtIndex(x, stringToCheck, fullString))
			{
				x += stringToCheck.Length-1;
				return true;
			}
			return false;
		}

		// extracts only the outermost "enclosures"
		public static SplitStringData SplitOutermost(string stringToSplit, string openingString, string closingString, string stringOpening = "\"", string stringClosing = "\"", string stringOpeningString = "\\\"", string stringClosingString = "\\\"")
		{
			SplitStringData data = new SplitStringData(openingString, closingString);
			
			int openingOccurences = 0;
			int closingOccurences = 0;
			bool isWithinString = false;
			List<KeyValuePair<int ,int>> IndexPairs = new List<KeyValuePair<int ,int>>();
			
			for (int x = 0; x < stringToSplit.Length; x++)
			{
				if (isWithinString)
				{
					// if either of stringOpeningString and stringClosingString is detected while within the string, skip
					// to prevent detection of closingString
					if (ShiftIndexIfOccurs(ref x, stringOpeningString, stringToSplit)) continue;
					if (ShiftIndexIfOccurs(ref x, stringClosingString, stringToSplit)) continue;
					if (ShiftIndexIfOccurs(ref x, stringClosing, stringToSplit)) continue;
				}

				if (ShiftIndexIfOccurs(ref x, stringOpeningString, stringToSplit)) continue;
				
				if (ShiftIndexIfOccurs(ref x, openingString, stringToSplit))
				{
					openingOccurences++;
					if (openingOccurences == 1)
						IndexPairs.Add(new KeyValuePair<int, int>(x, -1));
					continue;
				}
				
				if (ShiftIndexIfOccurs(ref x, closingString, stringToSplit))
				{
					closingOccurences++;
					
					if (openingOccurences == closingOccurences && openingOccurences > 0)
					{
						KeyValuePair<int, int> previous = IndexPairs[IndexPairs.Count-1];
						IndexPairs[IndexPairs.Count-1] = new KeyValuePair<int, int>(previous.Key, x);
						
						// partner enclosing string found, reset count
						openingOccurences = 0;
						closingOccurences = 0;
					}
				}
			}
			
			data.MainContent = new string[IndexPairs.Count+1];
			data.EnclosedContent = new string[IndexPairs.Count];
			for (int x = 0; x < IndexPairs.Count; x++)
			{
				KeyValuePair<int, int> currPair = IndexPairs[x];
				KeyValuePair<int, int> previousPair = x == 0 ? StartingPair : IndexPairs[x-1];
				
				if (currPair.Key == -1 || currPair.Value == -1)
				{
					data.Error = "Parse Error: Misplaced or missing \"" + data.OpeningString + "\" or \"" + data.ClosingString + "\"";
					return data;
				}
				
				data.MainContent[x] = stringToSplit.Substring(previousPair.Value+1, currPair.Key-previousPair.Value-1);
				data.EnclosedContent[x] = stringToSplit.Substring(currPair.Key+1, currPair.Value-currPair.Key-1);
				
				if (x == IndexPairs.Count-1)
				{
					data.MainContent[x+1] = stringToSplit.Substring(currPair.Value+1, stringToSplit.Length-currPair.Value-1);
				}
			}

			if (data.MainContent.Length == 1)
			{
				data.MainContent[0] = stringToSplit;
			}
			return data;
		}
		
		public static bool OccursAtIndex(int index, string subString, string fullString)
		{
			int substringIndex = -1;
			for (int x = index; x < index + subString.Length; x++)
			{
				if (x >= fullString.Length) return false;
				substringIndex++;
				if (fullString[x] == subString[substringIndex])
					continue;
				return false;
			}
			return true;
		}
	}
}