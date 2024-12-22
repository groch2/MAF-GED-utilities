<Query Kind="Program">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Json</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Nodes</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeLinqToSql>true</IncludeLinqToSql>
</Query>

const string environment = "int";
async Task Main() {
	var patchDocumentsResponses =
		await Task.WhenAll(
			File
				.ReadAllLines(@"C:\Users\deschaseauxr\Documents\GED\documentId list.txt")
				.Select(async (documentId, index) => {
					var libelle = get_random_word(10);
					var documentPatch =
						JsonSerializer.SerializeToNode(
							new {
								ModifiePar = "ROD",
								Libelle = $"{libelle} {index + 1}"
							});			
					var patchDocumentResponse =
						await PatchSingleDocument(documentId, documentPatch);
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
}

static readonly Random dice = new Random();
static string get_random_word(int length) =>
	new string(new int[length].Select(_ => (char)(dice.Next(26) + (int)'A')).ToArray());
static async Task<HttpResponseMessage> PatchSingleDocument(
	string documentId, 
	JsonNode documentPatch) {
		using var requestContent = JsonContent.Create(documentPatch);
		return await httpClient.PatchAsync(documentId, requestContent);
}
static readonly HttpClient httpClient =
	new HttpClient {
		BaseAddress = new Uri($"https://api-ged-intra.{environment}.maf.local/v2/Documents/")
	};
