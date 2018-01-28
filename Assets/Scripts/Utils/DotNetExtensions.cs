using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public static class DotNetExtensions
{
	public static void RemoveAtSwap<T>(this List<T> lst, int idx) 
	{
		lst[idx] = lst[lst.Count-1];
		lst.RemoveAt(lst.Count-1);
	}
}
