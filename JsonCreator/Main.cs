using AppKit;
using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace JsonCreator
{
	static class MainClass
	{
		static TextConverter textConverter = new TextConverter();
		static TxtJsonConverter txtJsonConverter = new TxtJsonConverter ();

		static void Main (string[] args)
		{
			string source = "/Users/voquanghoa/Downloads/gec123";
			string des = "/Users/voquanghoa/Downloads/dess";
			Directory.Delete (des, true);

			var dataItem = ProcessFolder (source, des);
			File.WriteAllText (Path.Combine (des, "data.json"), JsonConvert.SerializeObject (dataItem));
		}

		private static DataItem ProcessFolder(string source, string des){
			if (!Directory.Exists (des)) {
				Directory.CreateDirectory (des);
			} 

			var dataItem = CreateDataItem (source);
			var directoryName = Path.GetFileName (source);

			var directories = Directory.EnumerateDirectories (source);

			var children = (from x in directories
				let orgDirectoryName = Path.GetFileName (x)
				let newDirectoryName = textConverter.FormatFileName(orgDirectoryName)
				let nextSource = Path.Combine (source, orgDirectoryName)
				let nextDes = Path.Combine (des, newDirectoryName)
			                select ProcessFolder (nextSource, nextDes)).ToList ();

			if (children.Count == 0) {
				foreach (var file in Directory.EnumerateFiles (source)) {
					if (!Path.GetFileNameWithoutExtension (file).Equals (directoryName)) {
						txtJsonConverter.Convert (file, des);
					}
				}

				textConverter.RenameFilesInDir (des);

				children.AddRange(Directory.EnumerateFiles (des).Where(x=>!x.EndsWith(".DS_Store")).Select(CreateDataItem));
			}

			if (children.Count > 0) {
				dataItem.Children = children;
			}
				
			dataItem.Children?.RemoveAll (x=>(!x.FileName.EndsWith(".json")) && (x.Children == null || x.Children.Count==0));

			return dataItem;
		}

		private static DataItem CreateDataItem(string filePath){
			var display = Path.GetFileNameWithoutExtension (filePath);
			var fileName = textConverter.FormatFullFileName (Path.GetFileName(filePath));

			if (filePath.EndsWith (".json")) {
				var directory = Path.GetDirectoryName (filePath);
				var newPath = Path.Combine (directory, fileName);
				File.Move (filePath, newPath);
			}

			return new DataItem () {
				FileName = fileName,
				Display = textConverter.FormatDisplay(display)
			};
		}
	}
}
