using System;
using JetBrains.Annotations;
using TaskOne.Grid.Components;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TaskOne.Grid
{
	[UsedImplicitly]
	public class GridManager : MonoBehaviour
	{
		public Transform gridParent;
		public Transform cellMarkerPoolParent;
		public CellController[,] CellControllers;


		[Serializable]
		public class Settings
		{
			public AssetReference cellPrefab;
			public AssetReference xObjectPrefab;
			public int width;
			public int height;
			public float distance;	
		}
	}
}