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
	//"https://localhost:44391/v2/";
const string FILE_PATH =
	@"C:\Users\deschaseauxr\Documents\GED\Document de test\test.txt";
async Task Main() {
	var documentMetadata =
		JsonSerializer.SerializeToNode(
			new {
				deposePar = "ROD",
				dateDocument = DateTime.Now.ToUniversalTime(),
				categoriesFamille = "DOCUMENTS PAPS",
				categoriesCote = "GESTION",
				categoriesTypeDocument = "AUTRE",
				canalId = 1,
				libelle = GetRandomWord(length: 10),
				assigneRedacteur = "GABU",
				//numeroSinistre = "MA-24-443-8858090-J",
				//assigneRedacteur = (object)null,
				//compteId = 200232,
				//numeroSinistre = "DS-19-000-1100-W",
			});
	var documentId = 
		await UploadDocumentToGed(
			filePath: FILE_PATH,
			documentMetadata: documentMetadata);
	new {
		documentId,
		libelle = documentMetadata["libelle"].GetValue<string>()
	}.Dump();
}

static HttpClient gedApiHttpClient =
	new HttpClient { BaseAddress = new Uri(GED_API_ADDRESS) };
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
		documentMetadata.Dump();
		using var documentUploadJson = JsonContent.Create(documentMetadata);
		using var response =
			await gedApiHttpClient.PostAsync(
				new Uri(
					"finalizeUpload",
					UriKind.Relative),
				documentUploadJson);
		try {
			response.EnsureSuccessStatusCode();
		} catch (HttpRequestException exception) {
			var responseContent = await response.Content.ReadAsStringAsync();
			new { Error = exception.HttpRequestError, responseContent }.Dump();
			throw;
		}
		var uploadResponseContent = await response.Content.ReadAsStringAsync();
		return JsonNode
			.Parse(uploadResponseContent)["documentId"]
			.GetValue<string>();
	}
}

static Random dice = new Random();
static string GetRandomWord(int length = 10) =>
	new string(new int[length].Select(_ => (char)(dice.Next(26) + (int)'A')).ToArray());
