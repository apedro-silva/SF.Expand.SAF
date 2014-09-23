using System;
using System.IO;
namespace SF.Expand.Secure.GenerateTokens
{
	internal static class ParseCommandLine
	{
		private static string NL = Environment.NewLine;
		private static string USAGE = string.Concat(new string[]
		{
			"Usage: ",
			Environment.GetCommandLineArgs()[0],
			ParseCommandLine.NL,
			"          [-f <output path>]",
			ParseCommandLine.NL,
			"          [-u <seed type>] ",
			ParseCommandLine.NL,
			"          [-t <total threads used>] ",
			ParseCommandLine.NL,
			"          [-s <number of seeds to generate>] ",
			ParseCommandLine.NL,
			"          [-o <output thread process>]",
			ParseCommandLine.NL,
			"          [-m <generate matrix file>]"
		});
		public static bool Execute(string[] args, out string seedType, out string fName, out int tThreads, out long tTotalSeeds, out bool showsResults, out bool generateMatrix)
		{
			int i = 0;
			seedType = null;
			fName = "'NULL'";
			showsResults = false;
			generateMatrix = false;
			tThreads = -1;
			tTotalSeeds = -1L;
			try
			{
				i = 0;
				while (i < args.Length)
				{
					bool result;
					if (args[i].Equals("-f"))
					{
						try
						{
							fName = Path.GetDirectoryName(args[++i]);
							if (!fName.EndsWith("\\"))
							{
								fName += "\\";
							}
							DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(fName));
							if (!directoryInfo.Exists)
							{
								Console.WriteLine("Path/folder destination not found! Create?");
								if (Console.ReadKey().Key.ToString().ToLower() != "y")
								{
									Console.WriteLine(ParseCommandLine.USAGE);
									result = false;
									return result;
								}
								directoryInfo.Create();
							}
							goto IL_20A;
						}
						catch
						{
							Console.WriteLine(string.Concat(new string[]
							{
								"Invalid argument /",
								args[i],
								"  '",
								fName,
								"'"
							}));
							Console.WriteLine(ParseCommandLine.USAGE);
							result = false;
							return result;
						}
						goto IL_110;
					}
					goto IL_110;
					IL_20A:
					i++;
					continue;
					IL_110:
					if (args[i].Equals("-t"))
					{
						tThreads = Convert.ToInt32(args[++i]);
						goto IL_20A;
					}
					if (args[i].Equals("-s"))
					{
						tTotalSeeds = (long)Convert.ToInt32(args[++i]);
						goto IL_20A;
					}
					if (args[i].Equals("-u"))
					{
						seedType = args[++i];
						goto IL_20A;
					}
					if (args[i].Equals("-o"))
					{
						showsResults = bool.Parse(args[++i]);
						goto IL_20A;
					}
					if (args[i].Equals("-m"))
					{
						generateMatrix = bool.Parse(args[++i]);
						goto IL_20A;
					}
					if (args[i].Equals("-help") || args[i].Equals("-?"))
					{
						Console.WriteLine(ParseCommandLine.USAGE);
						result = false;
						return result;
					}
					Console.WriteLine("Unrecognized parameter '" + args[i] + "'.");
					Console.WriteLine(ParseCommandLine.USAGE);
					result = false;
					return result;
				}
			}
			catch (IndexOutOfRangeException)
			{
				Console.WriteLine("Missing argument for " + args[i - 1] + ".");
				Console.WriteLine(ParseCommandLine.USAGE);
				bool result = false;
				return result;
			}
			catch (FormatException)
			{
				Console.WriteLine(string.Concat(new string[]
				{
					"Invalid argument '",
					args[i],
					"' for ",
					args[i - 1],
					"."
				}));
				Console.WriteLine(ParseCommandLine.USAGE);
				bool result = false;
				return result;
			}
			Console.WriteLine(string.Concat(new string[]
			{
				"SF.Expand.Secure.GenerateTokens",
				ParseCommandLine.NL,
				ParseCommandLine.NL,
				"Settings: ",
				ParseCommandLine.NL,
				"     <output path>........",
				fName,
				ParseCommandLine.NL,
				"     <seed type>..............",
				seedType,
				ParseCommandLine.NL,
				"     <total threads>..........",
				tThreads.ToString(),
				ParseCommandLine.NL,
				"     <seeds to generate>......",
				tTotalSeeds.ToString(),
				ParseCommandLine.NL,
				"     <show processing>........",
				showsResults.ToString(),
				ParseCommandLine.NL,
				"     <generate matrix file>...",
				generateMatrix.ToString()
			}));
			return fName.Length >= 4 && tThreads != 0 && tTotalSeeds != 0L;
		}
	}
}
