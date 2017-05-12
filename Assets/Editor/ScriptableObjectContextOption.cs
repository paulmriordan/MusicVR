using UnityEngine;
using UnityEditor;
using MusicVR.Instruments;

static class ScriptableObjectContextOption {

	[MenuItem("Assets/Create/ScriptableObjectContextOption")]
	public static void CreateYourScriptableObject() {
		ScriptableObjectUtility.CreateAsset<InstrumentDefinitions>();
	}
}