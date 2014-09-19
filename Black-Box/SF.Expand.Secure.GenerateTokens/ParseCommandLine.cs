

using System;


namespace SF.Expand.Secure.GenerateTokens
{
    static class ParseCommandLine
    {
        private static string NL = Environment.NewLine;
        private static string USAGE =
                "Usage: " + Environment.GetCommandLineArgs()[0] + NL +
                "          [-f <output path>]" + NL +
                "          [-u <seed type>] " + NL +
                "          [-t <total threads used>] " + NL +
                "          [-s <number of seeds to generate>] " + NL +
                "          [-o <output thread process>]" + NL +
                "          [-m <generate matrix file>]";


        public static bool Execute(string[] args, out string seedType, out string fName, out int tThreads, out long tTotalSeeds, out bool showsResults,out bool generateMatrix)
        {
            int i = 0;
            seedType = null;
            fName = "'NULL'";
            showsResults = false;
            generateMatrix = false;
            tThreads = -1;
            tTotalSeeds = -1;

            try
            {
                for (i = 0; i < args.Length; i++)
                {
                    if (args[i].Equals("-f"))
                    {
                        try
                        {
                            fName = System.IO.Path.GetDirectoryName(args[++i]);
                            if (!fName.EndsWith(@"\")) fName = fName + @"\";

                            System.IO.DirectoryInfo _dInfo = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(fName));
                            if (!_dInfo.Exists)
                            {
                                System.Console.WriteLine("Path/folder destination not found! Create?");
                                ConsoleKeyInfo _conInfo = System.Console.ReadKey();
                                if (_conInfo.Key.ToString().ToLower() != "y")
                                {
                                    Console.WriteLine(USAGE);
                                    return false; 
                                }
                                _dInfo.Create();
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Invalid argument /" + args[i] + "  '" + fName + "'");
                            Console.WriteLine(USAGE);
                            return false; 
                        }
                        continue;
                    }
                    if (args[i].Equals("-t"))
                    {
                        tThreads = Convert.ToInt32(args[++i]);
                        continue;
                    }
                    if (args[i].Equals("-s"))
                    {
                        tTotalSeeds = Convert.ToInt32(args[++i]);
                        continue;
                    }
                    if (args[i].Equals("-u"))
                    {
                        seedType = args[++i];
                        continue;
                    }
                    if (args[i].Equals("-o"))
                    {
                        showsResults = Boolean.Parse(args[++i]);
                        continue;
                    }
                    if (args[i].Equals("-m"))
                    {
                        generateMatrix = Boolean.Parse(args[++i]);
                        continue;
                    }
                    if (args[i].Equals("-help") || args[i].Equals("-?"))
                    {
                        Console.WriteLine(USAGE);
                        return false;
                    }

                    Console.WriteLine("Unrecognized parameter '" + args[i] + "'.");
                    Console.WriteLine(USAGE);
                    return false;
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Missing argument for " + args[i - 1] + ".");
                Console.WriteLine(USAGE);
                return false;
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid argument '" + args[i] + "' for " + args[i - 1] + ".");
                Console.WriteLine(USAGE);
                return false;
            }

            //if (args.Length / 2 != 5)
            //{
            //    Console.WriteLine(USAGE);
            //    return false;
            //}

            Console.WriteLine("SF.Expand.Secure.GenerateTokens" + NL + NL +
                      "Settings: " + NL +
                      "     <output path>........" + fName + NL +
                      "     <seed type>.............." + seedType + NL +
                      "     <total threads>.........." + tThreads.ToString() + NL +
                      "     <seeds to generate>......" + tTotalSeeds.ToString() + NL +
                      "     <show processing>........" + showsResults.ToString() + NL +
                      "     <generate matrix file>..." + generateMatrix.ToString());

            if (fName.Length < 4 || tThreads == 0 || tTotalSeeds == 0)
            {
                return false;
            }
            return true;
        }
    }
}