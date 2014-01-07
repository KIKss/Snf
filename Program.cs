using System;
using System.Collections.Generic;
using System.IO;

namespace snf
{
	class Program
	{

		public static void Main(string[] args)
		{
			ConsoleKey vvd;
			bool autostart = false;
			bool onlylog = false;
			bool log = false;
			Console.WriteLine("	===================================================================");
			Console.WriteLine("			Search new and modified files in directory");
			Console.WriteLine("	___________________________________________________________________");
			Console.WriteLine("	command line arguments:");
			Console.WriteLine("	-p path   :Path for check");
			Console.WriteLine("	-cp path  :Folder for modified files (program directory by default)");
			Console.WriteLine("	-start    :Start checking after run");
			Console.WriteLine("	-log      :Create log file with paths");
			Console.WriteLine("	-onlylog  :Create only log file and exit");
			Console.WriteLine("	===================================================================\n");
			
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == "-p")
				{
					if (!String.IsNullOrEmpty(args[i + 1]))
					{
						Tools.CHDir = args[i + 1];
					}
				}
				if (args[i] == "-cp")
				{
					if (!String.IsNullOrEmpty(args[i + 1]))
					{
						Tools.SDir = args[i + 1];
					}
				}
				
				if (args[i] == "-start")
				{
					autostart = true;
				}
				
				if (args[i] == "-onlylog")
				{
					onlylog = true;
				}
				
				if (args[i] == "-log")
				{
					log = true;
				}
			}
			
			if (!Directory.Exists(Tools.CHDir)) {
				Console.WriteLine(" Error. No folder for checking");
				Console.WriteLine(" Press any key to exit..");
				Console.ReadKey(true);
				return;
			}
			
			Tools.SDir = Tools.SDir+Tools.CHDir.Substring(Tools.CHDir.LastIndexOf(Path.DirectorySeparatorChar));
			Console.WriteLine(" Checking folder: "+Tools.CHDir);
			Console.WriteLine(" Save to: "+Tools.SDir+"\n");
			Tools.DBFile = Environment.CurrentDirectory+
				Tools.CHDir.Substring(Tools.CHDir.LastIndexOf(Path.DirectorySeparatorChar))+
				".data";
			
			List<Tools.FDataClass> Base = new List<Tools.FDataClass>();
			List<Tools.FDataClass> NBase = new List<Tools.FDataClass>();
			List<string> ToWork = new List<string>();
			
			if(!(File.Exists(Tools.DBFile)) )
			{
				Console.WriteLine(" First run. Creating folder image..");
				Base = Tools.GetDirList();
				Tools.SaveDB(Base);
				Console.WriteLine("  Complete. Files found: "+Base.Count);
			}else{
				if(autostart){vvd = ConsoleKey.Enter;goto Start;}
				Console.WriteLine(" Press: ");
				Console.WriteLine(" Enter - check folder for changes");
				Console.WriteLine(" N - Create folder image");
			TryAgain:
				vvd = Console.ReadKey(true).Key;
			Start:
				switch (vvd) {
					case ConsoleKey.Enter:
						Base = Tools.LoadDB(Tools.DBFile);
						Console.WriteLine(" Checking folder..");
						NBase = Tools.GetDirList();
						Tools.FDataClass f;
						foreach (Tools.FDataClass d in NBase)
						{
							f = Base.Find(p => p.Path == d.Path);
							if(f == null ||f.CHDate != d.CHDate || f.CRDate != d.CRDate)
							{
								ToWork.Add(d.Path);
							}
						}
						
						if (ToWork.Count != 0) {
							Console.WriteLine(" Files found: "+ ToWork.Count);
							
							string CRPath;
							
							if(log || onlylog)
							{
								File.WriteAllLines(Environment.CurrentDirectory+
								                   Tools.CHDir.Substring(Tools.CHDir.LastIndexOf(Path.DirectorySeparatorChar))+
								                   ".txt",ToWork);
								Console.WriteLine(" Log created.");
								if(onlylog){Tools.SaveDB(NBase);break;}
							}
							
							Console.WriteLine(" Copying new files..");
							if (Directory.Exists(Tools.SDir))
							{
								DirectoryInfo directory = new DirectoryInfo(Tools.SDir);
								foreach (System.IO.FileInfo file in directory.GetFiles()) file.Delete();
								foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
							}
							
							foreach (var w in ToWork) {
								
								CRPath = Tools.SDir+Path.DirectorySeparatorChar+Path.GetDirectoryName(w.Replace(Tools.CHDir,""));
								
								if (!Directory.Exists(CRPath))
								{
									Directory.CreateDirectory(CRPath);
								}
								
								File.Copy(w,CRPath+Path.DirectorySeparatorChar+w.Substring(w.LastIndexOf(Path.DirectorySeparatorChar)));
							}
							
							Tools.SaveDB(NBase);
							Console.WriteLine(" Complete.");
						}else{
							Console.WriteLine(" Files not found.");
						}
						
						break;
					case ConsoleKey.N://--------------------------------------------------------------

						Console.WriteLine(" Creating folder image..");
						Base = Tools.GetDirList();
						Tools.SaveDB(Base);
						Console.WriteLine(" Complete. Files found: "+Base.Count);
						break;
					default:
						goto TryAgain;
				}

			}
			
			Console.Write(" Press any key to exit..");
			Console.ReadKey(true);
		}
	}
}