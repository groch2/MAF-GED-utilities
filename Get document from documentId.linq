<Query Kind="Statements">
  <Reference>C:\TeamProjects\GED API\MAF.GED.API.Host\bin\Debug\net6.0\MAF.GED.Domain.Model.dll</Reference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeLinqToSql>true</IncludeLinqToSql>
</Query>

var httpClient = new HttpClient {
	BaseAddress = new Uri("https://api-ged-intra.int.maf.local/v2/Documents/")
};
const string documentId = "20240108162602511400805243";
var raw_document =
	await httpClient.GetStringAsync(documentId);
var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
var document =
	JsonSerializer.Deserialize<MAF.GED.Domain.Model.Document>(raw_document, jsonSerializerOptions);
new {
	document.DocumentId,
	document.Libelle,
	document.CompteId,
	document.PersonneId
}.Dump();
