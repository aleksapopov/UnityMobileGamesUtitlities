using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Events;
using System.IO;

namespace Localization {

	public class LocalizationManager : MonoBehaviour {

		public static LocalizationManager Instance;

		public bool UseKeyAsValue = false;
		public UnityEvent OnChangeLocale;

		private SystemLanguage _currentLocalization;
		public SystemLanguage CurrentLocalization {
			get { return _currentLocalization; }
			set {
				_currentLocalization = value; 
				OnChangeLocale.Invoke ();
			}
		}

		private bool _isReady = false;
		public bool IsReady {
			get { return _isReady; }
		}
			
		private List<LocalizationData> Dictionary;
		private string _localizationLocalPath = "Assets/Resources/localization.bytes";
		private string _missingTextString = "Localized text not found.";

		void Awake () {
			if (Instance == null) {
				Instance = this;
			} else if (Instance != this) {
				Destroy (gameObject);
			}

			DontDestroyOnLoad (gameObject);
		}

		// Use this for initialization
		void Start () {
			
			//LoadLocalizationFromFile ();
			LoadLocalization ();
			if (Application.systemLanguage == SystemLanguage.Russian || 
				Application.systemLanguage == SystemLanguage.English || 
				Application.systemLanguage == SystemLanguage.German || 
				Application.systemLanguage == SystemLanguage.French || 
				Application.systemLanguage == SystemLanguage.Spanish) {

				_currentLocalization = Application.systemLanguage;
			} else {
				_currentLocalization = SystemLanguage.English;
			}
			OnChangeLocale= new UnityEvent ();

			CurrentLocalization = (SystemLanguage)PlayerPrefs.GetInt ("locale", (int)_currentLocalization);
		}
		public void UpdateAll () {
			OnChangeLocale.Invoke ();
		}
	
		//#if UNITY_EDITOR
		public void LoadLocalization () {
			/* if (Debug.isDebugBuild) {
				Localization.CloudConnectorCore.processedResponseCallback.AddListener (parseJSONData);
				Localization.CloudConnectorCore.rawResponseCallback.AddListener (onDataLoadFailed);
				Localization.CloudConnectorCore.GetAllTables ();
			} else { */
				LoadLocalizationFromFile ();
			//}
		}
		//#endif

		#if UNITY_EDITOR
		public void LoadLocalizationToFile () {
			Localization.CloudConnectorCore.processedResponseCallback.AddListener (saveData);
			Localization.CloudConnectorCore.rawResponseCallback.AddListener (onDataLoadFailed);
			Localization.CloudConnectorCore.GetAllTables ();
		}
		#endif

		public string GetLocalizedValue (string key) {

			string _result = key;

			if (!_isReady)
				return _result;

			LocalizationData _resultData = Dictionary.Find (t => t.Key == key);
			if (_resultData == null) {
				Debug.LogError (_missingTextString + " for Key: " + key);
				return _result;
			}

			if (UseKeyAsValue)
				return _result;

			switch (_currentLocalization) {
			case SystemLanguage.English:
				_result = _resultData.English;
				break;

			case SystemLanguage.Russian:
				_result = _resultData.Russian;
				break;

			case SystemLanguage.German:
				_result = _resultData.German;
				break;

			case SystemLanguage.French:
				_result = _resultData.French;
				break;

			case SystemLanguage.Spanish:
				_result = _resultData.Spanish;
				break;
			}

			return _result;
		}  

		private void LoadLocalizationFromFile () {
			TextAsset _ta = Resources.Load ("localization") as TextAsset;
			Stream _s = new MemoryStream (_ta.bytes);

			BinaryFormatter _bf = new BinaryFormatter ();
			RawData _rd = (RawData) _bf.Deserialize (_s);
			_s.Close ();

			if (_rd != null) {
				parseData (_rd);
			}
			else {
				Debug.LogError ("Localization load failed. Try to reload data from the cloud service.");
			}
		}

		private void saveData (Localization.CloudConnectorCore.QueryType query, List<string> objTypeNames, List<string> jsonData) {
			RawData _rd = new RawData ();
			_rd.ObjNypeNames = objTypeNames;
			_rd.JsonData = jsonData;

			BinaryFormatter _bf = new BinaryFormatter();
			FileStream _file = File.Create (_localizationLocalPath);
			_bf.Serialize (_file, _rd);
			_file.Close ();
		}

		private void parseJSONData (Localization.CloudConnectorCore.QueryType query, List<string> objTypeNames, List<string> jsonData) {
			RawData _rd = new RawData ();
			_rd.ObjNypeNames = objTypeNames;
			_rd.JsonData = jsonData;

			BinaryFormatter _bf = new BinaryFormatter();
			FileStream _file = File.Create (_localizationLocalPath);
			_bf.Serialize (_file, _rd);
			_file.Close ();

			parseData (_rd);
		}

		private void parseData (RawData data) {
			for (int i = 0; i < data.ObjNypeNames.Count; i++) {
				switch (data.ObjNypeNames[i]) {
				case "ingame":
					Dictionary = new List<LocalizationData> (JsonHelper.JsonToArray<LocalizationData> (data.JsonData [i]));
					break;
				} 
			}

			_isReady = true;
		}

		private void onDataLoadFailed (string responce) {
			Debug.LogError ("Localization data loading failed with responce: " + responce);
		}

		private class JsonHelper
		{
			public static T[] JsonToArray<T>(string json)
			{
				string newJson = "{ \"array\": " + json + "}";
				Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
				return wrapper.array;
			}

			[System.Serializable]
			private class Wrapper<T>
			{
				public T[] array = new T[] { };
			}
		}
	}
}
