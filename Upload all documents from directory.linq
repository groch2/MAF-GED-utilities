<Query Kind="Statements">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
  <Namespace>System.Net.Http.Json</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Nodes</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

const string ENVIRONMENT_CODE = "int";
const string apiVersion = "v2";
Directory
	.GetFiles(@"C:\Users\deschaseauxr\Documents\GED\Document de test")
	.ToList()
	.ForEach(async filePath => {
		var originalWordDocumentFileName = Path.GetFileName(filePath);

		// client http GED MAF
		// https://api-ged-intra.int.maf.local/
		var client = new HttpClient { BaseAddress = new Uri($"https://api-ged-intra.{ENVIRONMENT_CODE}.maf.local/") };

		// upload document
		"upload document début".Dump();
		await using var stream = File.OpenRead(filePath);
		using var requestUpload =
			new HttpRequestMessage(HttpMethod.Post, new Uri($"/{apiVersion}/upload", UriKind.Relative));
		using var content = new MultipartFormDataContent { { new StreamContent(stream), "file", originalWordDocumentFileName } };
		requestUpload.Content = content;
		var httpResponseUpload = await client.SendAsync(requestUpload);
		var uploadResponseContent = await httpResponseUpload.Content.ReadAsStringAsync();
		var uploadId = JsonNode.Parse(uploadResponseContent)["guidFile"].GetValue<string>();
		"upload document fin".Dump();

		// finalize upload
		"finalize upload document début".Dump();
		var documentUpload = new {
		  fileId = uploadId,
		  libelle = originalWordDocumentFileName,
		  deposePar = "ROD",
		  dateDocument = DateTime.Now.ToUniversalTime(),
		  fichierNom = originalWordDocumentFileName,
		  fichierTaille = new FileInfo(filePath).Length,
		  categoriesFamille = "DOCUMENTS CONTRAT",
		  categoriesCote = "AUTRES",
		  categoriesTypeDocument = "DIVERS",
		  canalId = 1,
		};
		var finalizeDocumentUploadJson = JsonContent.Create(documentUpload);
		var finalizeUploadAddress = new Uri($"/{apiVersion}/finalizeUpload", UriKind.Relative);
		var finalizeUploadResponse =
			await client.PostAsync(finalizeUploadAddress, finalizeDocumentUploadJson);
		var finalizeUploadResponseContent =
			await finalizeUploadResponse.Content.ReadAsStringAsync();
		var documentId = JsonNode.Parse(finalizeUploadResponseContent)["documentId"].GetValue<string>();
		new { documentId }.Dump();
		"finalize upload document fin".Dump();
	});