using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace JsonCreator
{
	public class TextConverter
	{
		private bool isValidCharacter(char x){
			x = Char.ToLower (x);
			if (Char.IsNumber (x)) {
				return true;
			}

			if (x <= 'z' && x >= 'a') {
				return true;
			}

			return "._".Contains (x);

		}

		public void RenameFilesInDir(string dir){
			var fileNames = Directory.EnumerateFiles (dir, "lesson_*.json").OrderBy(x=>x).ToList();

			foreach (var fileName in fileNames) {
				ReplaceFile (fileName, fileName + "_");
			}

			for (int i = 0; i < fileNames.Count; i++) {
				ReplaceFile (fileNames [i] + "_", Path.Combine(dir, String.Format ("lesson_{0:D2}.json", i + 1))); 
			}
		}

		private void ReplaceFile(string source, string des){
			if (File.Exists (des)) {
				File.Delete (des);
			}
			File.Move (source, des);
		}

		public string FormatFullFileName(string fileName){
			return new string (fileName.Select (x => isValidCharacter (x) ? x : '_').ToArray ());
		}

		public string FormatFileName(string fileName){
			fileName = FormatFullFileName (fileName);
			return Path.GetFileNameWithoutExtension (fileName);
		}

		public string FormatDisplay(string fileName){
			fileName = Path.GetFileNameWithoutExtension (fileName);
			fileName = Char.ToUpper (fileName [0]) + fileName.Substring (1).ToLower ();
			fileName = fileName.Replace ('_', ' ');
			return fileName;
		}

		public string FormatQuestion(string question){
			return Regex.Replace(question, @"^(\d)*\. ","");
		}
	}
}

