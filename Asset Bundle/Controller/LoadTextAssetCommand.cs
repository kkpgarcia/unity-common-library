using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using strange.extensions.command.impl;

using Common.Main.Controller;
using Common.AssetBundleService;
using Common.ObjectPointer;

namespace Common.AssetBundleService.Controller
{
	public class LoadTextAssetCommand : CoroutineCommand
	{
		[Inject]
		public IAssetManagerService AssetManager { get; set; }

		private List<TextAsset> TextAssets = new List<TextAsset> ();
		private int TextAssetCount;

		#region Injected Properties

		[Inject]
		public string AssetPath { get; set; }

		[Inject]
		public List<string> URL { get; set; }

		#endregion

		#region Outgoing Signal

		[Inject]
		public LoadTextAssetResponseSignal LoadTextAssetResponseSignal { get; set; }

		#endregion

		public override void Execute ()
		{
			Retain ();
			StartCoroutine (LoadTextAsset ());
		}

		IEnumerator LoadTextAsset ()
		{
			List<string> textPath = URL;
			TextAssetCount = textPath.Count;

			for (int i = 0; i < textPath.Count; i++) {

				ObjectPtr<TextAsset> textAssetPtr = new ObjectPtr<TextAsset> ();
				AssetManager.LoadAssetBundle (AssetPath + textPath [i], textAssetPtr, OnTextAssetLoaded);
				yield return null;
			}
		}

		private void OnTextAssetLoaded (bool success, ObjectPtr<TextAsset> textPtr)
		{
			TextAsset textAsset = textPtr == null ? null : textPtr.value;

			if (!success || textAsset == null) {
				Debug.LogError ("Failed to load text asset at: " + URL);
				return;
			}

			TextAssets.Add (textAsset);
			TextAssetCount--;

			if (TextAssetCount == 0) {
				SendResponse ();
			}
		}

		private void SendResponse ()
		{
			LoadTextAssetResponseSignal.Dispatch (TextAssets);
			Release ();
		}
	}
}