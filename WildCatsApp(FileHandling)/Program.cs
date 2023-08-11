using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WildCatsApplication_FileHandling_
{
	public class Program
	{
		static void Main( string[] args )
		{
			string content = String.Empty;
			string _rootPath = AppDomain.CurrentDomain.BaseDirectory;

			try
			{
				DirectoryInfo sourceDirectory = InitialiseSourceDirectory(_rootPath);

				string htmlOutputFilePath = Path.Combine(_rootPath, "WildCats.html");

				FileInfo[] textFiles = sourceDirectory.GetFiles("*.txt");

				using (var htmlFile = new StreamWriter(htmlOutputFilePath))
				{
					AddTopHTML(htmlFile);

					foreach (var textFile in textFiles)
					{
						using (StreamReader streamReader = new StreamReader(textFile.OpenRead()))
						{
							content = streamReader.ReadToEnd();
						}
						BuildHtmlBody(htmlFile, content, Path.GetFileNameWithoutExtension(textFile.FullName));
					}
					AddBottomHtml(htmlFile);
				}
				LaunchHtmlFileInBrowser(htmlOutputFilePath);
			}
			catch (UnauthorizedAccessException ex)
			{
				LogExceptionMessageToScreen(ex);
			}
			catch (DirectoryNotFoundException ex)
			{
				LogExceptionMessageToScreen(ex);
			}
			catch (FileNotFoundException ex)
			{
				LogExceptionMessageToScreen(ex);
			}
			catch (IOException ex)
			{
				LogExceptionMessageToScreen(ex);
			}
			catch (NotImplementedException ex)
			{
				LogExceptionMessageToScreen(ex);
			}
		}

		private static void LogExceptionMessageToScreen( Exception ex )
		{
			Console.BackgroundColor = ConsoleColor.DarkRed;
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(ex.Message);
			Console.ResetColor();
		}

		private static void AddTopHTML( StreamWriter sw )
		{
			sw.WriteLine("<!doctype html>");
			sw.WriteLine(@"<html lang = ""en"">");
			sw.WriteLine("<head>");
			sw.WriteLine(@"<meta charset = ""utf-8"">");
			sw.WriteLine(@"<meta name = ""viewport"" content = ""width=device-width,intial-scale=1"">");
			sw.WriteLine("<title>Wild Cats</title>");
			sw.WriteLine(@"<link rel = ""stylesheet"" href = ""https://code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css"" >");
			sw.WriteLine(@"<script src = ""https://code.jquery.com/jquery-1.12.4.js""></script>");
			sw.WriteLine(@"<script src = ""https://code.jquery.com/ui/1.12.1/jquery-ui.js""></script>");

			sw.WriteLine("<script>");
			sw.WriteLine("$(function(){");
			sw.WriteLine(@"$(""#accordion"").accordion();");
			sw.WriteLine("});");
			sw.WriteLine("</script>");
			sw.WriteLine("</head>");
			sw.WriteLine("<body>");

			sw.WriteLine(@"<h1 style=""text-align:center;font-family:arial"">Wild Cats</h1> ");
			sw.WriteLine(@"<div id = ""accordion"">");
		}

		private static void BuildHtmlBody( StreamWriter sw, string topicContent, string topicHeading )
		{
			sw.WriteLine($"<h3>{topicHeading}</h3>");
			sw.WriteLine("<div>");
			sw.WriteLine("<p>");
			sw.Write(topicContent);
			sw.WriteLine("</p>");
			sw.WriteLine("</div>");
		}

		private static void AddBottomHtml( StreamWriter sw )
		{
			sw.WriteLine("</div>");
			sw.WriteLine("</body>");
			sw.WriteLine("</html>");
		}

		private static void LaunchHtmlFileInBrowser( string url )
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				url = url.Replace("&", "^&");
				Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				Process.Start("xdg-open", url);
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				Process.Start("open", url);
			}
			else
			{
				throw new NotImplementedException("Unable to launch your default browser through this application.");
			}
		}

		private static DirectoryInfo InitialiseSourceDirectory( string rootPath )
		{
			string wildCatDirectoryPath = Path.Combine(rootPath, "WildCats");
			string infoFilePath = Path.Combine(wildCatDirectoryPath, "Information.txt");

			if (!Directory.Exists(wildCatDirectoryPath))
			{
				Directory.CreateDirectory(wildCatDirectoryPath);
			}

			DirectoryInfo sourceDirectory = new DirectoryInfo(wildCatDirectoryPath);

			int numOfTextFilesInDirectory = sourceDirectory.GetFiles("*.txt").Length;

			if (numOfTextFilesInDirectory == 0)
			{
				using StreamWriter streamWriter = File.CreateText(infoFilePath);
				streamWriter.WriteLine($"Text files has not been added to this directory, {wildCatDirectoryPath}");
			}
			else if (numOfTextFilesInDirectory > 1 && File.Exists(infoFilePath))
			{
				File.Delete(infoFilePath);
			}
			return sourceDirectory;
		}
	}
}