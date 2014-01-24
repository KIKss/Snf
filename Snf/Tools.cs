using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace snf
{
	public class Tools
	{
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
		
		public static List<File_Data> GetDirList(string chdir)
		{
			DirectoryInfo dir=new DirectoryInfo(chdir);
			List<File_Data> Data = new List<Tools.File_Data>();

			foreach (FileInfo file in dir.GetFiles("*",SearchOption.AllDirectories))
			{
				Data.Add(new File_Data(file.FullName,file.LastWriteTime,file.CreationTime));
			}
			return Data;
		}
		
		public static List<string> CheckDirectories(List<File_Data> _base, List<File_Data> _nbase)
		{
			Tools.File_Data f;
			List<string> towork = new List<string>();
			Object syn = new object();
			Parallel.ForEach(_nbase, fd =>
			                 {
			                 	f = _base.Find(p => p.Path == fd.Path);
			                 	if(f.Path == "" ||f.Change_date != fd.Change_date || f.Create_date != fd.Create_date)
			                 	{
			                 		lock(syn)
			                 		{
			                 			towork.Add(fd.Path);
			                 		}
			                 	}
			                 }
			                );
			return towork;
		}
		
		public static void CreateImage(string dir,string fname)
		{
			List<File_Data> image;
			image = Tools.GetDirList(dir);
			Tools.SaveDB(image,fname);
			Console.WriteLine(" Complete. Files found: "+image.Count);
		}
		
		public static void CopyFiles(List<string> _towork,string svdir,string chdir)
		{
			Parallel.ForEach(_towork, w =>
			                 {
			                 	string crpath;
			                 	crpath = svdir+Path.DirectorySeparatorChar+Path.GetDirectoryName(w.Replace(chdir,""));
			                 	if (!Directory.Exists(crpath)){Directory.CreateDirectory(crpath);}
			                 	File.Copy(w,crpath+Path.DirectorySeparatorChar+w.Substring(w.LastIndexOf(Path.DirectorySeparatorChar)));
			                 }
			                );
			
		}
		
		public static void SaveDB(List<File_Data> data,string fname)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			using(var fStream = new FileStream(fname, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				formatter.Serialize(fStream, data);
			}
		}
		
		public static List<File_Data> LoadDB(string fname)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			using(var fStream = File.OpenRead(fname))
			{
				return (List<File_Data>)formatter.Deserialize(fStream);
			}
		}
		
	}
}
