<Query Kind="Statements">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Nodes</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeUncapsulator>false</IncludeUncapsulator>
</Query>

const string gedWebApiBaseAddress = "https://api-ged-intra.int.maf.local";
var httpClient = new HttpClient {
	BaseAddress = new Uri(gedWebApiBaseAddress)
};
var responseContent = await httpClient.GetStringAsync("/v2/Documents/?$filter=deposePar eq 'ROD'&$orderBy=deposeLe desc&$select=documentId,libelle&$top=10");
JsonDocument
	.Parse(responseContent)
	.RootElement
	.GetProperty("value")
	.EnumerateArray()
	.Select(
		node =>
			new {
				DocumentId = node.GetProperty("documentId").GetString(),
				Libelle = node.GetProperty("libelle").GetString()
			}
	).Dump();

