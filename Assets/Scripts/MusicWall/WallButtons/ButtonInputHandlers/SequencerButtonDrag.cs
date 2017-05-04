using UnityEngine;

public class SequencerButtonDrag
{
	public enum E_SelectState {selecting, unselecting}

	public E_SelectState 				InputSelectType {get; private set;}
	public SequencerWallButton 			LastHitButton {get;set;}

	public SequencerButtonDrag(E_SelectState selectType)
	{
		InputSelectType = selectType;
	}

	~SequencerButtonDrag()
	{
		if (LastHitButton)
			LastHitButton.InputHandler.MouseDown = false;
	}
}

