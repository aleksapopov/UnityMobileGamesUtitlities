using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Localization {

	public class CloudConnector : MonoBehaviour
	{
		// -- Complete the following fields. --
		private string webServiceUrl = "";
		private string spreadsheetId = ""; // If this is a fixed value could also be setup on the webservice to save POST request size.
		private string servicePassword = "";
		private float timeOutLimit = 30f;
		public bool usePOST = true;
		// --
	
		private static CloudConnector _Instance;
		public static CloudConnector Instance {
			get {
				return _Instance ?? (_Instance = new GameObject("CloudConnector").AddComponent<CloudConnector>());
			}
		}
	
		private UnityWebRequest www;
	
		public void CreateRequest(Dictionary<string, string> form) {
			form.Add("ssid", spreadsheetId);
			form.Add("pass", servicePassword);
		
			if (usePOST) {
				CloudConnectorCore.UpdateStatus("Establishing Connection at URL " + webServiceUrl);
				www = UnityWebRequest.Post(webServiceUrl, form);	
			}
			// Use GET. 
			else {
				string urlParams = "?";
				foreach (KeyValuePair<string, string> item in form) {
					urlParams += item.Key + "=" + item.Value + "&";
				}
				CloudConnectorCore.UpdateStatus("Establishing Connection at URL " + webServiceUrl + urlParams);
				www = UnityWebRequest.Get(webServiceUrl + urlParams);
			}
			StartCoroutine(ExecuteRequest(form));
		}
	
		IEnumerator ExecuteRequest(Dictionary<string, string> postData) {
			www.Send();
		
			float elapsedTime = 0.0f;
		
			while (!www.isDone) {
				elapsedTime += Time.deltaTime;			
				if (elapsedTime >= timeOutLimit) {
					CloudConnectorCore.ProcessResponse("TIME_OUT", elapsedTime);
					break;
				}
			
				yield return null;
			}
		
			if (www.isError) {
				CloudConnectorCore.ProcessResponse(CloudConnectorCore.MSG_CONN_ERR + "Connection error after " + elapsedTime.ToString() + " seconds: " + www.error, elapsedTime);
				yield break;
			}	
		
			CloudConnectorCore.ProcessResponse(www.downloadHandler.text, elapsedTime);
		}
	}
}

	