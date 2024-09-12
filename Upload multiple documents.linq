<Query Kind="Program">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
  <Namespace>System.Net.Http.Json</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Nodes</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task Main() {
	const string filePath = @"C:\Users\deschaseauxr\Documents\MAFlyDoc\test.pdf";
	var documentMetadataJson =
		new Func<JsonNode>(() => {
			var documentMetadata = new {
			  deposePar = "ROD",
			  deposeLe = DateTime.Now.ToUniversalTime(),
			  categoriesFamille = "DOCUMENTS CONTRAT",
			  categoriesCote = "AUTRES",
			  categoriesTypeDocument = "DIVERS",
			  canalId = 1,
			  compteId = 595804
			};
			return JsonSerializer.SerializeToNode(documentMetadata);
		})();
	var uploadDocuments =
		await Task.WhenAll(
			Enumerable
				.Range(0, 3)
				.Select(async index => {
					documentMetadataJson["libelle"] = $"{index}-{GetRandomWord()}";
					documentMetadataJson.Dump();
					var documentId =
						await UploadDocumentToGed(
							filePath: filePath,
							documentMetadata: documentMetadataJson);
					return documentId;
				}));
	uploadDocuments.Dump();
}

const string gedApiAddress =
	"https://api-ged-intra.int.maf.local/v2/";
static readonly HttpClient gedApiHttpClient =
	new HttpClient { BaseAddress = new Uri(gedApiAddress) };
static async Task<string> UploadDocumentToGed(
	string filePath,
	JsonNode documentMetadata) {
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
		await using var stream =
			File.OpenRead(filePath);
		using var request =
			new HttpRequestMessage(
				HttpMethod.Post, 
				new Uri(
					"upload",
					UriKind.Relative));
		using var content =
			new MultipartFormDataContent { 
				{
					new StreamContent(stream), 
					"file",
					fileName
				}
			};
		request.Content = content;
		using var response = await gedApiHttpClient.SendAsync(request);
		response.EnsureSuccessStatusCode();
		var responseContent = await response.Content.ReadAsStringAsync();
		return JsonNode.Parse(responseContent)["guidFile"].GetValue<string>();
	}

	async Task<string> FinalizeUpload(
		string documentUploadId,
		string fileName,
		JsonNode documentMetadata) {
		documentMetadata["fileId"] = documentUploadId;
		documentMetadata["fichierNom"] = fileName;
		using var jsonContent = JsonContent.Create(documentMetadata);
		using var response =
			await gedApiHttpClient.PostAsync(
				new Uri(
					"finalizeUpload",
					UriKind.Relative),
				jsonContent);
		response.EnsureSuccessStatusCode();
		var uploadResponseContent =
			await response.Content.ReadAsStringAsync();
		return JsonNode
			.Parse(uploadResponseContent)["documentId"]
			.GetValue<string>();
	}
}

readonly Random dice = new();
string GetRandomWord() {
	return new string(new int[10].Select(_ => (char)(dice.Next(26) + (int)'A')).ToArray());
}
