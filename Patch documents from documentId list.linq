<Query Kind="Statements">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Json</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Nodes</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeLinqToSql>true</IncludeLinqToSql>
</Query>

var httpClient =
	new HttpClient {
		BaseAddress = new Uri("https://api-ged-intra.int.maf.local/v2/Documents/")
	};
var documentPatch =
	JsonSerializer.SerializeToNode(
		new {
			CategoriesFamille = "DOCUMENTS ENTRANTS",
			CategoriesCote = "AUTRES",
			CategoriesTypeDocument = "DIVERS",
			ModifiePar = "toto"
		});
var patchDocumentsResponses =
	await Task.WhenAll(
		File
			.ReadAllLines(@"C:\Users\deschaseauxr\Documents\GED\documentId list.txt")
			.Select(async documentId => {
				var patchDocumentResponse = await PatchSingleDocument(documentId, documentPatch);
				patchDocumentResponse.EnsureSuccessStatusCode();
				return patchDocumentResponse;
			}));
patchDocumentsResponses
	.Select(
		patchDocumentResponse =>
			new {
				documentId = patchDocumentResponse.RequestMessage.RequestUri.Segments[^1],
				patchDocumentResponse.ReasonPhrase,
				patchDocumentResponse.StatusCode
			})
	.Dump();

async Task<HttpResponseMessage> PatchSingleDocument(
	string documentId, 
	JsonNode documentPatch) {
		var requestContent = JsonContent.Create(documentPatch);
		return await httpClient.PatchAsync(documentId, requestContent);
}