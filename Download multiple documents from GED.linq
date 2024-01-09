<Query Kind="Program">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
  <Namespace>System.Net.Http.Json</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Nodes</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task Main()
{
	var downloadDocuments =
		File
			.ReadAllLines(@"C:\Users\deschaseauxr\AppData\Local\Temp\documents_id_list.txt")
			.Select(DownloadDocumentByDocumentId)
			.ToArray();
	await Task.WhenAll(downloadDocuments);
	new { downloadDocuments = downloadDocuments.Select(document => document.Result) }.Dump();
}

const string apiVersion = "v2";
string downloadDirectory =
	Path.Combine(
		Path.GetTempPath(),
		"documents-GED-full-document");
HttpClient client = new HttpClient { BaseAddress = new Uri("https://api-ged-intra.int.maf.local/") };
async Task<string> DownloadDocumentByDocumentId(string documentId) {
	var downloadAddress = new Uri($"/{apiVersion}/download?documentId={documentId}", UriKind.Relative);
	using var downloadRequest = new HttpRequestMessage(HttpMethod.Get, downloadAddress);
	var httpDownloadResponse = await client.SendAsync(downloadRequest);
	if (Directory.Exists(downloadDirectory)) {
		Directory.CreateDirectory(downloadDirectory);
	}
	var fileDownloadPath =
		Path.Combine(
			downloadDirectory,
			Path.GetRandomFileName());
	fileDownloadPath = Path.ChangeExtension(fileDownloadPath, "pdf");
	using var outputFileStream = new FileStream(fileDownloadPath, FileMode.Create);
	using var downloadStream = await httpDownloadResponse.Content.ReadAsStreamAsync();
	await downloadStream.CopyToAsync(outputFileStream);
	return fileDownloadPath;
}
