using UnityEngine;
using UnityEditor;
using Localization;

[CustomEditor(typeof(LocalizationManager))]
public class LocalizationManagerEditor : Editor {

	public override void OnInspectorGUI () {
		DrawDefaultInspector ();

		LocalizationManager _localizationManager = (LocalizationManager)target;
		if (GUILayout.Button ("Load localization data")) {
			_localizationManager.LoadLocalizationToFile ();
		}
	}
}
