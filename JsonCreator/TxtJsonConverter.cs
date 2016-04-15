using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace JsonCreator
{
	public class TxtJsonConverter
	{
		private TextConverter textConverter = new TextConverter();

		public void Convert(string source, string desFolder){
			var fileName = Path.GetFileNameWithoutExtension(source);
			var desPath = Path.Combine (desFolder, fileName + ".json");
			var content = ReadFile (source);
			if(content != null){
				File.WriteAllText (desPath, JsonConvert.SerializeObject(content));
			}
		}

		private ListenContent ReadFile(string path){
			var content = new ListenContent ();

			var lines = File.ReadAllLines (path);
			var lineGroups = GroupLines (lines).ToList ();
			content.AddRange (lineGroups.Select (CreateQuestion));
			if (content == null || content.Count == 0 ||  (content.Any (x => x.Answers.Count==1 || x.CorrectAnswer < 0))) {
				Console.WriteLine ("File error {0} count {1} question {2}", path, content.Count, content.FindIndex(x => x.Answers.Count>1 && x.CorrectAnswer < 0));
				return null;
			}
			return content;
		}

		private IEnumerable<List<string>> GroupLines(string[] source){
			List<string> list = null;
			for (int i = 0; i <= source.Length; i++) {
				if (list == null) {
					list = new List<string> ();
				}

				if (i == source.Length || isEmpty (source [i])) {
					if (list.Count > 0) {
						yield return list;
						list = null;
					}
				} else {
					list.Add (source [i]);
				}
			}
		}

		private Question CreateQuestion(List<string> lines){
			var question = new Question ();
			question.QuestionTitle = textConverter.FormatQuestion(lines [0]);
			question.CorrectAnswer = Enumerable.Range (1, lines.Count -1).FirstOrDefault (x => lines [x].StartsWith (" ")) - 1;
			question.Answers = lines.Skip (1).Select (x => x.Trim ()).ToList ();
			return question;
		}

		private bool isEmpty(string str){
			return string.IsNullOrWhiteSpace (str) || string.IsNullOrEmpty (str);
		}
	}


}

