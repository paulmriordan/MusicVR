using System;
using System.Collections.Generic;

/// <summary>
/// Allows objects to consume input, preventing other input handling from occuring. 
/// 
/// For example, dragging/panning the wall consumes input once the drag has
/// moved over a threshold distance. Selecting wall buttons themselves cannot occur until
/// wall dragging has finished consuming input
/// 
/// Counter example, selecting a wall button consumes input if held still over a button past
/// a time threshold. Thereafter, dragging around will select more buttons, and not pan the wall
/// </summary>
public abstract class InputConsumerBase
{
	public static List<InputConsumerBase> s_registeredConsumers = new List<InputConsumerBase>();
	public static InputConsumerBase CurrentInputConsumer {get;private set;}

	/// <summary>
	/// Override this method, and return true if, given the current inputstate, whether
	/// your objects consumes input
	/// </summary>
	public abstract bool TryConsumeInput(InputManager.InputState state);
	/// <summary>
	/// When your object has finished consuming input, return true
	/// </summary>
	public abstract bool IsFinished();

	public InputConsumerBase()
	{
		s_registeredConsumers.Add(this);
	}

	~InputConsumerBase()
	{
		s_registeredConsumers.Remove(this);
	}

	/// <summary>
	/// Is a consumer consuming input
	/// </summary>
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

