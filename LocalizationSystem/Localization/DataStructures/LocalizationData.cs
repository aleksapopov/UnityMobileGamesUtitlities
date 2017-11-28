namespace Localization {
	
	[System.Serializable]
	public class LocalizationData {

		public string Key;
		public string English;
		public string Russian;
		public string German;
		public string French;
		public string Spanish;

		//add any number of languages. Names of the fields in this class should correspond to the names of columns in google spreadshit.
	}
}
