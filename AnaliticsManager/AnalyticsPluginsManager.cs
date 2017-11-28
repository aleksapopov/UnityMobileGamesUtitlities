using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;
using Facebook.Unity;

public class AnalyticsPluginsManager : MonoBehaviour {

	public string FlurryKey;

	public static AnalyticsPluginsManager _instance;
	public static AnalyticsPluginsManager Instance {
		get { return _instance; }
	}

	private bool _isInitialized = false;
	public bool IsInitialized {
		get { return _isInitialized; }
	}

	public void TrackEvent (string eventName, Dictionary<string, string> flurryParameters = null, Dictionary<string, object> ymParameters = null) {
		if (_isInitialized) {
			FlurryAnalytics.logEventWithParameters (eventName, flurryParameters, false);
			AppMetrica.Instance.ReportEvent (eventName, ymParameters);
		}
	}

	void Awake () {
		if (_instance == null) {
			_instance = this;
		} else if (_instance != this) {
			Destroy (this.gameObject);
		}

		if (FB.IsInitialized) {
			FB.ActivateApp ();
		} else {
			FB.Init (() => { FB.ActivateApp (); });
		}

		DontDestroyOnLoad (this.gameObject);
	}

	// Use this for initialization
	void Start () {
	}

	void OnApplicationPause (bool pauseStatus) {
		if (!pauseStatus) {
			if (FB.IsInitialized) {
				FB.ActivateApp();
			} else {
				FB.Init(() => {FB.ActivateApp(); });
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Application.internetReachability != NetworkReachability.NotReachable) {
			if (!_isInitialized)
				initPlugins ();
		}
	}

	public void reportFBDeeplink () {
		//AppMetrica.Instance.
		AppMetrica.Instance.ReportEvent ("Facebook Install");
	}

	private void initPlugins () {
		FlurryAnalytics.setSessionReportsOnPauseEnabled( true );
		FlurryAnalytics.setSessionReportsOnCloseEnabled( true );
		//FlurryAnalytics.startSession( "FY62XCCVWBJV3HTHDYYT" );
		FlurryAnalytics.startSession( "ZQHX27PYFDWRNRKSWQN3" );	

		_isInitialized = true;
	}
}
