using UnityEngine;
using System.Collections;
using System;
using Common.Social.Model;
using Common.Social.Controller;

namespace Common.Social.Service
{
	public enum PostResult
	{
		Failed,
		Successful,
		Cancelled,
		Unavailable
	}

	public interface ISocialService
	{
		FBUserDataResponseSignal FBUserDataResponseSignal { get; set; }

		FBUserFriendsResponseSignal FBUserFriendsResponseSignal { get; set; }

		bool IsInitialized ();

		bool IsLoggedin ();

		void EndSession (Action<bool> callback);

		void GetFacebookUserData ();

		void PostMessage (string message, Action<PostResult> callback);

		void PostMessageWithLink (string message, string link, Action<PostResult> callback);

		void PostMessageWithImage (string message, Texture2D image, Action<PostResult> callback);

		/// <summary>
		/// Not working at the moment. If you want to use it, then set the image parameter as null
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="image">Image.</param>
		/// <param name="link">Link.</param>
		/// <param name="callback">Callback.</param>
		void PostMessageWithImageAndLink (string message, Texture2D image, string link, Action<PostResult> callback);

		void PostMessageWithImage (string message, string localImagePath, Action<PostResult> callback);

		void Post (string message, string localImagePath, string link, Action<PostResult> callback);

		void GetProfilePhoto (string facebookId, Action<Texture2D> callback);

		void FetchAppFriends ();

	}
}
