using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Objects which inherit from this will be updated once per frame.
/// Similar to Monobehaviour, without less overhead
/// </summary>
public abstract class AutoUpdatingObject
{
	protected static List<WeakReference> ObjectsToUpdate = new List<WeakReference>();

	protected AutoUpdatingObject()
	{
		ObjectsToUpdate.Add(new WeakReference(this));
	}

	/// <summary>
	/// This needs to be called once per frame
	/// </summary>
	public static void UpdateAll() 
	{
		for (int i = 0; i < ObjectsToUpdate.Count; i++)
		{
			var weakRef = ObjectsToUpdate[i];
			if (weakRef.IsAlive) 
			{
				var u = weakRef.Target as AutoUpdatingObject;
				if (u != null)
				{
					u.Update();
				}
			} 
			else
			{
				ObjectsToUpdate.RemoveAtSwap(i);
				i--;
			}
		}
	}

	protected abstract void Update();
}