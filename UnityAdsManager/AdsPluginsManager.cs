using UnityEngine;
using UnityEngine.Advertisements;
using System;
using System.Collections;

/// <summary>
/// Unity ads video ads init, show and callback management
/// </summary>
public class AdsPluginsManager : MonoBehaviour {

	public bool IsInitialized = false;
	public bool IsAdsReady {
		get { return (Application.internetReachability != NetworkReachability.NotReachable) && Advertisement.IsReady (); }
	}

	public static AdsPluginsManager Instance;

	private const string RewardedZoneId = "rewardedVideo";

	private Action _callBack;
	private Action _callBackSkipped;
	private Action _callBackFailed;

	void Awake () {
		if (Instance == null) {
			Instance = this;
		} else if (Instance != this) {
			Destroy (gameObject);
		}

		DontDestroyOnLoad (this.gameObject);
	}

	void Update() {
		if (Application.internetReachability != NetworkReachability.NotReachable) {
			if (!IsInitialized) initPlugins();
		}
	}

	private void initPlugins () {
		if (Advertisement.isSupported) {
			//TODO add real unity ads id in GameConfig file
			Advertisement.Initialize ("1233102", Debug.isDebugBuild);
		}

		IsInitialized = true;
	}

	public void PlayRewardedVideo (Action callBack, Action failedCallBack, Action skippedCallBack) {
		
		if (Application.internetReachability == NetworkReachability.NotReachable) {
			failedCallBack ();
			return;
		}

		if (Advertisement.IsReady ()) {
			_callBack = callBack;
			_callBackFailed = failedCallBack;
			_callBackSkipped = skippedCallBack;
			var _options = new ShowOptions { resultCallback = onRewardedVideoComplete };
			Advertisement.Show (RewardedZoneId, _options);
		} else {
			failedCallBack ();
		}
	}

	private void onRewardedVideoComplete (ShowResult result) {
		switch (result) {

		case ShowResult.Finished:
			
			_callBack ();
			break;

		case ShowResult.Skipped:
			
			_callBackSkipped ();
			break;

		case ShowResult.Failed:
			
			_callBackFailed ();
			break;
		}
	}
}
