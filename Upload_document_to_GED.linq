<Query Kind="Statements">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
  <Namespace>System.Net.Http.Json</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Nodes</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

var documentMetadata = 
	JsonSerializer.SerializeToNode(
		new {
			deposePar = "ROD",
			dateDocument = DateTime.Now.ToUniversalTime(),
			categoriesFamille = "DOCUMENTS CONTRAT",
			categoriesCote = "AUTRES",
			categoriesTypeDocument = "DIVERS",
			canalId = 1,
		});
const string filePath = @"C:\Users\deschaseauxr\Documents\GED\upload_tiny_document.pdf";
var documentId = 
	await UploadDocumentToGed(
		filePath: filePath,
		documentMetadata: documentMetadata);
new { documentId }.Dump();

async Task<string> UploadDocumentToGed(string filePath, JsonNode documentMetadata) {
	var fileName = Path.GetFileName(filePath);
	var client = new HttpClient { BaseAddress = new Uri("https://api-ged-intra.int.maf.local/") };

	await using var stream = File.OpenRead(filePath);
	using var request = new HttpRequestMessage(HttpMethod.Post, new Uri("/v2/upload", UriKind.Relative));
	using var content = new MultipartFormDataContent { { new StreamContent(stream), "file", fileName } };
	request.Content = content;

	var uploadResponse = await client.SendAsync(request);
	uploadResponse.EnsureSuccessStatusCode();
	var responseContent = await uploadResponse.Content.ReadAsStringAsync();
	var documentUploadId = JsonNode.Parse(responseContent)["guidFile"].GetValue<string>();

	documentMetadata["fileId"] = documentUploadId;
	documentMetadata["libelle"] = fileName;
	documentMetadata["fichierNom"] = fileName;
	var documentUploadJson = JsonContent.Create(documentMetadata);
	var finalizeUploadResponse = 
		await client.PostAsync(new Uri("/v2/finalizeUpload", UriKind.Relative), documentUploadJson);
	finalizeUploadResponse.EnsureSuccessStatusCode();
	var uploadResponseContent = await finalizeUploadResponse.Content.ReadAsStringAsync();
	return JsonNode.Parse(uploadResponseContent)["documentId"].GetValue<string>();
}