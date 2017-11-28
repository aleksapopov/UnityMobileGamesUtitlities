using UnityEngine;
using UnityEngine.UI;

namespace Localization {

	public class TextLocalization : MonoBehaviour {

		public string Key;

		private Text _tf;

		void Awake () {
			LocalizationManager.Instance.OnChangeLocale.AddListener (OnLocaleChanged);
		}

		void Start () {
			LocalizationManager.Instance.OnChangeLocale.AddListener (OnLocaleChanged);
			updateLocalization ();
		}

		void OnEnable () {
			LocalizationManager.Instance.OnChangeLocale.AddListener (OnLocaleChanged);
			updateLocalization ();
		}

		private void OnLocaleChanged () {
			updateLocalization ();
		}

		public void updateLocalization () {
			if (_tf == null) {
				_tf = this.GetComponent<Text> ();
			}

			if (_tf == null || LocalizationManager.Instance == null)
				return;
			
			_tf.text = LocalizationManager.Instance.GetLocalizedValue (Key);
		}
	}
}
