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
	//$"https://api-ged-intra.{ENVIRONMENT_CODE}.maf.local/v2/";
	//"http://localhost:44363/v2/";
	"https://localhost:51691/v2/";
async Task Main() {
	const string filePath = @"C:\Users\deschaseauxr\Documents\MAFlyDoc\test.pdf";
	var documentMetadataJson =
		new Func<JsonNode>(() => {
			var documentMetadata = new {
			  deposePar = "ROD",
			  dateDocument = DateTime.Now.ToUniversalTime(),
			  categoriesFamille = "DOCUMENTS CONTRAT",
			  categoriesCote = "AUTRES",
			  categoriesTypeDocument = "DIVERS",
			  canalId = 1,
			};
			return JsonSerializer.SerializeToNode(documentMetadata);
		})();
	var uploadFilesList =
		await Task.WhenAll(
			Enumerable
				.Range(0, 3)
				.Select(async index => {
					var uploadFileId = await UploadFile(filePath);
					return new { uploadFileId, filePath };
				}));
	var documentsIdList =
		await Task.WhenAll(
			uploadFilesList
				.Select(async (uploadFile, index) => {
				documentMetadataJson["libelle"] = $"{index + 1}-{GetRandomWord()}";
				var documentId =
					await FinalizeUpload(
						documentUploadId: uploadFile.uploadFileId,
						fileName: Path.GetFileName(uploadFile.filePath),
						documentMetadata: documentMetadataJson);
				return documentId;
			}
		));
	documentsIdList.Dump();
}

static readonly HttpClient gedApiHttpClient =
	new HttpClient { BaseAddress = new Uri(GED_API_ADDRESS) };
async Task<string> UploadFile(string filePath) {
	await using var stream =
		File.OpenRead(filePath);
	using var request =
		new HttpRequestMessage(
			HttpMethod.Post,
			new Uri(
				"upload",
				UriKind.Relative));
	var fileName = Path.GetFileName(filePath);
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
	var documentUploadId =
		JsonNode.Parse(responseContent)["guidFile"].GetValue<string>();
	new { documentUploadId }.Dump();
	return documentUploadId;
}

async Task<string> FinalizeUpload(
	string documentUploadId,
	string fileName,
	JsonNode documentMetadata) {
	documentMetadata["fileId"] = documentUploadId;
	documentMetadata["fichierNom"] = fileName;
	documentMetadata.Dump();
	using var jsonContent = JsonContent.Create(documentMetadata);
	using var response =
		await gedApiHttpClient.PostAsync(
			new Uri(
				"finalizeUpload",
				UriKind.Relative),
			jsonContent);
	try {
		response.EnsureSuccessStatusCode();
	} catch (Exception exception) {
		exception.Dump();
		throw;
	}
	var uploadResponseContent =
		await response.Content.ReadAsStringAsync();
	return JsonNode
		.Parse(uploadResponseContent)["documentId"]
		.GetValue<string>();
}

static readonly Random dice = new();
static string GetRandomWord() =>
	new string(new int[10].Select(_ => (char)(dice.Next(26) + (int)'A')).ToArray());
