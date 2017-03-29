using System;
using System.Collections.Generic;

public abstract class InputConsumerBase
{
	public static List<InputConsumerBase> s_registeredConsumer = new List<InputConsumerBase>();

	public InputConsumerBase()
	{
		s_registeredConsumer.Add(this);
	}

	~InputConsumerBase()
	{
		s_registeredConsumer.Remove(this);
	}

	public abstract bool TryConsumeInput();
}

