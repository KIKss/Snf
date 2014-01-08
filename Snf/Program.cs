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
			Console.WriteLine("	-p path   :Directory for check");
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
						Tools.Checked_dir = args[i + 1];
					}
				}
				if (args[i] == "-cp")
				{
					if (!String.IsNullOrEmpty(args[i + 1]))
					{
						Tools.Save_dir = args[i + 1];
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
			
			if (!Directory.Exists(Tools.Checked_dir)) {
				Console.WriteLine(" Error. No folder for checking");
				Console.WriteLine(" Press any key to exit..");
				Console.ReadKey(true);
				return;
			}
			
			Tools.Save_dir = Tools.Save_dir+Tools.Checked_dir.Substring(Tools.Checked_dir.LastIndexOf(Path.DirectorySeparatorChar));
			Console.WriteLine(" Checking folder: "+Tools.Checked_dir);
			Console.WriteLine(" Save to: "+Tools.Save_dir+"\n");
			Tools.DB_file_name = Environment.CurrentDirectory+
				Tools.Checked_dir.Substring(Tools.Checked_dir.LastIndexOf(Path.DirectorySeparatorChar))+
				".data";
			
			List<Tools.File_Data> Base;
			List<Tools.File_Data> NBase;
			List<string> ToWork;
			
			if(!(File.Exists(Tools.DB_file_name)) )
			{
				Console.WriteLine(" First run. Creating folder image..");
				Tools.CreateImage(Tools.Checked_dir);
				
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
						Base = Tools.LoadDB(Tools.DB_file_name);
						Console.WriteLine(" Checking folder..");
						
						NBase = Tools.GetDirList(Tools.Checked_dir);
						
						ToWork = Tools.CheckDirectories(Base,NBase);
						
						if (ToWork.Count != 0) {
							Console.WriteLine(" Files found: "+ ToWork.Count);
							
							
							
							if(log || onlylog)
							{
								File.WriteAllLines(Environment.CurrentDirectory+
								                   Tools.Checked_dir.Substring(Tools.Checked_dir.LastIndexOf(Path.DirectorySeparatorChar))+
								                   ".txt",ToWork);
								Console.WriteLine(" Log created.");
								if(onlylog){Tools.SaveDB(NBase);break;}
							}
							
							Console.WriteLine(" Copying files..");
							if (Directory.Exists(Tools.Save_dir))
							{
								DirectoryInfo directory = new DirectoryInfo(Tools.Save_dir);
								foreach (System.IO.FileInfo file in directory.GetFiles()) file.Delete();
								foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
							}
							
							foreach (var w in ToWork) {
								
								string CRPath;
								CRPath = Tools.Save_dir+Path.DirectorySeparatorChar+Path.GetDirectoryName(w.Replace(Tools.Checked_dir,""));
								
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
						Tools.CreateImage(Tools.Checked_dir);
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