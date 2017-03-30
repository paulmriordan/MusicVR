using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public static class DotNetExtensions
{
	public static DateTime UnixEpoch {get; private set;}

	static DotNetExtensions() {
		UnixEpoch = new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc);
	}

	public static long UnixEpochTime(this DateTime dt) {
		return System.Convert.ToInt64((dt.ToUniversalTime() - UnixEpoch).TotalSeconds);
	}

	public static void ThrowIfNull<T>(T o, string param = null) where T : class {
		if(o == null)
			throw new ArgumentNullException(param ?? "parameter is null");
	}

	public static class Encoding {
		public static UTF8Encoding UTF8NoBOM = new UTF8Encoding(false, true);
	}

	public static class TimeSpan {
		public static System.TimeSpan FromSeconds(float seconds) {
			var s = Mathf.Floor(seconds);
			var ms = (seconds - s) * 1000f;
			return new System.TimeSpan(0,0,0,(int)s,(int)ms);
		}
	}

	public static class Array {
		public static void Shuffle<T>(T[] arr) { //https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle#The_modern_algorithm
			var n = arr.Length-1;
			for(int i=0; i < n; ++i) {
				var swap = n-i;
				var k = UnityEngine.Random.Range(0,swap+1);
				if(k != swap) {
					var value = arr[k];
					arr[k] = arr[swap];
					arr[swap] = value;
				}
			}
		}
	}

	public static T GetRandomNoRepeat<T>(this List<T> _this, System.Random rnd)
	{
		var len =_this.Count;
		if (len == 0)
			throw new Exception("No elements in list");
		if (len == 1)
			return _this[0];
		var chosenIndex = rnd.Next(len-2);
		var temp = _this[chosenIndex];
		_this[chosenIndex] = _this[len - 1];
		_this[len - 1] = temp;
		return temp;
	}

	public static void RemoveAtSwap<T>(this List<T> lst, int idx) 
	{
		lst[idx] = lst[lst.Count-1];
		lst.RemoveAt(lst.Count-1);
	}
	public static bool RemoveSwap<T>(this List<T> lst, T thing) where T : class{
		for(int i=0; i < lst.Count; ++i) {
			if(lst[i] == thing) {
				lst.RemoveAtSwap(i);
				return true;
			}
		}
		return false;
	}
	public static bool RemoveSwap<T>(this List<T> lst, Predicate<T> pred) {
		for(int i=0; i < lst.Count; ++i) {
			if(pred(lst[i])) {
				lst.RemoveAtSwap(i);
				return true;
			}
		}
		return false;
	}

	public static int BinarySearchIndexOf<T>(this IList<T> list, System.Func<T,int,int> comparer)
	{
		if (list == null)
			throw new System.ArgumentNullException("list");
		if (list.Count == 0)
			return -1;

		int lower = 0;
		int upper = list.Count - 1;

		while (lower < upper)
		{
			int middle = lower + (upper - lower) / 2;
			int comparisonResult = comparer(list[middle], middle);
			if (comparisonResult == 0)
				return middle;
			else if (comparisonResult < 0)
				upper = middle - 1;
			else
				lower = middle + 1;
		}

		return comparer(list[lower], lower) >= 0 ? lower : lower - 1;
	}

	public static T FindBest<T>(this IEnumerable<T> list, float currBestVal, System.Func<T, float> getCompareVal, System.Func<float, float, bool> isBetter)
	{
		if (list == null)
			throw new System.ArgumentNullException("enumerator cannot be null");

		var bestItem = default(T);
		foreach (var item in list)
		{
			var val = getCompareVal(item);
			if (isBetter(val, currBestVal))
			{
				bestItem = item;
				currBestVal = val;
			}
		}
		return bestItem;
	}

	public static T GetRandom<T>(this ICollection<T> _this)
	{
		int total = _this.Count;
		if (total == 0)
			return default(T);
		int rnd = UnityEngine.Random.Range(0, total);
		var enumerator = _this.GetEnumerator();
		while (rnd-- >= 0 )
			enumerator.MoveNext();
		return enumerator.Current;
	}
}
