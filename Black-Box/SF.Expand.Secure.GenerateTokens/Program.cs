

using System;


namespace SF.Expand.Secure.GenerateTokens
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string sName = null;
            bool showsResults = false;
            bool generateMatrix = false;
            int iThreads = -1;
            long lTotalSeeds = -1;
            string sSeedType = null;

            try
            {
                Console.Title = "SF.Expand.Secure.GenerateTokens [ init:" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + "]";
                if (ParseCommandLine.Execute(args, out sSeedType, out sName, out iThreads, out lTotalSeeds, out showsResults, out generateMatrix))
                {
                    GenerateSeeds.Generate(sSeedType, sName, iThreads, lTotalSeeds, showsResults, generateMatrix);
                }
                Console.ReadKey();
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
