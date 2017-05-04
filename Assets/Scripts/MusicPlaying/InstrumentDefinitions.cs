using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentDefinitions : ScriptableObject {

	[System.Serializable]
	public class Instrument
	{
		public string Name;

		[ColorUsageAttribute(true, true, 0, 8, 0.125f, 3)]
		public Color SelectedEmmissiveColor = Color.white;

		[ColorUsageAttribute(true, true, 0, 8, 0.125f, 3)]
		public Color UnselectedEmmissiveColor = Color.white;

		public int InstrumentInt = 0;
		public bool IsDrum = false;
		public int InstrumentNoteOffset = 0;
		[Range(0,255)]
		public int NoteVelocity = 127;

		private Material m_unselectedMat = null;
		private Material m_selectedMat = null;

		public Material UnselectedButtonMaterial {get { return m_unselectedMat; }}
		public Material SelectedButtonMaterial {get { return m_selectedMat; }}

		public void Init(Material baseSelectedMat, Material baseUnselectedMat)
		{
			ResetMaterial(ref m_unselectedMat, baseUnselectedMat, UnselectedEmmissiveColor);
			ResetMaterial(ref m_selectedMat, baseSelectedMat, SelectedEmmissiveColor);
		}

		private void ResetMaterial(ref Material cachedMat, Material baseMat, Color emmisiveCol)
		{
			if (cachedMat != null)
				GameObject.Destroy(cachedMat);
			cachedMat = new Material(baseMat);
			cachedMat.SetColor("_EmissionColor", emmisiveCol);
		}
	}

	public Material BaseSelectedMat;
	public Material BaseUnselectedMat;
	public List<Instrument> Instruments;
	private static InstrumentDefinitions s_internalInstance;

	public static InstrumentDefinitions Instance
	{
		get{
			if (s_internalInstance == null)
			{
				s_internalInstance = Resources.Load<InstrumentDefinitions>("InstrumentDefinitionsDB");
				if(s_internalInstance == null)
				{
					Debug.LogError("InstrumentDefinitionsDB could not be loaded");
					return null;
				}
				s_internalInstance.Init();
			}
			return s_internalInstance;
		}
	}

	public Instrument Get(int index)
	{
		if ((uint)index > Instruments.Count)
		{
			Debug.LogError("Index into instruments definition not valid " + index);
			return null;
		}
		return Instruments[index];
	}

	public void Init()
	{
		for (int i = 0; i < Instruments.Count; i++)
		{
			Instruments[i].Init(BaseSelectedMat, BaseUnselectedMat);
		}
	}
}
