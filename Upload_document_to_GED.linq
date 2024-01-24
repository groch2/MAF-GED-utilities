<Query Kind="Program">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
  <Namespace>System.Net.Http.Json</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Nodes</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task Main() {
	var documentMetadata = 
		JsonSerializer.SerializeToNode(
			new {
				deposePar = "ROD",
				dateDocument = DateTime.Now.ToUniversalTime(),
				categoriesFamille = "DOCUMENTS CONTRAT",
				categoriesCote = "AUTRES",
				categoriesTypeDocument = "DIVERS",
				canalId = 1
			});
	const string filePath = @"C:\Users\deschaseauxr\Documents\GED\upload_tiny_document.pdf";
	var documentId = 
		await UploadDocumentToGed(
			filePath: filePath,
			documentMetadata: documentMetadata);
	new { documentId }.Dump();
}
	
const string gedApiAddress = "https://api-ged-intra.int.maf.local/v2/";
HttpClient gedApiHttpClient = new HttpClient { BaseAddress = new Uri(gedApiAddress) };
async Task<string> UploadDocumentToGed(string filePath, JsonNode documentMetadata) {
	var fileName = Path.GetFileName(filePath);
	var documentUploadId = await UploadFile(filePath: filePath, fileName: fileName);
	var documentId =
		await FinalizeUpload(
			documentUploadId: documentUploadId,
			fileName: fileName,
			documentMetadata: documentMetadata);
	return documentId;

	async Task<string> UploadFile(string filePath, string fileName) {
		await using var stream = File.OpenRead(filePath);
		using var request = new HttpRequestMessage(HttpMethod.Post, new Uri("/v2/upload", UriKind.Relative));
		using var content = new MultipartFormDataContent { { new StreamContent(stream), "file", fileName } };
		request.Content = content;
		var uploadResponse = await gedApiHttpClient.SendAsync(request);
		uploadResponse.EnsureSuccessStatusCode();
		var responseContent = await uploadResponse.Content.ReadAsStringAsync();
		return JsonNode.Parse(responseContent)["guidFile"].GetValue<string>();
	}

	async Task<string> FinalizeUpload(string documentUploadId, string fileName, JsonNode documentMetadata) {
		documentMetadata["fileId"] = documentUploadId;
		documentMetadata["libelle"] = fileName;
		documentMetadata["fichierNom"] = fileName;
		var documentUploadJson = JsonContent.Create(documentMetadata);
		var finalizeUploadResponse =
			await gedApiHttpClient.PostAsync(new Uri("finalizeUpload", UriKind.Relative), documentUploadJson);
		finalizeUploadResponse.EnsureSuccessStatusCode();
		var uploadResponseContent = await finalizeUploadResponse.Content.ReadAsStringAsync();
		return JsonNode.Parse(uploadResponseContent)["documentId"].GetValue<string>();
	}
}
