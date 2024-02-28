<Query Kind="Statements">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
  <Namespace>System.Net.Http.Json</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Nodes</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

const string filePath =
	@"C:\Users\deschaseauxr\AppData\Local\Temp\new 4.txt";
var fileName = Path.GetFileName(filePath);
const string gedApiVersion = "v2";

await using var stream = File.OpenRead(filePath);
using var requestUpload =
	new HttpRequestMessage(HttpMethod.Post, new Uri($"/{gedApiVersion}/upload", UriKind.Relative));
using var content = new MultipartFormDataContent { { new StreamContent(stream), "file", fileName } };
requestUpload.Content = content;

var client = new HttpClient { BaseAddress = new Uri("https://api-ged-intra.int.maf.local/") };
var httpResponseUpload = await client.SendAsync(requestUpload);
httpResponseUpload.EnsureSuccessStatusCode();
var uploadResponseContent = await httpResponseUpload.Content.ReadAsStringAsync();
var uploadId = JsonNode.Parse(uploadResponseContent)["guidFile"].GetValue<string>();

var documentUpload = new {
  fileId = uploadId,
  libelle = fileName,
  deposePar = "ROD",
  dateDocument = DateTime.Now.ToUniversalTime(),
  fichierNom = fileName,
  fichierTaille = new FileInfo(filePath).Length,
  categoriesFamille = "DOCUMENTS CONTRAT",
  categoriesCote = "AUTRES",
  categoriesTypeDocument = "DIVERS",
  canalId = 1,
};

var finalizeDocumentUploadJson = JsonContent.Create(documentUpload);
var finalizeUploadAddress = new Uri($"/{gedApiVersion}/finalizeUpload", UriKind.Relative);
var finalizeUploadResponse =
	await client.PostAsync(finalizeUploadAddress, finalizeDocumentUploadJson);
var finalizeUploadResponseContent =
	await finalizeUploadResponse.Content.ReadAsStringAsync();
finalizeUploadResponseContent.Dump();
var documentId = JsonNode.Parse(finalizeUploadResponseContent)["documentId"].GetValue<string>();

var documentDownloadAddress =
	new Uri($"/{gedApiVersion}/download?documentId={documentId}", UriKind.Relative);
using var documentDownloadRequest =
	new HttpRequestMessage(HttpMethod.Get, documentDownloadAddress);
var documentDownloadResponse = await client.SendAsync(documentDownloadRequest);
documentDownloadResponse.EnsureSuccessStatusCode();
var fileDownloadPath =
	Path.Combine(
		Path.GetTempPath(),
		Path.ChangeExtension(
			Path.GetRandomFileName(), "pdf"
		)
	);
using var outputFileStream = new FileStream(fileDownloadPath, FileMode.Create);
using var documentDownloadStream = await documentDownloadResponse.Content.ReadAsStreamAsync();
await documentDownloadStream.CopyToAsync(outputFileStream);  

using var process = new Process();
process.StartInfo.UseShellExecute = true;
process.StartInfo.WorkingDirectory =
	Path.Combine(
		Environment.GetEnvironmentVariable("ProgramFiles(x86)"),
		@"Google\Chrome\Application\");
process.StartInfo.FileName = "chrome.exe";
process.StartInfo.Arguments = fileDownloadPath;
process.Start();
process.WaitForExit();
process.Close();