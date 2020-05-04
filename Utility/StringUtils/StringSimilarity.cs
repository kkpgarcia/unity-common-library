using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Common.Utils
{

	public static class StringSimilarity
	{
		public static float GetSimilarity (this string str1, string str2)
		{
			List<string> pairs1 = GetWordLetterPairs (str1.ToUpper ());
			List<string> pairs2 = GetWordLetterPairs (str2.ToUpper ());
			int pairsCount1 = pairs1.Count;
			int pairsCount2 = pairs2.Count;
			//Debug.Log(pairs1.JSerialize()+"n"+pairs2.JSerialize());
			int intersection = 0;
			int union = pairsCount1 + pairsCount2;
			for (int i = 0; i < pairsCount1; i++) {
				string pair1 = pairs1 [i];
				for (int j = 0; j < pairs2.Count; j++) {
					//Debug.Log(j + " " + pairsCount2);
					string pair2 = pairs2 [j];
					if (pair1 == pair2) {
						intersection++;
						pairs2.RemoveAt (j);
						break;
					}
				}
			}
			return (float)(2 * intersection) / union;
		}

		private static string[] GetLetterPairs (string str)
		{
			int numPairs = str.Length - 1;
			string[] pairs = new string[numPairs];
			for (int i = 0; i < numPairs; i++) {
				pairs [i] = str.Substring (i, 2);
			}
			return pairs;
		}

		private static List<string> GetWordLetterPairs (string str)
		{
			List<string> allPairs = new List<string> ();
			// Tokenize the string and put the tokens/words into an array
			string[] words = str.Split (' ');
			// For each word
			for (int w = 0; w < words.Length; w++) {
				// Find the pairs of characters
				string[] pairsInWord = GetLetterPairs (words [w]);
				for (int p = 0; p < pairsInWord.Length; p++) {
					allPairs.Add (pairsInWord [p]);
				}
			}
			return allPairs;
		}
	}
}