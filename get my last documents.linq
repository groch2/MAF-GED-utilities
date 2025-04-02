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
var responseContent = await httpClient.GetStringAsync("/v2/Documents/?$filter=deposePar eq 'ROD'&$orderBy=deposeLe desc&$select=documentId,libelle,deposeLe,categoriesFamille,categoriesCote,categoriesTypeDocument&$top=10");
JsonDocument
	.Parse(responseContent)
	.RootElement
	.GetProperty("value")
	.EnumerateArray()
	.Select(
		node =>
			new {
				DocumentId = node.GetProperty("documentId").GetString(),
				Libelle = node.GetProperty("libelle").GetString(),
				DeposeLe = GetDateOnly(node.GetProperty("deposeLe").GetDateTime()),
				Famille = node.GetProperty("categoriesFamille").GetString(),
				Côte = node.GetProperty("categoriesCote").GetString(),
				TypeDocument = node.GetProperty("categoriesTypeDocument").GetString(),
			}
	).Dump();

static DateOnly? GetDateOnly(DateTime? date) =>
	date.HasValue ? DateOnly.FromDateTime(date.Value): (DateOnly?)null;
