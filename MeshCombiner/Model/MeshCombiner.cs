using UnityEngine;
using System.Collections;

using Hopico.Game.Model;

namespace Common.MeshCombiner
{
	public class MeshCombiner : IMeshCombiner
	{
		[Inject]
		public IMaterialManager MaterialManager { get; set; }

		public void CombineObjects (MeshFilter[] meshFilters, GameObject obj, bool mergeSubMeshes = true, bool useMatrices = true)
		{
			int i = 0;
			CombineInstance[] combine = new CombineInstance[meshFilters.Length];
			Vector3 referencePosition = obj.transform.position;

			while (i < meshFilters.Length) {
				combine [i].mesh = meshFilters [i].sharedMesh;
				combine [i].transform = meshFilters [i].transform.localToWorldMatrix;
				meshFilters [i].transform.parent.gameObject.SetActive (false);
				i++;
			}

			MeshFilter objMf;
			MeshRenderer objMr;

			if (obj.GetComponent<MeshFilter> () == null) {
				objMf = obj.AddComponent<MeshFilter> ();
			} else {
				objMf = obj.GetComponent<MeshFilter> ();
			}

			if (obj.GetComponent<MeshRenderer> () == null) {
				objMr = obj.AddComponent<MeshRenderer> ();
			} else {
				objMr = obj.GetComponent<MeshRenderer> ();
			}

			objMf.mesh.Clear ();
			objMf.mesh = new Mesh ();
			objMf.mesh.CombineMeshes (combine, mergeSubMeshes, useMatrices);
			obj.transform.position = referencePosition;

			//TODO Remove
			objMr.sharedMaterial = MaterialManager.GetEnvironmentMaterial ();

			obj.SetActive (true);

		}
	}
}