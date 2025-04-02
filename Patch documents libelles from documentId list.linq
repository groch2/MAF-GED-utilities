<Query Kind="Statements">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Json</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Nodes</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeLinqToSql>true</IncludeLinqToSql>
</Query>

const string ENVIRONMENT_CODE = "int";
var dice = new Random();
var httpClient =
	new HttpClient {
		BaseAddress = new Uri($"https://api-ged-intra.{ENVIRONMENT_CODE}.maf.local/v2/Documents/")
	};
var patchDocumentsResponses =
	await Task.WhenAll(
		File
			.ReadAllLines(@"C:\Users\deschaseauxr\Documents\GED\documentId list.txt")
			.Select(async documentId => {
				var random_text = get_random_text(nb_words: 20).ToUpperInvariant();
				var documentPatch =
					JsonSerializer.SerializeToNode(
						new {
							ModifiePar = "ROD",
							Libelle = random_text,
						});
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

string get_random_text(int nb_words) =>
	string.Join(' ', Enumerable.Range(0, nb_words).Select(_ => get_random_word(dice.Next(5, 15))));
string get_random_word(int length) =>
	new string(new int[length].Select(_ => (char)(dice.Next(26) + (int)'a')).ToArray());
