using System;
using System.Threading;
namespace SF.Expand.Secure.GenerateTokens
{
	public static class RandomGen
	{
		private static Random _global = new Random();
		[ThreadStatic]
		private static Random _local;
		public static int Next()
		{
			Random random = RandomGen._local;
			if (random == null)
			{
				Random global;
				Monitor.Enter(global = RandomGen._global);
				int seed;
				try
				{
					seed = RandomGen._global.Next();
				}
				finally
				{
					Monitor.Exit(global);
				}
				random = (RandomGen._local = new Random(seed));
			}
			return random.Next();
		}
	}
}
