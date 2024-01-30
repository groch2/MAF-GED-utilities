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
	var documentUploadId =
		await UploadFile(
			filePath: filePath,
			fileName: fileName);
	var documentId =
		await FinalizeUpload(
			documentUploadId: documentUploadId,
			fileName: fileName,
			documentMetadata: documentMetadata);
	return documentId;

	async Task<string> UploadFile(string filePath, string fileName) {
		await using var stream = File.OpenRead(filePath);
		using var request = new HttpRequestMessage(HttpMethod.Post, new Uri("upload", UriKind.Relative));
		using var content = new MultipartFormDataContent { { new StreamContent(stream), "file", fileName } };
		request.Content = content;
		using var response = await gedApiHttpClient.SendAsync(request);
		response.EnsureSuccessStatusCode();
		var responseContent = await response.Content.ReadAsStringAsync();
		return JsonNode.Parse(responseContent)["guidFile"].GetValue<string>();
	}

	async Task<string> FinalizeUpload(string documentUploadId, string fileName, JsonNode documentMetadata) {
		documentMetadata["fileId"] = documentUploadId;
		documentMetadata["libelle"] = fileName;
		documentMetadata["fichierNom"] = fileName;
		using var documentUploadJson = JsonContent.Create(documentMetadata);
		using var response =
			await gedApiHttpClient.PostAsync(new Uri("finalizeUpload", UriKind.Relative), documentUploadJson);
		response.EnsureSuccessStatusCode();
		var uploadResponseContent = await response.Content.ReadAsStringAsync();
		return JsonNode.Parse(uploadResponseContent)["documentId"].GetValue<string>();
	}
}
