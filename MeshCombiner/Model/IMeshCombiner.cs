using UnityEngine;
using System.Collections;

namespace Common.MeshCombiner
{
	public interface IMeshCombiner
	{
		void CombineObjects (MeshFilter[] meshFilters, GameObject obj, bool mergeSubMeshes = true, bool useMatrices = true);
	}
}