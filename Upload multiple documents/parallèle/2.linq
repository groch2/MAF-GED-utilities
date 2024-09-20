<Query Kind="Program">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
  <Namespace>System.Net.Http.Json</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Nodes</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

const string ENVIRONMENT_CODE = "int";
const string GED_API_ADDRESS =
	$"https://api-ged-intra.{ENVIRONMENT_CODE}.maf.local/v2/";
	//"https://localhost:51691/v2/";
async Task Main() {
	const string filePath = @"C:\Users\deschaseauxr\Documents\MAFlyDoc\test.pdf";
	var dateDocument = DateTime.Now.ToUniversalTime();
	var uploadDocuments =
		await Task.WhenAll(
			Enumerable
				.Range(0, 3)
				.Select(async index => {
					var documentMetadataJson =
						new Func<JsonNode>(() => {
							var documentMetadata = new {
								canalId = 1,
								categoriesCote = "AUTRES",
								categoriesFamille = "DOCUMENTS CONTRAT",
								categoriesTypeDocument = "DIVERS",
								chantierId = 2398,
								dateDocument = dateDocument,
								deposePar = "ROD",
								libelle = $"{index + 1}-{GetRandomWord()}",
								numeroContrat = "6928B",
								periodeValiditeDebut = "1985-05-04",
								periodeValiditeFin = "1995-02-21",
								sens = "interne",
								typeGarantie = "RCD/RCG",
							};
							return JsonSerializer.SerializeToNode(documentMetadata);
						})();
					var documentId =
						await UploadDocumentToGedAsync(
							filePath: filePath,
							documentMetadata: documentMetadataJson);
					return documentId;
				}));
	new { uploadDocuments }.Dump();
}

static readonly HttpClient gedApiHttpClient =
	new HttpClient { BaseAddress = new Uri(GED_API_ADDRESS) };
static async Task<string> UploadDocumentToGedAsync(
	string filePath,
	JsonNode documentMetadata) {
	var fileName = Path.GetFileName(filePath);
	var fileUploadId =
		await UploadFile(
			filePath: filePath,
			fileName: fileName);
	var documentId =
		await FinalizeUpload(
			fileUploadId: fileUploadId,
			fileName: fileName,
			documentMetadata: documentMetadata);
	return documentId;

	static async Task<string> UploadFile(
		string filePath,
		string fileName) {
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
		using var response =
			await gedApiHttpClient.SendAsync(request);
		CheckHttpResponse(response);
		var responseContent =
			await response.Content.ReadAsStringAsync();
		var fileUploadId =
			JsonNode.Parse(responseContent)["guidFile"].GetValue<string>();
		return fileUploadId;
	}

	static async Task<string> FinalizeUpload(
		string fileUploadId,
		string fileName,
		JsonNode documentMetadata) {
		documentMetadata["fileId"] = fileUploadId;
		documentMetadata["fichierNom"] = fileName;
		using var jsonContent = JsonContent.Create(documentMetadata);
		using var response =
			await gedApiHttpClient.PostAsync(
				new Uri(
					"finalizeUpload",
					UriKind.Relative),
				jsonContent);
		CheckHttpResponse(response);
		var uploadResponseContent =
			await response.Content.ReadAsStringAsync();
		return JsonNode
			.Parse(uploadResponseContent)["documentId"]
			.GetValue<string>();
	}

	static void CheckHttpResponse(HttpResponseMessage response) {
		try {
			response.EnsureSuccessStatusCode();
		} catch (Exception exception) {
			exception.Dump();
			throw;
		}
	}
}

static readonly Random dice = new();
static string GetRandomWord() =>
	new string(
		new int[10]
			.Select(_ => (char)(dice.Next(26) + (int)'A'))
			.ToArray());
