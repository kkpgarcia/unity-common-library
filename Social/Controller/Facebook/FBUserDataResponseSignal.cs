using System;
using strange.extensions.signal.impl;
using Common.Social.Model;

namespace Common.Social.Controller
{
	public class FBUserDataResponseSignal : Signal<bool, UserAccount>
	{
	}
}