using UnityEngine;
using System;
using System.Collections;

using strange.extensions.command.impl;

using Common.AssetBundleService;
using Common.ObjectPointer;
using Common.ResolutionUtility;

namespace Common.AssetBundleService.Controller
{
	public class LoadGameObjectCommand : Command
	{
		[Inject]
		public IAssetManagerService AssetManager { get; set; }

		#region Injected Properties

		[Inject]
		public string[] Params { get; set;}

		#endregion

		#region Outgoing Signal

		[Inject]
		public LoadGameObjectResponseSignal LoadGameObjectResponseSignal { get; set; }

		#endregion

		public override void Execute ()
		{
			Retain ();
			LoadGameObject ();
		}

		private void LoadGameObject ()
		{
			string bundlePath = Params [1];// + GetSuffix(ResolutionReference.resolution);

			ObjectPtr<GameObject> gameObjectPtr = new ObjectPtr<GameObject> ();

			AssetManager.LoadAssetBundle (Params[0], bundlePath, gameObjectPtr, OnGameObjectLoaded);
		}

		private string GetSuffix (ResolutionReference.Resolution resolution)
		{
			switch (resolution) {
			case ResolutionReference.Resolution.HD:
				return "";
			default: 
				return "_" + resolution.ToString ().ToLower ();
			}
		}

		private void OnGameObjectLoaded (bool success, ObjectPtr<GameObject> goPtr)
		{
			GameObject gameObject = goPtr == null ? null : goPtr.value;

			if (!success || gameObject == null) {
				Debug.LogError ("Failed to load game object at: " + Params[1]);
				return;
			}

			SendResponse (gameObject);
		}

		private void SendResponse (GameObject gameObject)
		{
			LoadGameObjectResponseSignal.Dispatch (gameObject);
			Release ();
		}
	}
}
