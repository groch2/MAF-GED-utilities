<Query Kind="Statements">
  <Reference>C:\TeamProjects\GED API\MAF.GED.API.Host\bin\Debug\net6.0\MAF.GED.Domain.Model.dll</Reference>
  <Reference>C:\TeamProjects\MAFlyDoc\MAFlyDoc\MAFlyDoc.WebApi\bin\Debug\net6.0\Newtonsoft.Json.dll</Reference>
  <Namespace>System.Data.SqlClient</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeLinqToSql>true</IncludeLinqToSql>
</Query>

var httpClient =
	new HttpClient {
		BaseAddress = new Uri("https://api-ged-intra.int.maf.local/v2/Documents/")
	};
var raw_responses =
	await httpClient.GetStringAsync(
		$"?$filter=categoriesFamille eq 'Documents entrants' and statut eq 'INDEXE'&$count=true&$top=0");
var documents =
	JsonDocument
		.Parse(raw_responses)
		.RootElement.GetProperty("value")
		.EnumerateArray()
		.Select(document => Newtonsoft.Json.JsonConvert.DeserializeObject<MAF.GED.Domain.Model.Document>($"{document}"))
		.Select(document =>
			new {
				document.DocumentId,
				document.Libelle,
				document.Statut,
				document.ModifiePar,
				document.CategoriesFamille,
				document.CategoriesCote,
				document.CategoriesTypeDocument
			});
documents.Dump();
