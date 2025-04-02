<Query Kind="Program">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Nodes</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeLinqToSql>true</IncludeLinqToSql>
</Query>

async Task Main() {
	const string documentsIdListFile = @"C:\Users\deschaseauxr\Documents\GED\documentId list.txt";
	var documentsIdList = File.ReadAllLines(documentsIdListFile);
	var documentsExistence = await CheckDocumentsByDocumentsIdList(documentsIdList);
	documentsExistence.Dump();
}

const string ENVIRONMENT_CODE = "j1d";
static readonly HttpClient mafGedHttpClient = new HttpClient { BaseAddress = new Uri($"https://api-ged-intra.{ENVIRONMENT_CODE}.maf.local/v2/Documents/") };
static async Task<Dictionary<string, bool>> CheckDocumentsByDocumentsIdList(IEnumerable<string> documentsIdList) {
	var documentsData =
		await Task.WhenAll(
	        documentsIdList
		        .Select(async documentId => {
					var request =
						new HttpRequestMessage {
							RequestUri = new Uri(documentId, UriKind.Relative),
							Method = HttpMethod.Head
						};
		            var responseMessage = await mafGedHttpClient.SendAsync(request);
					var responseContent = responseMessage.StatusCode;
					return new { DocumentId = documentId, DocumentExists = responseMessage.IsSuccessStatusCode };
				}));
	var documentsExistence =
		documentsData.ToDictionary(
			item => item.DocumentId,
			item => item.DocumentExists);
	return documentsExistence;
}
