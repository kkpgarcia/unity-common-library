using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

using Common.Singleton;
using Common.ObjectPointer;
using Common.Task;
using Common.Task.Model;
using Common.AssetBundleService.Model;

namespace Common.AssetBundleService
{
	public class AssetManager : PermanentMonoSingleton<AssetManager>, IAssetManagerService
	{
		private class AssetBundleRef
		{
			public AssetBundle AssetBundleInst = null;
			public int Version;
			public string Url;
			public AssetType Type;

			public AssetBundleRef (string strUrlIn, int intVersionIn, AssetType type)
			{
				Url = strUrlIn;
				Version = intVersionIn;
				Type = type;
			}
		};

		private class AssetRef
		{
			public Object Asset;
			public string Url;
			public bool IsDiskAsset = false;
			public AssetType Type;

			public AssetRef (Object asset, string url = null, bool isDiskAsset = false, AssetType type = AssetType.Undeclared)
			{
				Asset = asset;
				Url = url;
				IsDiskAsset = isDiskAsset;
				Type = type;
			}
		}

		static string assetPath;
		const string streamingDataPath = "Streaming Data/";
		bool isLoading = false;

		//Dictionary to hold the asset bundle references
		private Dictionary<string, AssetBundleRef> dictAssetBundleRefs = new Dictionary<string, AssetBundleRef> ();
		private Dictionary<string, AssetBundle> PersistentAssetBundles = new Dictionary<string, AssetBundle> ();
		private Dictionary<string, AssetBundle> SemiPersistentAssetBundles = new Dictionary<string, AssetBundle> ();
		private List<AssetRef> NonPersistentObjects = new List<AssetRef> ();
		private List<AssetRef> PersistentObjects = new List<AssetRef> ();
		private List<AssetRef> SemiPersistentObjects = new List<AssetRef> ();

		private TaskDictionary Tasks = new TaskDictionary ();
		private bool IsBusy = false;
		public int Version;

		[SerializeField]
		private bool LogAssetBundlePath;

		public void LoadAssetBundle<T>(string name, string url, ObjectPtr<T> prefab, Action<bool, ObjectPtr<T>> callback, AssetType type) where T : Object
		{
			Task<T> task = new Task<T> ();
			task.TaskType = TaskType.Assetbundle;
			task.Name = name;
			task.Url = url;
			task.TType = typeof(T);
			task.Prefab = prefab;
			task.GenericCallback = callback;
			task.Persistence = Persistence.None;
			task.AssetType = type;

			Tasks.Add (task, (int)task.TaskType);
		}

		public void LoadAssetBundle<T> (string url, ObjectPtr<T> prefab, Action<bool, ObjectPtr<T>> callback, AssetType type) where T : Object
		{
			Task<T> task = new Task<T> ();
			task.TaskType = TaskType.Assetbundle;
			task.Url = url;
			task.TType = typeof(T);
			task.Prefab = prefab;
			task.GenericCallback = callback;
			task.Persistence = Persistence.None;
			task.AssetType = type;

			Tasks.Add (task, (int)task.TaskType);
		}

		public void LoadPersistentAssetBundle<T> (string url, ObjectPtr<T> prefab, Action<bool, ObjectPtr<T>> callback, AssetType type) where T : Object
		{
			Task<T> task = new Task<T> ();
			task.TaskType = TaskType.Assetbundle;
			task.Url = url;
			task.TType = typeof(T);
			task.Prefab = prefab;
			task.GenericCallback = callback;
			task.Persistence = Persistence.Full;
			task.AssetType = type;

			Tasks.Add (task, (int)task.TaskType);
		}

		public void LoadSemiPersistentAssetBundle<T> (string url, ObjectPtr<T> prefab, Action<bool, ObjectPtr<T>> callback, AssetType type) where T : Object
		{
			Task<T> task = new Task<T> ();
			task.TaskType = TaskType.Assetbundle;
			task.Url = url;
			task.TType = typeof(T);
			task.Prefab = prefab;
			task.GenericCallback = callback;
			task.Persistence = Persistence.Partial;
			task.AssetType = type;

			Tasks.Add (task, (int)task.TaskType);
		}

		public void LoadTextureFromFullPath (string fullPath, Action<bool, Texture2D> callback, bool unloadOnSceneChange, bool isLocalFile)
		{
			Task<Texture2D> task = new Task<Texture2D> ();
			task.TaskType = TaskType.Texture;
			task.Url = fullPath;
			task.TextureCallback = callback;
			task.Persistence = unloadOnSceneChange ? Persistence.None : Persistence.Full;
			task.IsLocalFile = isLocalFile;

			Tasks.Add (task, (int)task.TaskType);
		}

		public void LoadTextureFromPersistentPath (string subPath, Action<bool, Texture2D> callback, bool unloadOnSceneChange)
		{
			string fullPath = Path.Combine (Application.persistentDataPath, subPath);
			LoadTextureFromFullPath (fullPath, callback, unloadOnSceneChange, true);
		}

		public void LoadTextureFromUrl (string url, Action<bool, Texture2D> callback, bool unloadOnSceneChange, bool cache = false, int maxWidth = 0, int maxHeight = 0)
		{
			if (cache) {
				Task<Texture2D> task = new Task<Texture2D> ();
				task.TaskType = TaskType.CacheableTexture;
				task.Url = url;
				task.TextureCallback = callback;
				task.Persistence = unloadOnSceneChange ? Persistence.None : Persistence.Full;
				task.MaxTextureWidth = maxWidth;
				task.MaxTextureHeight = maxHeight;

				Tasks.Add (task, (int)task.TaskType);
			} else
				LoadTextureFromFullPath (url, callback, unloadOnSceneChange, false);
		}

		private IEnumerator LoadTextureFromCacheOrDownload (string url, Action<bool, Texture2D> callback, bool unloadOnSceneChange, int maxWidth, int maxHeight)
		{
			string localFilePath = Path.Combine (Application.temporaryCachePath, url + ".png");
			bool fileCached = File.Exists (localFilePath);
			WWW www = null;

			using (www = fileCached ? new WWW ("file:///" + localFilePath) : new WWW (url)) {
				yield return www;

				if (!string.IsNullOrEmpty (www.error)) {
					callback (false, null);
					yield break;
				}

				Texture2D texture = www.texture;

				if (!fileCached)
					Texture2DUtility.SaveImage (texture, Application.temporaryCachePath, url, false);

				callback (true, texture);
			}
		}

		public void UnloadAssetBundle (string url)
		{
			UnloadAssetBundle (url, Version, true);
		}

		public void UnloadAsset (string url)
		{
			AssetRef assetRef = PersistentObjects.Find (x => x.Url == url);

			if (assetRef != null) {
				Destroy (assetRef.Asset);
				PersistentObjects.Remove (assetRef);
			}
		}

		public void UnloadAllNonPersistent ()
		{
			UnloadAllNonPersistentBundles ();
			UnloadOtherAssets ();
		}

		public void UnloadAllNonPersistentBundlesOfType (AssetType type)
		{
			List<string> urlsToRemove = new List<string> ();

			foreach (KeyValuePair<string, AssetBundleRef> pair in dictAssetBundleRefs) {
				if (PersistentAssetBundles.ContainsKey (pair.Key) || SemiPersistentAssetBundles.ContainsKey (pair.Key))
					continue;

				if (pair.Value.Type == type)
					urlsToRemove.Add (pair.Key);
			}

			foreach (string url in urlsToRemove)
				UnloadAssetBundleWithActualAddress (url, true);
		}

		public void UnloadAllPersistentAssetBundles ()
		{
			foreach (string url in PersistentAssetBundles.Keys) {
				dictAssetBundleRefs.Remove (url);
				AssetBundle bundle = PersistentAssetBundles [url];
				if (bundle)
					bundle.Unload (true);
			}

			PersistentAssetBundles.Clear ();
		}

		public void UnloadAllPersistentAssets ()
		{
			foreach (AssetRef assetRef in PersistentObjects) {
				if (assetRef.IsDiskAsset)
					Resources.UnloadAsset (assetRef.Asset);
				else
					Destroy (assetRef.Asset);
			}
			PersistentObjects.Clear ();
		}

		public void UnloadAllSemiPersistentAssetBundles ()
		{
			foreach (string url in SemiPersistentAssetBundles.Keys) {
				dictAssetBundleRefs.Remove (url);
				AssetBundle bundle = SemiPersistentAssetBundles [url];
				if (bundle)
					bundle.Unload (true);
			}

			SemiPersistentAssetBundles.Clear ();
		}

		public void UnloadAllSemiPersistentAssets ()
		{
			foreach (AssetRef assetRef in SemiPersistentObjects) {
				if (assetRef.IsDiskAsset)
					Resources.UnloadAsset (assetRef.Asset);
				else
					Destroy (assetRef.Asset);
			}
		}

		public void UnloadOnSceneTransition (Object nonPersistentObject, bool isDiskAsset = false)
		{
			if (nonPersistentObject)
				NonPersistentObjects.Add (new AssetRef (nonPersistentObject, null, isDiskAsset));
		}

		public void CacheAsset (Object persistentObject, string id, bool isDiskAsset = false)
		{
			AssetRef assetRef = PersistentObjects.Find (x => x.Url == id);

			if (assetRef == null) {
				assetRef = new AssetRef (persistentObject, id, isDiskAsset);
				PersistentObjects.Add (assetRef);
			} else {
				Object previousAsset = assetRef.Asset;

				if (previousAsset != persistentObject) {
					if (assetRef.IsDiskAsset)
						Resources.UnloadAsset (previousAsset);
				} else {
					Destroy (previousAsset);
				}

				assetRef.Asset = persistentObject;
				assetRef.IsDiskAsset = isDiskAsset;
			}
		}

		public System.TimeSpan GetAssetAge (string subPath)
		{
			DateTime creationTimeUTC = File.GetLastWriteTimeUtc (Path.Combine (Application.persistentDataPath, subPath));
			TimeSpan age = DateTime.UtcNow - creationTimeUTC;
			return age;
		}


		public override bool Init ()
		{
			/*
			#if UNITY_EDITOR
			assetPath = Application.dataPath + "/StreamingAssets/" + "/";
			#elif UNITY_ANDROID
			assetPath = "jar:file://" + Application.dataPath + "!/assets/";
			#elif UNITY_IOS
			assetPath = Application.dataPath + "/Raw/";
			#endif*/

			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				assetPath = Application.dataPath + "/Raw/";
			} else if (Application.platform == RuntimePlatform.Android) {
				string origPath = Application.streamingAssetsPath;
				
				WWW reader = new WWW (origPath);
				while (!reader.isDone) {
				}

				string realPath = Application.persistentDataPath;
				assetPath = realPath;

			} else {
				assetPath = Application.dataPath + "/StreamingAssets/";
			}

			if (assetPath.IndexOf ("file://") < 0)
				assetPath = "file://" + assetPath;

			Debug.Log ("Assetbundle Service Ready!");

			return true;
		}

		IEnumerator LogBundleDetails ()
		{
			LogBundlesNow ();
			yield return new WaitForSeconds (0.5f);
			StartCoroutine (LogBundleDetails ());
		}

		public void LogBundlesNow ()
		{
			string log = "ASSETBUNDLE LOG: " + DateTime.UtcNow.TimeOfDay + "\n";
			foreach (KeyValuePair<string, AssetBundleRef> data in dictAssetBundleRefs) {
				log += data.Key + ", Bundle?, " + (bool)(data.Value.AssetBundleInst);
				log += "\n";
			}

			Debug.Log (log);
		}

		void OnDestroy ()
		{
			UnloadAllNonPersistent ();
			UnloadAllPersistentAssets ();
			LogBundlesNow ();
		}

		string GetFullPathInDownloads (string subDirectory)
		{
			return Uri.EscapeUriString (Path.Combine ("file://" + Application.persistentDataPath + "/", subDirectory.ToLower () + ".unity3d"));
		}

		string GetFullPath (string subDirectory)
		{
			return Uri.EscapeUriString (assetPath + subDirectory.ToLower () + ".unity3d");
		}

		public Coroutine StartLoad<T> (string url, ObjectPtr<T> prefab, Action<bool, ObjectPtr<T>> callback = null, Persistence persistence = Persistence.None, AssetType type = AssetType.Undeclared) where T: Object
		{
			return StartCoroutine (Load<T> ("", url, prefab, callback, persistence, type));
		}

		public Coroutine StartLoad<T> (string name, string url, ObjectPtr<T> prefab, Action<bool, ObjectPtr<T>> callback = null, Persistence persistence = Persistence.None, AssetType type = AssetType.Undeclared) where T: Object
		{
			return StartCoroutine (Load<T> (name, url, prefab, callback, persistence, type));
		}

		private IEnumerator Load<T> (string name, string url, ObjectPtr<T> objPtr, Action<bool, ObjectPtr<T>> callback = null, Persistence persistence = Persistence.None, AssetType type = AssetType.Undeclared) where T : Object
		{
			while (isLoading) {
				yield return null;
			}

			isLoading = true;

			string fullPath = GetFullPathInDownloads (url);
			int version = Version;

			if (LogAssetBundlePath)
				Debug.Log ("LOADING: " + url);
			
			AssetBundle bundle = GetAssetBundle (url, version);

			#if UNITY_EDITOR
			yield return new WaitForSeconds (0.05f);
			#endif

			if (!bundle) {
				yield return StartCoroutine (DownloadAssetBundle (fullPath, url, version, type));
				bundle = GetAssetBundle (url, version);

				if (!bundle) {
					fullPath = GetFullPath (url);
					yield return StartCoroutine (DownloadAssetBundle (fullPath, url, version, type, true));
					bundle = GetAssetBundle (url, version);

				}
			}
				
			isLoading = false;

			Object mainAsset = null;

			//New asset bundle request
			if (bundle == null) { 

				var bundleLoadRequest = AssetBundle.LoadFromFileAsync (Application.streamingAssetsPath + "/" + url);
		
				yield return bundleLoadRequest;

				if (bundleLoadRequest.assetBundle == null) {
					if (callback != null)
						callback (false, null);
					
					yield break;
				} else {

					if (!string.IsNullOrEmpty (name)) {

						var assetLoadRequest = bundleLoadRequest.assetBundle.LoadAssetAsync<T> (name);

						yield return assetLoadRequest;

						if (assetLoadRequest.asset == null) {
							if (callback != null)
								callback (false, null);

						} else {
							mainAsset = assetLoadRequest.asset;
						}
					} else {
						mainAsset = bundleLoadRequest.assetBundle;
					}

					bundleLoadRequest.assetBundle.Unload (false);
				}
			} else {
				mainAsset = bundle.mainAsset;
			}

			FinalizeLoadedAsset (mainAsset, objPtr, callback);

			string keyName = GetKeyName (url, version);

			if (persistence == Persistence.Full)
				PersistentAssetBundles [keyName] = bundle;
			else if (persistence == Persistence.Partial)
				SemiPersistentAssetBundles [keyName] = bundle;

			isLoading = false;
		}

		string GetKeyName (string subUrl, int version)
		{
			return subUrl + version;
		}

		void FinalizeLoadedAsset<T> (Object mainAsset, ObjectPtr<T> prefab, Action<bool, ObjectPtr<T>> callback) where T : Object
		{
			if (mainAsset is GameObject) {
				prefab.value = mainAsset.Instantiate<T> ();
				Object obj = (Object)prefab.value;
				GameObject go = (GameObject)obj;
				go.SetActive (false);
			} else {
				prefab.value = mainAsset as T;
			}

			if (callback != null)
				callback (prefab.value != null, prefab);
		}

		AssetBundle GetAssetBundle (string url, int version)
		{
			string keyName = GetKeyName (url, version);
			AssetBundleRef abRef;

			if (dictAssetBundleRefs.TryGetValue (keyName, out abRef)) {
				if (abRef.AssetBundleInst) {
					if (abRef.AssetBundleInst.mainAsset == null) {
						dictAssetBundleRefs.Remove (keyName);
						abRef.AssetBundleInst.Unload (true);
						return null;
					}
				}
				return abRef.AssetBundleInst;
			}
			return null;
		}

		IEnumerator DownloadAssetBundle (string url, string subUrl, int version, AssetType type, bool logError = false)
		{
			string keyName = GetKeyName (subUrl, version);
			if (dictAssetBundleRefs.ContainsKey (keyName)) {
				Debug.Log ("ASSETBUNDLE: Asset key found in reference, skipping download: " + keyName);
				yield break;
			}

			while (!Caching.ready)
				yield return null;

			using (WWW www = new WWW (url)) {
				yield return www;
				if (www.error != null) {
					yield break;
				}
				AssetBundleRef abRef = new AssetBundleRef (subUrl, version, type);
				abRef.AssetBundleInst = www.assetBundle;
				dictAssetBundleRefs.Add (keyName, abRef);
			}
		}

		IEnumerator CoLoadTexture (string path, Action<bool, Texture2D> callback, bool unloadOnSceneChange, bool isLocalFile = false)
		{
			using (WWW www = new WWW (isLocalFile ? ("file:///" + path) : path)) {
				yield return www;
				if (www.bytes.Length == 0) {
					callback (false, null);
					yield break;
				}
				Texture2D image = www.texture;

				if (unloadOnSceneChange)
					UnloadOnSceneTransition (image);

				if (callback != null)
					callback (image != null, image);
			}
		}

		IEnumerator CoLoadLocalTexture (string path, Action<bool, Texture2D> callback, bool unloadOnSceneChange)
		{
			if (!File.Exists (path)) {
				callback (false, null);
				yield break;
			}

			FileStream fs = new FileStream (path, FileMode.Open);
			byte[] data = new byte[fs.Length];
			int bytesToReadPerFrame = 5000;
			int x = 0;
			int bytesRead = 0;
			Debug.Log ("FS length: " + fs.Length);

			while (fs.CanRead) {
				if (data.Length - bytesRead < bytesToReadPerFrame) {
					bytesToReadPerFrame = data.Length - bytesRead;
				}

				x = fs.Read (data, bytesRead, bytesToReadPerFrame);
				bytesRead += x;

				if (bytesRead == data.Length)
					break;
				yield return null;
			}

			Texture2D image = new Texture2D (0, 0);
			image.LoadImage (data);
			if (unloadOnSceneChange)
				UnloadOnSceneTransition (image);
			if (callback != null)
				callback (image != null, image);
		}

		protected void UnloadAssetBundle (string address, int version, bool allObjects)
		{
			string fullAddress = GetKeyName (address, Version);
			UnloadAssetBundleWithActualAddress (fullAddress, allObjects);
		}

		protected void UnloadAssetBundleWithActualAddress (string url, bool allObjects)
		{
			AssetBundleRef abRef;

			if (dictAssetBundleRefs.TryGetValue (url, out abRef)) {
				if (abRef.AssetBundleInst != null) {
					abRef.AssetBundleInst.Unload (allObjects);
					abRef.AssetBundleInst = null;
				}

				dictAssetBundleRefs.Remove (url);
			}

			PersistentAssetBundles.Remove (url);
		}

		void UnloadOtherAssets ()
		{
			foreach (AssetRef assetRef in NonPersistentObjects) {
				if (assetRef.IsDiskAsset)
					Resources.UnloadAsset (assetRef.Asset);
				else
					Destroy (assetRef.Asset);
			}

			NonPersistentObjects.Clear ();
		}

		void UnloadAllNonPersistentBundles ()
		{
			Dictionary<string,AssetBundleRef> dict = new Dictionary<string,AssetBundleRef> (dictAssetBundleRefs);
			bool remove = true;

			foreach (var kv in dict) {
				remove = !PersistentAssetBundles.ContainsKey (kv.Key) && !SemiPersistentAssetBundles.ContainsKey (kv.Key);

				if (remove) {
					AssetBundle ab = kv.Value.AssetBundleInst;
					ab.Unload (true);
					kv.Value.AssetBundleInst = null;
					dictAssetBundleRefs.Remove (kv.Key);
				}
			}
		}

		private void Update ()
		{
			if (IsBusy)
				return;
			if (Tasks.Count == 0)
				return;

			StartCoroutine (ProcessNextTask ());
		}

		private IEnumerator ProcessNextTask ()
		{
			object taskObj = Tasks.GetKey (0);

			if (taskObj == null) {
				Tasks.RemoveAt (0);
				yield break;
			}

			TaskType taskType = (TaskType)Tasks.GetValue (0);
			Tasks.RemoveAt (0);
			IsBusy = true;

			switch (taskType) {
			case TaskType.Assetbundle:
				if (taskObj is Task<GameObject>)
					yield return StartCoroutine (ProcessAssetBundleTask<GameObject> (taskObj as Task<GameObject>));
				else if (taskObj is Task<Mesh>)
					yield return StartCoroutine (ProcessAssetBundleTask<Mesh> (taskObj as Task<Mesh>));
				else if (taskObj is Task<Texture>)
					yield return StartCoroutine (ProcessAssetBundleTask<Texture> (taskObj as Task<Texture>));
				else if (taskObj is Task<Texture2D>)
					yield return StartCoroutine (ProcessAssetBundleTask<Texture2D> (taskObj as Task<Texture2D>));
				else if (taskObj is Task<Material>)
					yield return StartCoroutine (ProcessAssetBundleTask<Material> (taskObj as Task<Material>));
				else if (taskObj is Task<AnimationClip>)
					yield return StartCoroutine (ProcessAssetBundleTask<AnimationClip> (taskObj as Task<AnimationClip>));
				else if (taskObj is Task<AudioClip>)
					yield return StartCoroutine (ProcessAssetBundleTask<AudioClip> (taskObj as Task<AudioClip>));
				else if (taskObj is Task<TextAsset>)
					yield return StartCoroutine (ProcessAssetBundleTask<TextAsset> (taskObj as Task<TextAsset>));
				break;
			case TaskType.Texture:
				{
					Task<Texture2D> task = taskObj as Task<Texture2D>;
					if (task != null) {
						yield return StartCoroutine (CoLoadTexture (task.Url, task.TextureCallback, task.Persistence == Persistence.None, task.IsLocalFile));
					}
				}
				break;
			case TaskType.CacheableTexture:
				{
					Task<Texture2D> task = taskObj as Task<Texture2D>;
					if (task != null) {
						yield return StartCoroutine (LoadTextureFromCacheOrDownload (task.Url, task.TextureCallback, task.Persistence == Persistence.None, task.MaxTextureWidth, task.MaxTextureHeight));
					}
				}
				break;
			}

			IsBusy = false;
		}

		private IEnumerator ProcessAssetBundleTask<T> (Task<T> task) where T : Object
		{
			yield return StartLoad (task.Name, task.Url, task.Prefab, task.GenericCallback, task.Persistence, task.AssetType);
		}
	}
}