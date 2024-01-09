<Query Kind="Statements">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

var httpClient = new HttpClient { BaseAddress = new Uri("https://api-ged-intra.int.maf.local/v2/") };
var rawJsonFamilles = await httpClient.GetStringAsync("Familles?$filter=isActif eq true&$expand=cotes($expand=typesDocuments;$filter=isActif eq true)");
var familles =
	JsonDocument
		.Parse(rawJsonFamilles)
		.RootElement
		.GetProperty("value")
		.EnumerateArray()
		.Select(jsonFamille => JsonSerializer.Deserialize<Famille>(jsonFamille, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }))
		.SelectMany(famille => {
			return famille.Cotes.SelectMany(cote => {
				return cote.TypesDocuments.Select(
					typeDocument =>
						new {
							FamilleId = famille.FamilleDocumentId,
							FamilleCode = famille.Code,
							FamilleLibelle = famille.Libelle,						

							CoteId = cote.CoteDocumentId,
							CoteCode = cote.Code,
							CoteLibelle = cote.Libelle,

							TypeDocumentId = typeDocument.TypeDocumentId,
							TypeDocumentCode = typeDocument.Code,
							TypeDocumentLibelle = typeDocument.Libelle,
						});
				});
		});
familles.Dump();
// isActif
record Famille(int FamilleDocumentId, string Code, string Libelle, Cote[] Cotes);
record Cote(int CoteDocumentId,string Code, string Libelle, TypeDocument[] TypesDocuments);
record TypeDocument(int TypeDocumentId, string Code, string Libelle);
