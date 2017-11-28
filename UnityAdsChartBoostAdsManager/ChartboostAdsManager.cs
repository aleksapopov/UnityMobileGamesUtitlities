using UnityEngine;
using UnityEngine.Advertisements;
using ChartboostSDK;
using System;
using System.Collections;
using Prime31;

/// <summary>
/// UnityAds Cgartboost interstitial static and video ads show and callbacks managment.
/// This is a singleton that is persistent through all scenes.
/// Ads are requested as a cascade. Unity ads has a priority, if it's not avaliable - show chartboost content
/// </summary>
public class AdPluginsManager : MonoBehaviour {
	private bool _isInitialized = false;
	public bool IsInitialized {
		get { return _isInitialized; }
	}

	private static AdPluginsManager _instance;
	public static AdPluginsManager Instance {
		get { return _instance; }
	}

	public EventHandler RewardedVideoCompleteEvent;
	public EventHandler StartGameInterstitalClose;
	private bool _isQuitting = false;
	private bool _AdBuddizIsCached = false;
	private bool _chartboostIsDisplayed = false;
	private bool _chartboostInterstitialIsCached = false;
	private bool _upsightInterstitialInGameIsCached = false;
	private bool _upsightInterstitialOnRestartIsCached = false;

	void Awake() {
		if (_instance != null && _instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			_instance = this;
		}
		DontDestroyOnLoad(this.gameObject);
	}

	#region Listeners
	void OnEnable() {
		#if UNITY_IPHONE
		Chartboost.didDisplayInterstitial += onChartboostShowInterstitial;
		Chartboost.didDisplayMoreApps += onChartboostShowMoreGames;
		Chartboost.didCacheInterstitial += onChartboostCacheInterstitial;
		Chartboost.didDismissInterstitial += onChartboostDismissInterstitial;
		Chartboost.didDismissMoreApps += onChartboostDismissMoreApps;
		Chartboost.didFailToLoadInterstitial += onChartBoostFailedToLoadInterstitial;
		Chartboost.didFailToLoadRewardedVideo += onChartboostFailedToLoadRewardedVideo;
		#endif
	}

	void OnDisable() {
		#if UNITY_IPHONE
		Chartboost.didDisplayInterstitial -= onChartboostShowInterstitial;
		Chartboost.didDisplayMoreApps -= onChartboostShowMoreGames;
		Chartboost.didCacheInterstitial -= onChartboostCacheInterstitial;
		Chartboost.didDismissInterstitial -= onChartboostDismissInterstitial;
		Chartboost.didDismissMoreApps -= onChartboostDismissMoreApps;
		Chartboost.didFailToLoadInterstitial -= onChartBoostFailedToLoadInterstitial;
		Chartboost.didFailToLoadRewardedVideo -= onChartboostFailedToLoadRewardedVideo;
		#endif
	}
	#endregion

	void Start() {

	}

	void Update() {
		// Initializing plugins if there is an internet connection. The game may start without one (e.g. in a subway), so have to check it regularly.
		if (Application.internetReachability != NetworkReachability.NotReachable) {
			if (!_isInitialized) initPlugins();
		}
	}

	void OnApplicationPause(bool isPaused) {
		if (!_isInitialized) return;
		else {
			#if UNITY_IPHONE
			ShowInterstitialOnResume();
			#endif
		}
	}

	void OnApplicationQuit() {
		if (!_isInitialized) return;
	}

	private void initPlugins() {
		Chartboost.CreateWithAppId ("CHARTBOOST_APP_ID_IOS", "CHARTBOOST_APP_SIGNATURE_IOS");

		Chartboost.cacheMoreApps(CBLocation.Default);
		Chartboost.cacheRewardedVideo(CBLocation.locationFromName ("Video Interstitial"));
		Chartboost.cacheInterstitial(CBLocation.locationFromName ("Video Interstitial"));
		Chartboost.cacheInterstitial(CBLocation.locationFromName ("Static Interstitial"));
		Chartboost.cacheInterstitial(CBLocation.Default);


		if (Advertisement.isSupported) { // If runtime platform is supported...
			Advertisement.Initialize(GameConfig.UNITY_ADS_ID, GameConfig.UNITY_ADS_TEST_MODE); // ...initialize.
		}

		_isInitialized = true;
	}

	private void onChartBoostVideoLoadFailed (CBLocation location, CBImpressionError error) {
		Debug.Log ("Interstitial not loaded at location: " + location.ToString ());
		Debug.Log ("with error: " + error.GetType ());
	}

	public void showVideoAds ()
	{
		if (Application.internetReachability != NetworkReachability.NotReachable) {
			if (Advertisement.IsReady ())
			{
				Advertisement.Show ("video");
			} else if (Chartboost.hasInterstitial(CBLocation.locationFromName("Video Interstitial"))) {
				Chartboost.showInterstitial(CBLocation.locationFromName("Video Interstitial"));
			}
		}
	}

	public void showRewardedVideo () {
		if (Application.internetReachability != NetworkReachability.NotReachable) {
			if (Advertisement.IsReady ("rewardedVideo")) {
				const string RewardedZoneId = "rewardedVideo";
				var options = new ShowOptions { resultCallback = onUnityRewardedComplete };
				Advertisement.Show (RewardedZoneId, options);
				AudioManager.GetInstance ().Mute (true);
			} else if (Chartboost.hasRewardedVideo (CBLocation.locationFromName ("Video Interstitial"))) {
				Chartboost.didCompleteRewardedVideo += onRewardedVideoComplete;
				Chartboost.didCloseRewardedVideo += onRewardedVideoClose;
				Chartboost.showRewardedVideo (CBLocation.locationFromName ("Video Interstitial"));
				AudioManager.GetInstance ().Mute (true);
			} else {
				Debug.Log ("Something went wront");
			}
		}
	}
		
	private void onRewardedVideoComplete(CBLocation location, int points) {
		Chartboost.didCompleteRewardedVideo -= onRewardedVideoComplete;
		if (RewardedVideoCompleteEvent != null) {
			RewardedVideoCompleteEvent (null, EventArgs.Empty);
		}

		AudioManager.GetInstance ().Mute (false);
	}

	private void onRewardedVideoClose (CBLocation location) {
		AudioManager.GetInstance ().Mute (false);
	}

	#region Interstitials
	public void ShowInterstitialIngame() {
		if (Application.internetReachability == NetworkReachability.NotReachable) return;
		#if UNITY_IPHONE 
		if (_isInitialized) {

			if (Chartboost.hasInterstitial(CBLocation.locationFromName("Static Interstitial"))) {
				Chartboost.showInterstitial(CBLocation.locationFromName("Static Interstitial"));
			}
		}
		#endif
	}

	public void ShowStaticInterstitialIngameFirst () {
		if (Application.internetReachability == NetworkReachability.NotReachable) return;

		#if UNITY_IPHONE 
		if (_isInitialized) {
			if (_chartboostInterstitialIsCached || Chartboost.hasInterstitial(CBLocation.locationFromName("Static Interstitial"))) {
				Chartboost.didCloseInterstitial += onChartboostStartInterstitialClose;
				Chartboost.showInterstitial(CBLocation.locationFromName("Static Interstitial"));
				Chartboost.cacheInterstitial(CBLocation.locationFromName("Static Interstitial"));
			}
		}
		#endif
	}


	private void onChartboostStartInterstitialClose(CBLocation param) {
		Chartboost.didCloseInterstitial -= onChartboostStartInterstitialClose;

		if (StartGameInterstitalClose != null) {
			StartGameInterstitalClose (null, EventArgs.Empty);
		}
	}

	// Chartboost ads pause Unity. Want to show on-resume interstitials only if user returns to the app. Not after each ad.
	public void ShowInterstitialOnResume() {
		#if UNITY_IPHONE
		if (_isInitialized)
		{

		}

		#endif
	}
		
	// Show only on exit. If user click close button, interstitial closes and shows black screen.
	public void ShowInterstitialOnExit() {

	}
	#endregion

	public void ShowMoreGames() {
		if (Application.internetReachability == NetworkReachability.NotReachable) {
			#if UNITY_IPHONE
			if (Chartboost.hasMoreApps (CBLocation.Default))
				ShowInterstitialCharboostMoreGames ();
			else if (Advertisement.IsReady ()) {
				Advertisement.Show ("video");
			} else if (Chartboost.hasInterstitial (CBLocation.locationFromName ("Video Interstitial"))) {
				Chartboost.showInterstitial (CBLocation.locationFromName ("Video Interstitial"));
			} 
			#endif
			#if UNITY_WINRT
			Application.OpenURL(GameConfig.MOREGAMES_WP8);
			#endif
		} 
	}

	public void ShowFreeGames() {
		if (Application.internetReachability == NetworkReachability.NotReachable) {
			#if UNITY_IPHONE
			Application.OpenURL(GameConfig.ITUNES_COMPANY_URL);
			#endif
		} 
	}

	public void QuitGame() {
		if (_isQuitting) Application.Quit();
		else {
			#if UNITY_WINRT
			Application.Quit();
			#endif
			_isQuitting = true;
		}
	}

	#region Chartboost
	public void ShowInterstitialChartboost() {
		#if UNITY_IPHONE
		Chartboost.showInterstitial(CBLocation.Default);
		Chartboost.cacheInterstitial(CBLocation.Default);
		#endif
	}

	public void ShowInterstitialCharboostMoreGames() {
		#if UNITY_IPHONE
		Chartboost.showMoreApps(CBLocation.Default);
		Chartboost.cacheMoreApps(CBLocation.Default);
		#endif
	}

	private void onChartboostShowInterstitial(CBLocation param) {
		_chartboostIsDisplayed = true;
		_chartboostInterstitialIsCached = false;

		AudioManager.GetInstance ().Mute (true);
	}

	private void onChartboostShowMoreGames(CBLocation param) {
		_chartboostIsDisplayed = true;
	}

	private void onChartboostCacheInterstitial(CBLocation param) {
		_chartboostInterstitialIsCached = true;
	}

	private void onChartboostDismissInterstitial(CBLocation param) {
		_chartboostIsDisplayed = false;
		AudioManager.GetInstance ().Mute (false);
	}

	private void onChartboostDismissMoreApps(CBLocation param) {
		_chartboostIsDisplayed = false;
	}

	private void onChartBoostFailedToLoadInterstitial (CBLocation param, CBImpressionError error) {

	}

	private void onChartboostFailedToLoadRewardedVideo (CBLocation param, CBImpressionError error) {

	}
	#endregion

	void OnApplicationFocus(bool focusStatus) {

	}
}