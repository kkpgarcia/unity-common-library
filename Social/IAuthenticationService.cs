using UnityEngine;
using System.Collections;
using Common.Social.Controller;

namespace Common.Social.Service
{
	public interface IAuthenticationService
	{
		void Authenticate ();

		AuthenticationResponseSignal ResponseSignal { get; }
		
	}
}
