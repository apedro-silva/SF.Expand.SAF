using System;
namespace SF.Expand.Secure.GenerateTokens
{
	internal class Program
	{
		[STAThread]
		private static void Main(string[] args)
		{
			string fname = null;
			bool showProcessing = false;
			bool generateMatrix = false;
			int tThreads = -1;
			long tSeeds = -1L;
			string seedType = null;
			try
			{
				Console.Title = string.Concat(new string[]
				{
					"SF.Expand.Secure.GenerateTokens [ init:",
					DateTime.Now.ToShortDateString(),
					" ",
					DateTime.Now.ToLongTimeString(),
					"]"
				});
				if (ParseCommandLine.Execute(args, out seedType, out fname, out tThreads, out tSeeds, out showProcessing, out generateMatrix))
				{
					GenerateSeeds.Generate(seedType, fname, tThreads, tSeeds, showProcessing, generateMatrix);
				}
				Console.ReadKey();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
	}
}
