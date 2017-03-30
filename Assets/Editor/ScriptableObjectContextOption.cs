using UnityEngine;
using UnityEditor;

static class ScriptableObjectContextOption {

	[MenuItem("Assets/Create/ScriptableObjectContextOption")]
	public static void CreateYourScriptableObject() {
		ScriptableObjectUtility.CreateAsset<InstrumentDefinitions>();
	}
}