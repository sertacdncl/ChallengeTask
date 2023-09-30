namespace Shared.Finger
{
	public static class FingerManager
	{
		public static bool IsFingerDown => FingerCount > 0;
		public static bool CanUseFinger { get; set; } = true;
		public static int FingerCount { get; set; }
		public static int MaxFingerCount { get; set; } = 1;
		
		public static bool CanTouch => FingerCount < MaxFingerCount && CanUseFinger;
	}
}