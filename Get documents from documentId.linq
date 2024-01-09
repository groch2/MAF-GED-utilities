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

var httpClient = new HttpClient {
	BaseAddress = new Uri("https://api-ged-intra.int.maf.local/v2/Documents/")
};
const string documentId = "20230828175135721281802470";
var raw_document =
	await httpClient.GetStringAsync(documentId);
var document =
	Newtonsoft.Json.JsonConvert.DeserializeObject<MAF.GED.Domain.Model.Document>(raw_document);
new {
	document.DocumentId,
	document.Libelle,
	document.CompteId,
	document.PersonneId
}.Dump();
