using UnityEngine;
using System;
using System.Collections;

using strange.extensions.command.impl;

using Common.Social.Service;
using Common.Social.Model;

using Facebook.Unity;

namespace Common.Social.Controller
{
	public class AuthenticationRequestCommand : Command
	{
		[Inject (SocialServiceType.FACEBOOK)]
		public IAuthenticationService AuthenticationService { get; set; }

		[Inject]
		public AuthenticationResultSignal AuthenticationResultSignal { get; set; }

		private bool IsAuthenticating;

		public override void Execute ()
		{
			Debug.Log ("Authentication Request - Execute");

			if (IsAuthenticating)
				return;
			
			IsAuthenticating = true;
			Retain ();
			AuthenticationService.ResponseSignal.AddListener (OnAuthenticate);
			AuthenticationService.Authenticate ();
		}

		private void OnAuthenticate (bool success)
		{
			Debug.Log ("Authentication Request - Is success: " + success);

			if (!success) {
				Debug.LogError ("Authentication Request - Authentication failed!");
				AuthenticationResultSignal.Dispatch (false);
				CleanUp ();
				return;
			}

			AuthenticationResultSignal.Dispatch (true);
			CleanUp ();
		}

		private void CleanUp ()
		{
			AuthenticationService.ResponseSignal.RemoveListener (OnAuthenticate);
			Release ();
		}
	}
}