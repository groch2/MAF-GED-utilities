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
	const string filePath = @"C:\Users\deschaseauxr\Documents\MAFlyDoc\test.pdf";
	var uploadDocuments =
		Enumerable
			.Range(0, 3)
			.Select(async index => {
				var documentId = await UploadDocument(filePath, index + 1);
				return documentId;
			});
	await Task.WhenAll(uploadDocuments);
	uploadDocuments.Select(uploadDocument => uploadDocument.Result).Dump();
}

HttpClient httpClient = new HttpClient { BaseAddress = new Uri("https://api-ged-intra.int.maf.local/") };
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

readonly Random dice = new();
string GetRandomWord() {
	return new string(new int[10].Select(_ => (char)(dice.Next(26) + (int)'A')).ToArray());
}
