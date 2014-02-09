using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace snf
{
	
	class Program
	{
		
		public static void Main(string[] @args)
		{
			Console.Title = "Find new files";
			ConsoleKey vvd;
			bool autostart = false;
			bool onlylog = false;
			bool log = false;
			string Checked_dir = String.Empty;
			string Save_dir = Environment.CurrentDirectory;
			string DB_file_name = String.Empty;
			//string CRPath;
			List<Tools.File_Data> Base;
			List<Tools.File_Data> NBase;
			List<string> ToWork;
			
			Console.WriteLine("	===================================================================");
			Console.WriteLine("			Search new and modified files in directory");
			Console.WriteLine("	___________________________________________________________________");
			Console.WriteLine("	command line arguments:");
			Console.WriteLine("	-p \"path\"   :Directory for check");
			Console.WriteLine("	-cp \"path\"  :Folder for modified files (program directory by default)");
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
						Checked_dir = args[i + 1];
					}
				}
				if (args[i] == "-cp")
				{
					if (!String.IsNullOrEmpty(args[i + 1]))
					{
						Save_dir = args[i + 1];
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
			
			if (!Directory.Exists(Checked_dir)) {
				Console.WriteLine(" Error. No folder for checking");
				Console.WriteLine(" Press any key to exit..");
				Console.ReadKey(true);
				return;
			}
			
			Save_dir = Save_dir+Checked_dir.Substring(Checked_dir.LastIndexOf(Path.DirectorySeparatorChar));
			DB_file_name = Environment.CurrentDirectory+
				Checked_dir.Substring(Checked_dir.LastIndexOf(Path.DirectorySeparatorChar))+
				".data";
			Console.WriteLine(" Checking folder: "+Checked_dir);
			Console.WriteLine(" Save to: "+Save_dir+"\n");
			
			if(!(File.Exists(DB_file_name)) )
			{
				Console.WriteLine(" New checking directory. Creating folder image..");
				Tools.CreateImage(Checked_dir,DB_file_name);
				
			}else{
				if(autostart){vvd = ConsoleKey.Enter;goto Start;}
				Console.WriteLine(" Press: ");
				Console.WriteLine(" Enter - check folder for changes");
				Console.WriteLine(" n - Create folder image");
			TryAgain:
				vvd = Console.ReadKey(true).Key;
			Start:
				
				switch (vvd) {
					case ConsoleKey.Enter:
						Console.WriteLine(" Checking folder..");
						Base = Tools.LoadDB(DB_file_name);
						NBase = Tools.GetDirList(Checked_dir);
						ToWork = Tools.CheckDirectories(Base,NBase);
						
						if (ToWork.Count != 0) {
							Console.WriteLine(" Files found: "+ ToWork.Count);
							if(log || onlylog)
							{
								File.WriteAllLines(Environment.CurrentDirectory+
								                   Checked_dir.Substring(Checked_dir.LastIndexOf(Path.DirectorySeparatorChar))+
								                   ".txt",ToWork);
								Console.WriteLine(" Log created.");
								if(onlylog){Tools.SaveDB(NBase,DB_file_name);break;}
							}
							
							Console.WriteLine(" Copying files..");
							if (Directory.Exists(Save_dir))
							{
								DirectoryInfo directory = new DirectoryInfo(Save_dir);
								foreach (System.IO.FileInfo file in directory.GetFiles()) {file.Delete();}
								foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) {subDirectory.Delete(true);}
							}
							Tools.CopyFiles(ToWork,Save_dir,Checked_dir);
							Tools.SaveDB(NBase,DB_file_name);
							Console.WriteLine(" Complete.");
						}else{
							Console.WriteLine(" Files not found.");
						}
						
						break;
						
					case ConsoleKey.N:
						Console.WriteLine(" Creating folder image..");
						Tools.CreateImage(Checked_dir,DB_file_name);
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