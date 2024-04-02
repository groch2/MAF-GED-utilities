<Query Kind="Program">
  <Reference>C:\TeamProjects\GED API\MAF.GED.API.Host\bin\Debug\net6.0\MAF.GED.Domain.Model.dll</Reference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
  <Namespace>System.Net.Http.Json</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Nodes</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

const string ENVIRONMENT_CODE = "int";
const string gedApiBaseAddress = $"https://api-ged-intra.{ENVIRONMENT_CODE}.maf.local/";
async Task Main()
{
	var filePathList = Directory.GetFiles(
		@"C:\Users\deschaseauxr\Documents\GED\batch de génération de preview\surcharge de la RAM - plantage de OPCON\documents de test upload\documents");
	var uploadDocuments =
		filePathList
			.Select(async (filePath, index) => {
				var documentId = await UploadDocument(filePath, index + 1);
				return documentId;
			});
	await Task.WhenAll(uploadDocuments);
	var documentIdList = uploadDocuments.Select(uploadDocument => uploadDocument.Result).ToArray();
	documentIdList.Dump();
	File.WriteAllLines(
		path: @"C:\Users\deschaseauxr\Documents\GED\batch de génération de preview\surcharge de la RAM - plantage de OPCON\documents de test upload\documents id in ged api.txt",
		contents: documentIdList);
	var uploadDateTime = DateTime.Now.ToString("G", System.Globalization.CultureInfo.GetCultureInfo("FR-fr"));
	$"instant d'upload des document: {uploadDateTime}".Dump();

	const string documentsIdListFile =
		@"C:\Users\deschaseauxr\Documents\GED\batch de génération de preview\surcharge de la RAM - plantage de OPCON\documents de test upload\documents id in ged api.txt";
	(await GetMultipleDocumentsByDocumentIdList(File.ReadAllLines(documentsIdListFile))).Dump();
}

HttpClient httpClient = new HttpClient { BaseAddress = new Uri(gedApiBaseAddress) };
async Task<string> UploadDocument(string filePath, int index) {
	var fileName = Path.GetFileName(filePath);

	await using var stream = File.OpenRead(filePath);
	using var request = new HttpRequestMessage(HttpMethod.Post, new Uri("/v2/upload", UriKind.Relative));
	using var content = new MultipartFormDataContent { { new StreamContent(stream), "file", fileName } };
	request.Content = content;

	var httpResponse = await httpClient.SendAsync(request);
	var responseContent = await httpResponse.Content.ReadAsStringAsync();
	var uploadId = JsonNode.Parse(responseContent)["guidFile"].GetValue<string>();

	var documentUpload = new {
	  fileId = uploadId,
	  libelle = $"{index}-{GetRandomWord()}",
	  deposePar = "POL",
	  dateDocument = DateTime.Now.ToUniversalTime(),
	  fichierNom = fileName,
	  fichierTaille = new FileInfo(filePath).Length,
	  categoriesFamille = "DOCUMENTS EMOA",
	  categoriesCote = "AUTRES",
	  categoriesTypeDocument = "DIVERS",
	  canalId = 1,
	  compteId = 595804
	};
	var documentUploadJson = JsonContent.Create(documentUpload);
	var uploadResponse = 
		await httpClient.PostAsync(new Uri("/v2/finalizeUpload", UriKind.Relative), documentUploadJson);
	var uploadResponseContent = await uploadResponse.Content.ReadAsStringAsync();
	var documentId = JsonNode.Parse(uploadResponseContent)["documentId"].GetValue<string>();
	return documentId;
}

async Task<IEnumerable<object>> GetMultipleDocumentsByDocumentIdList(IEnumerable<string> documentsIdList) {
	var documentsIdListFormatted =
		string.Join(',', documentsIdList.Select(documentId => $"'{documentId}'"));
	var json_documents = await httpClient.GetStringAsync($"v2/documents?$filter=documentId in ({documentsIdListFormatted})");
	var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
	return
		JsonDocument
			.Parse(json_documents)
			.RootElement.GetProperty("value")
			.EnumerateArray()
			.Select(document =>
				JsonSerializer.Deserialize<MAF.GED.Domain.Model.Document>(
					json: document.ToString(),
					options: jsonSerializerOptions))
			.Select(document =>
				new {
					document.DocumentId,
					document.Preview,
					document.DateNumerisation,
					document.FichierNom,
					document.Extension
				})
			.OrderBy(document => document.DocumentId);
}

readonly Random dice = new();
string GetRandomWord() {
	return new string(new int[10].Select(_ => (char)(dice.Next(26) + (int)'A')).ToArray());
}
