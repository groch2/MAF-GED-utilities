<Query Kind="Program">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeLinqToSql>true</IncludeLinqToSql>
</Query>

async Task Main()
{
	var allMafUsers = (await GetAllMafUsers()).ToArray();
	Shuffle(allMafUsers);
	var patchDocuments =
		File
			.ReadAllLines(@"C:\Users\deschaseauxr\AppData\Local\Temp\documents_to_patch.txt")
			.Select(async (documentId, index) =>
				new {
					documentId,
					response =
						await UpdateDocument(
							documentId: documentId, 
							patch: new {
								assigneRedacteur =
									index % 2 == 0 ?
									allMafUsers[index].codeUtilisateur :
									null }
						)
				});
	await Task.WhenAll(patchDocuments);
	var patchDocumentSuccess =
		patchDocuments
			.Select(
				patchDocument => new {
					documentId = patchDocument.Result.documentId,
					isSuccessStatusCode = patchDocument.Result.response.IsSuccessStatusCode
			});
	new { patchDocumentSuccess }.Dump();	
}

readonly HttpClient gedHttpClient = new HttpClient { BaseAddress = new Uri("https://api-ged-intra.int.maf.local/v1/Documents/") };
async Task<HttpResponseMessage> UpdateDocument(string documentId, object patch) {
	var serializedPatch = JsonSerializer.Serialize(patch);
	var requestContent = new StringContent(serializedPatch, Encoding.UTF8, "application/json");
	return await gedHttpClient.PatchAsync(documentId, requestContent);
}

readonly HttpClient butHttpClient = new HttpClient { BaseAddress = new Uri("https://api-but-intra.int.maf.local/api/v2/Utilisateurs/") };
async Task<IEnumerable<Utilisateur>> GetAllMafUsers()
{
	var rawUsers = await butHttpClient.GetStringAsync("?includeNonActif=false");
	return
		JsonDocument
			.Parse(rawUsers)
			.RootElement
			.EnumerateArray()
			.Select(rawUser => 
				JsonSerializer.Deserialize<Utilisateur>($"{rawUser}"));
}
record Utilisateur(string codeUtilisateur, string nom, string prenom);

Random random = new Random();
void Shuffle<T>(T[] items) {
	for (var i = 0; i < items.Length; i++) {
		var temp = items[i];
		items[i] = temp;
		var randomIndex = random.Next(i, items.Length);
		items[i] = items[randomIndex];
		items[randomIndex] = temp;
	}
}
