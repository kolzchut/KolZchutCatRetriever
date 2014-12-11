using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

namespace KolZchutCatAnalyzer
{
	class Program
	{
		static void Main(string[] args)
		{
			var inputFilename = args[0];
			var outputFilename = args[1];

			var inputFile = new StreamReader(inputFilename, System.Text.Encoding.Default, false);

			var outputFile = new StreamWriter(outputFilename, false, inputFile.CurrentEncoding);

			// copy column header line
			var header = inputFile.ReadLine(); 
			outputFile.WriteLine(header);

			while (!inputFile.EndOfStream)
			{
				var line = inputFile.ReadLine().TrimEnd(new char[] {' ',','});
				if (String.IsNullOrWhiteSpace(line))
					continue;

				var columns = line.Split(new char[] { ',' });
				try
				{

					string pageURL = null;

					for (int i = 0; i<columns.Length; i++)
					{
						var text = columns[i];

						while (text.StartsWith("\"") && !text.EndsWith("\""))
							text += "," + columns[++i];

						text = text.Trim(new char[] { '\"' });

						if (text.StartsWith("http:", StringComparison.InvariantCultureIgnoreCase))
						{
							pageURL = text;
							break;
						}
					}

					if (String.IsNullOrWhiteSpace(pageURL))
						throw new Exception();

					Console.WriteLine(pageURL);

					string pageText = null;
					for (var i = 0; i < 3; i++) // retry maximum 3 times
					{
						pageText = ReadPage(pageURL);

						if (String.IsNullOrWhiteSpace(pageText))
							System.Threading.Thread.Sleep(5000); // wait 5 second before retry
						else
							break;
					}

					var categories = ExtractCategories(pageText);

					if (categories == null || categories.Count == 0)
						throw new Exception();

					outputFile.Write(line);
					foreach (var category in categories)
						outputFile.Write(",\"" + category + "\"");

					outputFile.Write(outputFile.NewLine);
				}
				catch
				{
					outputFile.WriteLine(line + @", ERROR!");
				}

				outputFile.Flush();
			}

			outputFile.Close();

		}

		private static List<string> ExtractCategories(string pageText)
		{
			try
			{
				var catlinksMatch = Regex.Match(pageText, "<!-- catlinks -->(.|\n)*<!-- /catlinks -->");
				var catlinks = catlinksMatch.Value.Replace("<!-- catlinks -->", "").Replace("<!-- /catlinks -->", "").Trim();
				XmlDocument docCatLinks = new XmlDocument();
				docCatLinks.LoadXml(catlinks);
				var liNodes = docCatLinks.GetElementsByTagName("li");
				var categories = new List<string>();
				foreach (XmlNode liNode in liNodes)
					categories.Add(liNode.InnerText);
				return categories;
			}
			catch
			{
				return null;
			}

		}

		private static string ExtractTitle(string pageText)
		{
			try
			{
				var titleMatch = Regex.Match(pageText, "<title>.*</title>");
				var title = titleMatch.Value.Replace("<title>", "").Replace("</title>", "");
				return title;
			}
			catch
			{
				return null;
			}
		}

		private static string ReadPage(string pageURL)
		{
			try
			{
				string postData = "parameter=text&param2=text2";
				ASCIIEncoding encoding = new ASCIIEncoding();
				byte[] baASCIIPostData = encoding.GetBytes(postData);

				HttpWebRequest HttpWReq = (HttpWebRequest)WebRequest.Create(pageURL);
				HttpWReq.Method = "POST";
				HttpWReq.Accept = "text/plain";
				HttpWReq.ContentType = "application/x-www-form-urlencoded";
				HttpWReq.ContentLength = baASCIIPostData.Length;

				// Prepare web request and send the data.
				Stream streamReq = HttpWReq.GetRequestStream();
				streamReq.Write(baASCIIPostData, 0, baASCIIPostData.Length);

				// grab the response
				HttpWebResponse HttpWResp = (HttpWebResponse)HttpWReq.GetResponse();
				Stream streamResponse = HttpWResp.GetResponseStream();

				// And read it out
				StreamReader reader = new StreamReader(streamResponse);
				var response = reader.ReadToEnd();

				return response;
			}
			catch
			{
				return null;
			}

		}
	}
}
