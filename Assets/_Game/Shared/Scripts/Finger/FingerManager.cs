namespace Shared.Finger
{
	public static class FingerManager
	{
		public static bool IsFingerDown => FingerCount > 0;
		public static int FingerCount { get; set; }
		public static int MaxFingerCount { get; set; } = 1;
		
		public static bool CanTouch => FingerCount < MaxFingerCount;
	}
}