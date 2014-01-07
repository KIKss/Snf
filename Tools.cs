using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace snf
{
	public class Tools
	{
		
		public static string CHDir = "";
		public static string SDir = Environment.CurrentDirectory;
		public static string DBFile = "";
		
		[Serializable]
		public class FDataClass
		{
			public string Path;
			public DateTime CHDate;
			public DateTime CRDate;
			
			public FDataClass(string p, DateTime ch,DateTime cr)
			{
				Path = p;
				CHDate = ch;
				CRDate = cr;
			}
		}

		public static List<FDataClass> GetDirList()
		{
			DirectoryInfo dir=new DirectoryInfo(CHDir);
			List<FDataClass> Data = new List<Tools.FDataClass>();
			foreach (FileInfo file in dir.GetFiles("*",SearchOption.AllDirectories))
			{
					Data.Add(new FDataClass(file.FullName,File.GetLastWriteTime(file.FullName),File.GetCreationTime(file.FullName)));
			}
			return Data;
		}
		
		public static void SaveDB(List<FDataClass> data)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			using(var fStream = new FileStream(DBFile, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				formatter.Serialize(fStream, data);
			}
		}
		
		public static List<FDataClass> LoadDB(string dbfile)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			using(var fStream = File.OpenRead(dbfile))
			{
				return (List<FDataClass>)formatter.Deserialize(fStream);
			}
			
		}
		
	}
}
