using UnityEngine;
using System.Collections;

using strange.extensions.command.impl;

using Common.AssetBundleService;
using Common.ObjectPointer;
using Common.ResolutionUtility;

namespace Common.AssetBundleService.Controller
{
	public class LoadTextureCommand : Command
	{
		[Inject]
		public IAssetManagerService AssetManager { get; set; }

		#region Injected Properties
		[Inject]
		public string[] Params {get;set;}

		#endregion

		#region Outgoing Signal

		[Inject]
		public LoadTextureResponseSignal LoadSpriteResponseSignal { get; set; }

		#endregion

		public override void Execute ()
		{
			Retain ();
			LoadTexture ();
		}

		private void LoadTexture ()
		{
			string bundlePath = Params [1];// + GetSuffix(ResolutionReference.resolution);

			ObjectPtr<Texture2D> imagePtr = new ObjectPtr<Texture2D> ();
			AssetManager.LoadAssetBundle (Params[0], bundlePath, imagePtr, OnTextureLoaded);
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

		private void OnTextureLoaded (bool success, ObjectPtr<Texture2D> imagePtr)
		{
			Texture2D image = imagePtr == null ? null : imagePtr.value;

			if (!success || image == null) {
				Debug.LogError ("Error loading texture");
				return;
			}

			LoadSpriteResponseSignal.Dispatch (image);
		}
	}
}