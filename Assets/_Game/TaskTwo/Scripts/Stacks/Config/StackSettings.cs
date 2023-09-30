using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TaskTwo.Stacks.Config
{
	[Serializable]
	public class StackSettings
	{
		public AssetReference stackAssetRef;
		public List<Material> stackMats;
		public float moveTime;
	}
}