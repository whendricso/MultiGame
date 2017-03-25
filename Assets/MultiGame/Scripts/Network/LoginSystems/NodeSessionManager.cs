using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

namespace MultiGame {

	public class NodeSessionManager : MultiModule {

		public Rect guiArea = new Rect(.01f,.01f,.98f,.98f);

		public string registrationURL = "";
		public string loginURL = "";
		public string logoutURL = "";
		public string rootURL = "";

		public bool debug = false;

		private string userName = "";
		//TODO: FIX PASSWORD SECURITY!
		private string pass = "";

		private WWWForm form;

		[System.NonSerialized]
		public string cookie;

		public UserSession sesh;

		[System.Serializable]
		public class UserSession {
			public string session;
		}

		void OnGUI () {
			GUILayout.BeginArea(new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height),"","box");

			GUILayout.BeginHorizontal();
			GUILayout.Label("User name:");
			userName = GUILayout.TextField(userName);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Password:");
			pass = GUILayout.TextField(pass);
			GUILayout.EndHorizontal();

			//if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(pass)) {
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Log in"))
					LogIn();
				if (GUILayout.Button("Register"))
					Register();
				GUILayout.EndHorizontal();
			//}

			GUILayout.EndArea();
		}

		public void LogIn () {
			form = new WWWForm();
			form.AddField("username",userName);
			form.AddField("password",pass);
			StartCoroutine(AttemptLogin());
		}

		public IEnumerator AttemptLogin () {
			using (WWW www = new WWW(loginURL, form)) {
				yield return www;
				if (!string.IsNullOrEmpty( www.error))
					Debug.LogError(www.error);
				else {
					sesh = JsonUtility.FromJson<UserSession>(www.text);
					if (debug) {
						foreach (KeyValuePair<string, string> kvp in www.responseHeaders) {
							Debug.Log(kvp.Key + ", " + kvp.Value);
						}
						Debug.Log(www.text);
						TestRootHit();
					}
					cookie = www.responseHeaders["SET-COOKIE"];
				}
			}
		}

		public void Register () {
			form = new WWWForm();
			form.AddField("username",userName);
			form.AddField("password",pass);
			StartCoroutine(AttemptRegistration());
		}

		public IEnumerator AttemptRegistration () {
			using (WWW www = new WWW(registrationURL, form)) {
				yield return www;
				if (!string.IsNullOrEmpty(www.error))
					Debug.LogError(www.error);
				else {
					sesh = JsonUtility.FromJson<UserSession>(www.text);
					if (debug) {
						foreach (KeyValuePair<string, string> kvp in www.responseHeaders) {
							Debug.Log(kvp.Key + ", " + kvp.Value);
						}
						Debug.Log(www.text);
						TestRootHit();
					}
					cookie = www.responseHeaders["SET-COOKIE"];
				}
			}
		}

		public void TestRootHit () {
			StartCoroutine(RunRootTest());
		}

		public IEnumerator RunRootTest () {
			Dictionary<string, string> head = new Dictionary<string, string>();
			head.Add("my_cookie",sesh.session);
			using (WWW www = new WWW(rootURL, null, head)) {
				yield return www;
				if (!string.IsNullOrEmpty( www.error))
					Debug.LogError(www.error);
				else {
					
					Debug.Log("Node Session Manager got a 'connect.sid' back from the server of " + ParseCookieValue());
				}
				if (debug) {
					Debug.Log(www.text);
				}
			}
		}

		/// <summary>
		/// Parses the cookie value, returning either null (if no cookie) or the 'connect.sid' value from the server.
		/// </summary>
		/// <returns>The cookie value.</returns>
		public string ParseCookieValue () {
			string _ret = "";

			if (string.IsNullOrEmpty(cookie))
				_ret = null;
			else {
				_ret = cookie.Split(';')[0];
				Debug.Log(_ret);
				_ret = _ret.Split('=')[1];
			}

			return _ret;
		}

		/// <summary>
		/// Returns true if a cookie has been generated for the session
		/// </summary>
		/// <returns><c>true</c>, if session active was checked, <c>false</c> otherwise.</returns>
//		public bool CheckSessionActive () {
//			if (!string.IsNullOrEmpty(cookie))
//				return true;
//			else
//				return false;
//		}
	}
}