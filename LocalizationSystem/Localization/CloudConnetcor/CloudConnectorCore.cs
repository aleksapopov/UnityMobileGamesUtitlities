using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Localization {

	public static class CloudConnectorCore
	{
		static bool debugMode = true;
	
		public enum QueryType
		{
			// Create
			createObject,		// Adds a row to a existing table type with the specified fields.
			createTable,		// Adds worksheet to the file, with the specified headers.
			// Retrieve
			getObjects,			// Returns an array of objects by field.
			getTable,			// Returns a complete table (worksheet) from the spreadsheet.
			getAllTables,		// Returns all worksheets on the spreadsheet.
			// Update
			updateObjects,		// Updates a field in one or more object(s) specified by field.
			// Delete
			deleteObjects		// Deletes object(s) specified by field.
		}
	
		public const string MSG_OBJ_CREATED_OK = "OBJ_CREATED_OK";
		public const string MSG_TBL_CREATED_OK = "TBL_CREATED_OK";
		public const string MSG_OBJ_DATA = "OBJ_DATA";
		public const string MSG_TBL_DATA = "TBL_DATA";
		public const string MSG_TBLS_DATA = "TBLS_DATA";
		public const string MSG_OBJ_UPDT = "OBJ_UPT_OK";
		public const string MSG_OBJ_DEL = "OBJ_DEL_OK";
		public const string MSG_MISS_PARAM = "MISSING_PARAM";
		public const string MSG_CONN_ERR = "CONN_ERROR";
		public const string MSG_TIME_OUT = "TIME_OUT";
		public const string TYPE_END = "_ENDTYPE\n";
		public const string TYPE_STRT = "TYPE_";
		public const string MSG_BAD_PASS = "PASS_ERROR";
	
		static string currentStatus = "";
	
		// Suscribe to this event if want to handle the response as it comes.
		public class CallBackEventRaw : UnityEvent<string> {}
		public static CallBackEventRaw rawResponseCallback = new CallBackEventRaw();
	
		// Suscribe to this event if want the response pre processed.
		public class CallBackEventProcessed : UnityEvent<QueryType, List<string>, List<string>> {}
		public static CallBackEventProcessed processedResponseCallback = new CallBackEventProcessed();
	
	
		public static void CreateObject(Dictionary<string, string> fields, string objTypeName, bool runtime = true)
		{
			Dictionary<string, string> form = fields;
			form.Add("action", QueryType.createObject.ToString());
			form.Add("isJson", "false");
			form.Add("type", objTypeName);
		
			SendRequest(form, runtime);
		}
	

		public static void CreateObject(string jsonObject, string objTypeName, bool runtime = true)
		{
			Dictionary<string, string> form = new Dictionary<string, string>();
			form.Add("action", QueryType.createObject.ToString());
			form.Add("isJson", "true");
			form.Add("type", objTypeName);
			form.Add("jsonData", jsonObject);
		
			SendRequest(form, runtime);
		}
	

		public static void CreateTable(string[] headers, string tableTypeName, bool runtime = true)
		{
			Dictionary<string, string> form = new Dictionary<string, string>();
			form.Add("action", QueryType.createTable.ToString());
			form.Add("type", tableTypeName);
			form.Add("num", headers.Length.ToString());
		
			for (int i = 0; i < headers.Length; i++)
			{
				form.Add("field" + i.ToString(), headers[i]);
			}
		
			SendRequest(form, runtime);
		}
	

		public static void GetObjectsByField(string objTypeName, string searchFieldName, string searchValue, bool runtime = true)
		{
			Dictionary<string, string> form = new Dictionary<string, string>();
			form.Add("action", QueryType.getObjects.ToString());
			form.Add("type", objTypeName);
			form.Add(searchFieldName, searchValue);
			form.Add("search", searchFieldName);
		
			SendRequest(form, runtime);
		}
	

		public static void GetTable(string tableTypeName, bool runtime = true)
		{
			Dictionary<string, string> form = new Dictionary<string, string>();
			form.Add("action", QueryType.getTable.ToString());
			form.Add("type", tableTypeName);
		
			SendRequest(form, runtime);
		}
	

		public static void GetAllTables(bool runtime = true)
		{
			Dictionary<string, string> form = new Dictionary<string, string>();
			form.Add("action", QueryType.getAllTables.ToString());
		
			SendRequest(form, runtime);
		}
	

		public static void UpdateObjects( string objTypeName,
			string searchFieldName,
			string searchValue,
			string fieldNameToUpdate,
			string updateValue,
			bool runtime = true )
		{
			Dictionary<string, string> form = new Dictionary<string, string>();
			form.Add("action", QueryType.updateObjects.ToString());
			form.Add("type", objTypeName);
			form.Add("searchField", searchFieldName);
			form.Add("searchValue", searchValue);
			form.Add("updtField", fieldNameToUpdate);
			form.Add("updtValue", updateValue);
		
			SendRequest(form, runtime);
		}
	

		public static void DeleteObjects(string objTypeName, string searchFieldName, string searchValue, bool runtime = true)
		{
			Dictionary<string, string> form = new Dictionary<string, string>();
			form.Add("action", QueryType.deleteObjects.ToString());
			form.Add("type", objTypeName);
			form.Add(searchFieldName, searchValue);
			form.Add("search", searchFieldName);
		
			SendRequest(form, runtime);
		}
	
		public static void UnpackJson(string response)
		{
			List<string> objTypeName = new List<string>();
			List<string> jsonData = new List<string>();
			string parsed = "";
			QueryType returnType = QueryType.getObjects;
		
			// Response for GetObjectsByField()
			if (response.StartsWith(MSG_OBJ_DATA)) 
			{
				parsed = response.Substring(MSG_OBJ_DATA.Length + 1);
				objTypeName.Add(parsed.Substring(0, parsed.IndexOf(TYPE_END)));
				jsonData.Add(parsed.Substring(parsed.IndexOf(TYPE_END) + TYPE_END.Length));
				returnType = QueryType.getObjects;
			}
		
			// Response for GetTable()
			if (response.StartsWith(MSG_TBL_DATA))
			{
				parsed = response.Substring(MSG_TBL_DATA.Length + 1);
				objTypeName.Add(parsed.Substring(0, parsed.IndexOf(TYPE_END)));
				jsonData.Add(parsed.Substring(parsed.IndexOf(TYPE_END) + TYPE_END.Length));
				returnType = QueryType.getTable;
			}
		
			// Response for GetAllTables()
			if (response.StartsWith(MSG_TBLS_DATA))
			{
				parsed = response.Substring(MSG_TBLS_DATA.Length + 1);
			
				// First split creates substrings containing type and content on each one.
				string[] separator = new string[] { TYPE_STRT };
				string[] split = parsed.Split(separator, System.StringSplitOptions.None);
			
				// Second split gives the final lists of type names and data on different lists.
				separator = new string[] { TYPE_END };
				for (int i = 0; i < split.Length; i++)
				{
					if (split[i] == "")
						continue;
				
					string[] secSplit = split[i].Split(separator, System.StringSplitOptions.None);
					objTypeName.Add(secSplit[0]);
					jsonData.Add(secSplit[1]);
				}
				returnType = QueryType.getAllTables;
			}
		
			// The callback returns:
			// * The return type, to identify which was the original request.
			// * An array of types names.
			// * An array of json strings (each string containing an array of objects).
			processedResponseCallback.Invoke(returnType, objTypeName, jsonData);
		}
	
		static void SendRequest(Dictionary<string, string> form, bool runtime = true)
		{
			if (runtime)
			{
				CloudConnector.Instance.CreateRequest(form);
			}
			#if UNITY_EDITOR
			else
			{
				CloudConnectorEditor.CreateRequest(form);
			}
			#endif
		}
	
		public static void ProcessResponse(string response, float time)
		{
			//rawResponseCallback.Invoke(response);
			//return;
		
			bool unpacked = false;
		
			if (response.StartsWith(MSG_OBJ_DATA))
			{
				UnpackJson(response);
				response = MSG_OBJ_DATA;
				unpacked = true;
			}
		
			if (response.StartsWith(MSG_TBL_DATA))
			{
				UnpackJson(response);
				response = MSG_TBL_DATA;
				unpacked = true;
			}
		
			if (response.StartsWith(MSG_TBLS_DATA))
			{
				UnpackJson(response);
				response = MSG_TBLS_DATA;
				unpacked = true;
			}
		
			if (response.StartsWith(MSG_BAD_PASS))
			{
				response = MSG_BAD_PASS;
			}
		
			string errorMsg = "Undefined connection error.";
			if (response.StartsWith(MSG_CONN_ERR))
			{
				errorMsg = response.Substring(MSG_CONN_ERR.Length);
				response = MSG_CONN_ERR;
			}
		
			string timeApendix = " Time: " + time.ToString();
			string logOutput = "";
		
			switch (response)
			{
			case MSG_OBJ_CREATED_OK:
				logOutput ="Object saved correctly.";
				break;
			
			case MSG_TBL_CREATED_OK:
				logOutput = "Worksheet table created correctly.";
				break;
			
			case MSG_OBJ_DATA:
				logOutput = "Object data received correctly.";
				break;
			
			case MSG_TBL_DATA:
				logOutput = "Table data received correctly.";
				break;
			
			case MSG_TBLS_DATA:
				logOutput = "All DB data received correctly.";
				break;
			
			case MSG_OBJ_UPDT:
				logOutput = "Object updated correctly.";
				break;
			
			case MSG_OBJ_DEL:
				logOutput = "Object deleted from DB.";
				break;
			
			case MSG_MISS_PARAM:
				logOutput = "Parsing Error: Missing parameters.";
				break;
			
			case MSG_TIME_OUT:
				logOutput = "Operation timed out, connection aborted. Check your internet connection and try again.";
				break;	
			
			case MSG_CONN_ERR:
				logOutput = errorMsg;
				break;
			
			case MSG_BAD_PASS:
				logOutput = "Error: password incorrect.";
				break;
			
			default:
				logOutput = "Undefined server response: \n" + response;
				break;
			}
		
			UpdateStatus(logOutput + timeApendix);
		
			if (!unpacked)
				rawResponseCallback.Invoke(response);
		}
	
		public static void UpdateStatus(string status)
		{
			currentStatus = status;
		
			if (debugMode)
				Debug.Log(currentStatus);
		}
	}
}
