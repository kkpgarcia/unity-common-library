/*
Author: Kyle Garcia
Date Created: 2016
Last Updated: 2020
*/

using UnityEngine;
using System.Collections.Generic;
using Common.Singleton;

namespace Common.ObjectPool
{
	public class ObjectPool : PermanentMonoSingleton<ObjectPool>
	{

		public GameObject[] objectPrefabs;
		public List<GameObject>[] pooledObjects;
		public int[] amountToBuffer;
		public int defaultBufferAmount = 20;
		public GameObject gameContext;

		protected GameObject objectContainer;

		private void Start ()
		{
			objectContainer = this.gameObject;

			pooledObjects = new List<GameObject>[objectPrefabs.Length];

			int i = 0;
			foreach (GameObject objectPrefab in objectPrefabs) {
				pooledObjects [i] = new List<GameObject> ();
				int bufferAmount;

				if (i < amountToBuffer.Length)
					bufferAmount = amountToBuffer [i];
				else
					bufferAmount = defaultBufferAmount;

				for (int j = 0; j < bufferAmount; j++) {
					GameObject newObj = (GameObject)Instantiate (objectPrefab);
					newObj.name = objectPrefab.name;
					PoolObject (newObj);
				}

				i++;
			}
		}

		public GameObject GetObjectForType (string objectType, bool onlyPooled)
		{
			for (int i = 0; i < objectPrefabs.Length; i++) {
				GameObject prefab = objectPrefabs [i];
				if (prefab.name == objectType) {
					if (pooledObjects [i].Count > 0) {
						GameObject pooledObject = pooledObjects [i] [0];
						pooledObjects [i].RemoveAt (0);
						pooledObject.transform.parent = gameContext.transform;
						pooledObject.SetActive (true);

						return pooledObject;
					} else if (!onlyPooled) {
						
						return Instantiate (objectPrefabs [i]) as GameObject;
					}
					break;
				}
			}
				
			return null;
		}

		public void PoolObject (GameObject obj)
		{
			for (int i = 0; i < objectPrefabs.Length; i++) {
				if (objectPrefabs [i].name == obj.name) {
					obj.SetActive (false);
					obj.transform.transform.parent = objectContainer.transform;
					pooledObjects [i].Add (obj);
					return;
				}
			}
		}
	}

}