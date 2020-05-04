using UnityEngine;
using System.Collections;
using Common.ObjectPointer;
using Common.AssetBundleService.Model;

namespace Common.AssetBundleService
{
	
	public interface IAssetManagerService
	{
		// new asset loading prior to Unity 5
		void LoadAssetBundle<T> (string name, string url, ObjectPtr<T> prefab, System.Action<bool, ObjectPtr<T>> callback, AssetType type = AssetType.Undeclared) where T : Object;

		// loads an asset bundle that will be removed from the memory during scene transitions
		void LoadAssetBundle<T> (string url, ObjectPtr<T> prefab, System.Action<bool, ObjectPtr<T>> callback, AssetType type = AssetType.Undeclared) where T : Object;

		// loads an asset bundle that will be retained in memory during scene transitions
		void LoadPersistentAssetBundle<T> (string url, ObjectPtr<T> prefab, System.Action<bool, ObjectPtr<T>> callback, AssetType type = AssetType.Undeclared) where T : Object;

		// loads an asset bundle that will be retained in memory during scene transitions, and then be deleted when next scene is fully loaded
		void LoadSemiPersistentAssetBundle<T> (string url, ObjectPtr<T> prefab, System.Action<bool, ObjectPtr<T>> callback, AssetType type = AssetType.Undeclared) where T : Object;

		// loads a texture located at the full path specified
		void LoadTextureFromFullPath (string fullPath, System.Action<bool, Texture2D> callback, bool unloadOnSceneChange, bool isLocalFile);

		// loads a texture in the app's persistent path at the provided subpath
		void LoadTextureFromPersistentPath (string subPath, System.Action<bool, Texture2D> callback, bool unloadOnSceneChange);

		void LoadTextureFromUrl (string url, System.Action<bool, Texture2D> callback, bool unloadOnSceneChange, bool cache, int maxWidth, int maxHeight);

		// unloads an asset bundle by url, persistent bundles may be unloaded by this method when they are no longer required
		void UnloadAssetBundle (string url);

		// unloads an asset by url
		void UnloadAsset (string url);

		// unloads all non persistent assets and asset bundles in memory
		void UnloadAllNonPersistent ();

		void UnloadAllNonPersistentBundlesOfType (AssetType type);

		// unloads all persistent asset bundles in memory
		void UnloadAllPersistentAssetBundles ();

		void UnloadAllPersistentAssets ();

		void UnloadAllSemiPersistentAssetBundles ();

		void UnloadAllSemiPersistentAssets ();

		// marks an object for deletion on scene transition, use this for objects loaded via means other than asset bundles
		// e.g. procedural assets
		void UnloadOnSceneTransition (Object nonPersistentObject, bool isDiskAsset = false);

		// keep the asset in memory during scene transitions, note that the asset can still be destroyed elsewhere
		void CacheAsset (Object persistentObject, string id, bool isDiskAsset = false);

		// returns TimeSpan age of asset in app's persistent path at provided subpath
		System.TimeSpan GetAssetAge (string subPath);
	}
}