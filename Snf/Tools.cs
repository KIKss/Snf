using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace snf
{
	public class Tools
	{
		public static string Checked_dir = "";
		public static string Save_dir = Environment.CurrentDirectory;
		public static string DB_file_name = "";
		
		[Serializable]
		public struct File_Data
		{
			public string Path;
			public DateTime Change_date;
			public DateTime Create_date;
			
			public File_Data(string _path, DateTime _ch_date, DateTime _cr_date)
			{
				Path = _path;
				Change_date = _ch_date;
				Create_date = _cr_date;
			}
		}
		
		
		public static List<string> CheckDirectories(List<File_Data> _base, List<File_Data> _nbase)
		{
			Tools.File_Data f;
			List<string> towork = new List<string>();
			
//			foreach (Tools.File_Data fd in _nbase)
//			{
//				f = _base.Find(p => p.Path == fd.Path);
//				if(f.Path == "" ||f.Change_date != fd.Change_date || f.Create_date != fd.Create_date)
//				{
//					towork.Add(fd.Path);
//				}
//			}
//			return towork;
			
			Object sync = new object();
			
			Parallel.ForEach(_nbase, fd =>
			                 {
			                 	f = _base.Find(p => p.Path == fd.Path);
			                 	if(f.Path == "" ||f.Change_date != fd.Change_date || f.Create_date != fd.Create_date)
			                 	{
			                 		lock(sync)
			                 		{
			                 			towork.Add(fd.Path);
			                 		}
			                 	}
			                 	

			                 });
			
			
			return towork;
		}
		
		public static void CreateImage(string dir)
		{
			List<File_Data> image;
			image = Tools.GetDirList(dir);
			Tools.SaveDB(image);
			Console.WriteLine(" Complete. Files found: "+image.Count);
		}

		public static List<File_Data> GetDirList(string chdir)
		{
			DirectoryInfo dir=new DirectoryInfo(chdir);
			List<File_Data> Data = new List<Tools.File_Data>();
			foreach (FileInfo file in dir.GetFiles("*",SearchOption.AllDirectories))
			{
				Data.Add(new File_Data(file.FullName,File.GetLastWriteTime(file.FullName),File.GetCreationTime(file.FullName)));
			}
			return Data;
		}
		
		public static void SaveDB(List<File_Data> data)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			using(var fStream = new FileStream(DB_file_name, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				formatter.Serialize(fStream, data);
			}
		}
		
		public static List<File_Data> LoadDB(string dbfile)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			using(var fStream = File.OpenRead(dbfile))
			{
				return (List<File_Data>)formatter.Deserialize(fStream);
			}
		}
		
	}
}
