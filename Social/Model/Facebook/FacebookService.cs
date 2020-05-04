using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Common.Singleton;
using Common.Social.Controller;
using Common.Social.Model;

using Facebook.Unity;

namespace Common.Social.Service
{
	public class FacebookService : PermanentMonoSingleton<FacebookService>, IAuthenticationService, ISocialService
	{
		[Inject]
		public AuthenticationResponseSignal ResponseSignal { get; set; }

		[Inject]
		public FBUserDataResponseSignal FBUserDataResponseSignal { get; set; }

		[Inject]
		public FBUserFriendsResponseSignal FBUserFriendsResponseSignal { get; set; }

		public void Initialize ()
		{
			if (!FB.IsInitialized) {
				FB.Init (InitCallback, OnHideUnity);
			} else {
				FB.ActivateApp ();
			}
		}

		private void InitCallback ()
		{
			if (FB.IsInitialized) {
				FB.ActivateApp ();
			} else {
				Debug.LogWarning ("Facebook SDK failed to initialize");
			}
		}

		private void OnHideUnity (bool isGameShown)
		{
			if (!isGameShown) {
				Time.timeScale = 0;
			} else {
				Time.timeScale = 1;
			}
		}

		private Action<PostResult> PostCompleteCallback { get; set; }

		private string imageDir = "social/";
		private string imageFileName = "fb_post";

		//Authentication
		private bool IsAuthenticating = false;
		private const float AUTHENTICATE_TIMEOUT_SECS = 60f;

		//End Session
		private bool IsLoggingOut = false;
		private const float ENDSESSION_TIMEOUT_SECS = 10f;

		//Validation
		private bool IsValidatingId = false;
		private const float VALIDATION_TIMEOUT_SECS = 10f;
		private List<Action> IdValidationCallbacks = new List<Action> ();

		//Request Processingn
		private bool IsProcessingRequest = false;
		private const float REQUEST_TIMEOUT_SECS = 30f;
		private const int REQUEST_MAX_RETRIES = 3;

		public override bool Init ()
		{
			Debug.Log ("Facebook Service Initialized!");
			Initialize ();
			return this;
		}

		public bool IsLoggedin ()
		{
			return FB.IsLoggedIn;
		}

		public bool IsInitialized ()
		{
			return FB.IsInitialized;
		}

		#region Authentication

		public void Authenticate ()
		{
			Debug.Log ("Facebook Service - Authentication");

			if (!FB.IsInitialized) {
				Debug.LogError ("Facebook Service has not yet initialized.");
				return;
			}

			StartCoroutine (WaitForAuthenticationTimeout ());

			FB.LogInWithReadPermissions (new List<string> (){ "public_profile", "email", "user_friends" }, AuthCallback);
		}

		public void EndSession (Action<bool> callback)
		{
			if (FB.IsInitialized) {
				if (FB.IsLoggedIn) {
					Debug.Log ("Facebook Service - Logging out current user");
					FB.LogOut ();
					StartCoroutine (EndSessionRoutine (callback));

					return;
				} else {
					Debug.LogWarning ("Facebook Service - There are no user logged in!");
				}
			} else {
				Debug.LogWarning ("Facebook Service - Facebook is not yet initialized!");
			}

			callback (false);
		}

		private IEnumerator EndSessionRoutine (Action<bool> callback)
		{
			float timeoutTime = Time.time + ENDSESSION_TIMEOUT_SECS;

			while (Time.time < timeoutTime) {

				Debug.Log ("Facebook Service - Logging out");

				if (!FB.IsLoggedIn) {
					Debug.Log ("Facebook Service - Logged out successfully!");
					callback (true);
					yield break;
				}
				yield return null;
			}

			if (FB.IsLoggedIn)
				callback (false);
			else
				callback (true);
		}

		private void AuthCallback (ILoginResult result)
		{
			Debug.Log (result.RawResult);

			if (result.Cancelled) {
				Debug.LogError ("Facebook Service - Login Canceled");
				IsAuthenticating = false;
				StopCoroutine (WaitForAuthenticationTimeout ());
				ResponseSignal.Dispatch (false);
			}

			if (FB.IsLoggedIn) {
				Debug.Log ("Facebook Service - Log in succeeded!");
				var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
				IsAuthenticating = false;
				StopCoroutine (WaitForAuthenticationTimeout ());
				ResponseSignal.Dispatch (true);
			} else {
				Debug.LogError ("Facebook Service - Log in failed: " + result.Error);
				ResponseSignal.Dispatch (false);
			}
		}

		private IEnumerator WaitForAuthenticationTimeout ()
		{
			float timeoutTime = Time.time + AUTHENTICATE_TIMEOUT_SECS;

			while (Time.time < timeoutTime) {
				if (!IsAuthenticating)
					yield break;
				yield return null;
			}

			if (IsAuthenticating) {
				Debug.LogError ("Facebook Service - Authentication timed out!");
				ResponseSignal.Dispatch (false);
			}
		}

		#endregion

		#region Authentication Methods


		#endregion

		#region Facebook Posting

		public void PostMessage (string message, Action<PostResult> callback)
		{
			Post (message, null, null, callback);
		}

		public void PostMessageWithLink (string message, string link, Action<PostResult> callback)
		{
			Post (message, link, null, callback);
		}

		public void PostMessageWithImage (string message, Texture2D image, Action<PostResult> callback)
		{
			PostMessageWithImageAndLink (message, image, null, callback);
		}

		public void PostMessageWithImageAndLink (string message, Texture2D image, string link, Action<PostResult> callback)
		{
			//TODO Works with Image upload. Both Image and Link should have a URI
			string localImagePath = Texture2DUtility.SaveImage (image, imageDir, imageFileName);
			Post (message, localImagePath, link, callback);
		}

		public void PostMessageWithImage (string message, string localImagePath, Action<PostResult> callback)
		{
			Post (message, localImagePath, null, callback);
		}

		public void Post (string message, string localImagePath, string link, Action<PostResult> callback)
		{
			string delMessage = message;
			string delLocalImagePath = localImagePath;
			string delLink = link;
			Action<PostResult> delCallback = callback;

			PostViaGraph (delMessage, delLocalImagePath, delLink, delCallback);

			//TODO Validation and callback
		}

		private void PostViaGraph (string message, string localImagePath, string link, Action<PostResult> callback)
		{
			Debug.Log ("Facebook Service - Post Via Graph");
			StartCoroutine (PostViaGraphCoroutine (message, localImagePath, link, callback));
		}

		private IEnumerator PostViaGraphCoroutine (string message, string localImagePath, string link, Action<PostResult> callback)
		{
			PostCompleteCallback = callback;

			Debug.Log ("Facebook Service - Handle Post Via Graph");

			Dictionary<string, string> parameters = new Dictionary<string, string> ();
			WWWForm form = new WWWForm ();
			parameters.Add ("message", message);

			bool hasImage = !string.IsNullOrEmpty (localImagePath);
			if (hasImage) {
				Debug.Log ("Has image");

				WWW imageWWW = new WWW ("file:///" + localImagePath);

				while (!imageWWW.isDone)
					yield return null;

				if (!string.IsNullOrEmpty (imageWWW.error)) {
					yield break;
				}
				byte[] imageBytes = imageWWW.bytes;

				form.AddBinaryData ("picture", imageBytes, "image.png");
			}

			if (!string.IsNullOrEmpty (link)) {
				Debug.Log ("Has link");
				form.AddField ("message", message);
				form.AddField ("link", link);

				string param = "me/feed";

				FB.API (param, HttpMethod.POST, HandlePostViaGraphResult, form);

				yield break;
			}

			if (!hasImage)
				FB.API ("me/", HttpMethod.POST, HandlePostViaGraphResult, parameters);
			else {
				FB.API ("me/photos", HttpMethod.POST, HandlePostViaGraphResult, form);
			}
		}

		private void HandlePostViaGraphResult (IGraphResult result)
		{
			bool success = string.IsNullOrEmpty (result.Error);

			Debug.Log (result.JSerialize ());
			Debug.Log ("Facebook Service - Handle Post Via Graph Result Successful: " + success);

			if (!string.IsNullOrEmpty (result.Error))
				Debug.LogError ("Facebook Service - Handle Post Via Graph Result: " + result.Error);

			if (PostCompleteCallback != null) {
				if (success)
					PostCompleteCallback (PostResult.Successful);
				else
					PostCompleteCallback (PostResult.Failed);
			}
		}

		#endregion

		#region Get methods

		public void GetFacebookUserData ()
		{
			Debug.Log ("FacebookService - Get Facebook user data");

			if (!FB.IsLoggedIn) {
				Debug.LogError ("Facebook Service - Get User Data failed! FB is not logged in!");
			}

			var req = new Dictionary<string, string> ();
			req.Add ("fields", "id,location,first_name,last_name");

			FB.API ("me", HttpMethod.GET, OnFacebookUserDataResponse, req);
		}

		private void OnFacebookUserDataResponse (IGraphResult result)
		{
			IDictionary<string, object> data = result.ResultDictionary;

			if (data.ContainsKey ("id")) {
				string facebookId = (string)data ["id"];
				string fbFirstName = "";
				string fbLastName = "";
				string locationString = "";

				if (data.ContainsKey ("first_name")) {
					fbFirstName = (string)data ["first_name"];
				}

				if (data.ContainsKey ("last_name")) {
					fbLastName = (string)data ["last_name"];
				}

				if (data.ContainsKey ("location")) {
					//TODO
					locationString = (string)data ["location"];
					Debug.Log ((string)data ["location"]);
				}

				UserAccount UserAccount = new UserAccount {
					FirstName = fbFirstName,
					LastName = fbLastName,
					Id = facebookId,
					Location = locationString
				};

				FBUserDataResponseSignal.Dispatch (true, UserAccount);
			} else {
				Debug.LogError ("Facebook Service - User Data Response Failed: " + result.Error);
				FBUserDataResponseSignal.Dispatch (false, null);
			}
		}


		public void GetProfilePhoto (string facebookId, Action<Texture2D> callback)
		{
			string publicProfilePhotoUrl = "http://graph.facebook.com/v2.6/" + facebookId + "/picture?height=200&width=200&redirect=false";
			StartCoroutine (DownloadPhotoFromJSON (publicProfilePhotoUrl, callback));

			Debug.Log ("Facebook Service - Getting FB Profile Photo: " + facebookId);
		}

		IEnumerator DownloadPhotoFromJSON (string url, Action<Texture2D> callback)
		{
			Debug.Log ("Facebook Service - DownloadPhotoFromJSON");

			WWW result = new WWW (url);
			yield return result;
			string text = result.text;

			if (result.error != null) {
				Debug.LogError (result.error);
				//TODO Send Error signal
			}

			Dictionary<string, object> dict = JsonUtil.Deserialize<Dictionary<string, object>> (text);

			if (dict != null && dict.ContainsKey ("data")) {
				Dictionary<string, object> dataDict = dict ["data"] as Dictionary<string, object>;
				string picUrl = (string)dataDict ["url"];

				if (picUrl.Substring (0, 5) == "https")
					picUrl = picUrl.Remove (4, 1);
				
				StartCoroutine (DownloadPhoto (picUrl, callback));

			} else {
				callback (null);
			}
		}

		IEnumerator DownloadPhoto (string url, Action<Texture2D> callback)
		{
			Debug.Log ("Facebook Service - Download Photo");

			WWW www = new WWW (url);
			yield return www;
			TextureFormat format = TextureFormat.RGBA32;

			Texture2D image = new Texture2D (200, 200, format, false);

			www.LoadImageIntoTexture (image);
			if (www.error != null) {
				Debug.LogError (www.error);
			}

			callback (image);
		}

		#endregion

		#region Friends

		/// <summary>
		/// NOTE: CURRENTLY NOT WORKING
		/// 
		/// Invites the friends to join.
		/// </summary>
		/// <param name="message">Message.</param>
		public void InviteFriendsToJoin (string message)
		{
			string delMessage = message;

			Action validationCallback = () => {
				Dictionary<string, string> options = new Dictionary<string, string> ();
				options.Add ("title", "Invite friends to play Hop Pico Piko!");
				options.Add ("message", delMessage);
				options.Add ("filters", "app_non_users");
			};
		}

		public void FetchAppFriends ()
		{ 
			Dictionary<string, string> formData = new Dictionary<string, string> ();
			formData.Add ("fields", "id,name");

			FB.API ("me/friends", HttpMethod.GET, ParseAppFriends, formData);
		}

		private void ParseAppFriends (IGraphResult result)
		{
			Debug.Log (result.RawResult);
			StartCoroutine (ParseAppFriendsRoutine (result.ResultDictionary));
		}

		private IEnumerator ParseAppFriendsRoutine (IDictionary<string, object> resultDictionary)
		{
			Debug.Log ("Facebook Service - Parsing friends");

			List<UserAccount> friends = new List<UserAccount> ();
			if (resultDictionary != null) {
				object[] dataArray = JsonUtil.Deserialize<object[]> (resultDictionary ["data"].JSerialize ());

				for (var i = 0; i < dataArray.Length; i++) {
					Dictionary<string, object> dataDict = dataArray [i] as Dictionary<string, object>;
					UserAccount account = new UserAccount ();

					if (dataDict.ContainsKey ("id")) {
						account.Id = (string)dataDict ["id"];
					}

					friends.Add (account);

					yield return null;
				}

				FBUserFriendsResponseSignal.Dispatch (friends);
			} else {
				Debug.LogError ("Facebook Service - Error parsing friends");
				FBUserFriendsResponseSignal.Dispatch (null);
			}
		}

		#endregion
	}
}