using UnityEngine;

namespace Shared.Extensions
{
	public static class ColorExtension
	{
		public static Color With(this Color origin, float? r = null, float? g = null, float? b = null, float? a = null)
		{
			return new Color(r ?? origin.r, g ?? origin.g, b ?? origin.b, a ?? origin.a);
		}
	}
}