using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class UpdatingObject
{
	protected static List<WeakReference> Checks = new List<WeakReference>();

	protected UpdatingObject()
	{
		Checks.Add(new WeakReference(this));
	}
	public static void Check() 
	{
		for (int iw = 0; iw < Checks.Count; iw++)
		{
			var weakRef = Checks[iw];
			if (weakRef.IsAlive) {
				var u = weakRef.Target as UpdatingObject;
				if (u != null)
					u.Update();
			} else {
				Checks.RemoveAtSwap(iw);
				iw --;
			}
		}
	}
	protected abstract void Update();
}