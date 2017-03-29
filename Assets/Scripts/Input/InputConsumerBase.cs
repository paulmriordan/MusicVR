using System;
using System.Collections.Generic;

public abstract class InputConsumerBase
{
	public static List<InputConsumerBase> s_registeredConsumers = new List<InputConsumerBase>();
	public static InputConsumerBase CurrentInputConsumer {get;private set;}

	public abstract bool TryConsumeInput(InputManager.InputState state);
	public abstract bool IsFinished();

	public InputConsumerBase()
	{
		s_registeredConsumers.Add(this);
	}

	~InputConsumerBase()
	{
		s_registeredConsumers.Remove(this);
	}

	public bool IsActive()
	{
		return CurrentInputConsumer == this;
	}

	public static void UpdateConsumers(InputManager.InputState state)
	{
		//Must check finished first, otherwise, mouse up event consuming will release immediately
		if (CurrentInputConsumer != null && CurrentInputConsumer.IsFinished())
			CurrentInputConsumer = null;
		if (CurrentInputConsumer == null)
			TryFindNewConsumer(state);
	}

	static void TryFindNewConsumer(InputManager.InputState state)
	{
		foreach (var consumer in InputConsumerBase.s_registeredConsumers)
		{
			if (consumer.TryConsumeInput(state))
			{
				CurrentInputConsumer = consumer;
				return;
			}
		}
	}
}

