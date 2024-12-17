<Query Kind="Statements">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
  <Namespace>System.Net.Http.Json</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Nodes</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

const string apiVersion = "v2";
var client = new HttpClient { BaseAddress = new Uri("https://api-ged-intra.int.maf.local/") };
Console.WriteLine("Enter documentId :");
var documentId = Console.ReadLine();
var downloadAddress = new Uri($"/{apiVersion}/download?documentId={documentId}", UriKind.Relative);
using var downloadRequest = new HttpRequestMessage(HttpMethod.Get, downloadAddress);
using var httpDownloadResponse = await client.SendAsync(downloadRequest);
var fileDownloadPath =
	Path.Combine(
		Path.GetTempPath(),
		"documents-GED-full-document",
		Path.GetRandomFileName());
var directoryPath = Path.GetDirectoryName(fileDownloadPath);
Directory.CreateDirectory(directoryPath);
fileDownloadPath = Path.ChangeExtension(fileDownloadPath, "pdf");
using var outputFileStream = new FileStream(fileDownloadPath, FileMode.Create);
using var downloadStream = await httpDownloadResponse.Content.ReadAsStreamAsync();
await downloadStream.CopyToAsync(outputFileStream);  

using var process = new Process();
const string chromeDirectory = @"C:\Program Files\Google\Chrome\Application";
process.StartInfo.FileName = Path.Combine(chromeDirectory, "chrome.exe");
process.StartInfo.Arguments = fileDownloadPath;
process.StartInfo.WorkingDirectory = chromeDirectory;
process.Start();
process.WaitForExit();
process.Close();