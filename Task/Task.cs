using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Common.ObjectPointer;
using Common.AssetBundleService;
using Common.AssetBundleService.Model;
using Common.Task.Model;

namespace Common.Task
{
	public class Task<T> where T : UnityEngine.Object
	{
		public TaskType TaskType;

		public string Url;
		public Persistence Persistence;
		public AssetType AssetType;

		//Asset Bundle specific
		public string Name;
		public Type TType;
		public ObjectPtr<T> Prefab;
		public Action<bool, ObjectPtr<T>> GenericCallback;

		//Texture2D specific
		public Action<bool, Texture2D> TextureCallback;
		public bool IsLocalFile;
		public int MaxTextureWidth;
		public int MaxTextureHeight;


	}

	public class NetworkTask<T> where T : class
	{
		public string Url;
		public WWWForm Form;
		public NetworkTaskType TaskType;
		public NetworkPtr<T> Prefab;
		public Action<bool, NetworkPtr<T>> GenericCallback;
	}

	public class DownloadTask<T> where T : class
	{
		public string Url;
		public string Name;
		public string JSONString;
		public DownloadTaskType Type;
		public Action<bool, DownloadPtr<T>> callBack;
	}
}

