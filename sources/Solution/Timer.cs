namespace Saxion.CMGT.Algorithms.sources.Solution;

public static class WaitTimer
{
	public static void WaitFor(int frames)
	{
		int framesWaited = 0;
		while (framesWaited != frames) framesWaited++;
	}
}