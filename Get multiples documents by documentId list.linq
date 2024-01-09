<Query Kind="Statements">
  <Reference>C:\TeamProjects\GED API\MAF.GED.API.Host\bin\Debug\net6.0\MAF.GED.Domain.Model.dll</Reference>
  <Namespace>System.Data.SqlClient</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Nodes</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeLinqToSql>true</IncludeLinqToSql>
</Query>

var httpClient = new HttpClient { BaseAddress = new Uri("https://api-ged-intra.int.maf.local/v2/Documents/") };
var documentsIdList =
	string.Join(',', File.ReadAllLines(@"C:\Users\deschaseauxr\Documents\Document entrant - remplacer GED WS par web api GED\documents id list.txt").Select(documentId => $"'{documentId}'"));
var actual_documents = await httpClient.GetStringAsync($"?$filter=documentId in ({documentsIdList})");
var jsonSerializerOptions = GetJsonSerializerOptions();
JsonDocument
	.Parse(actual_documents)
	.RootElement.GetProperty("value")
	.EnumerateArray()
	.Select(document =>
		JsonSerializer.Deserialize<MAF.GED.Domain.Model.Document>(
			json: document.ToString(),
			options: jsonSerializerOptions))
	.Select(document =>
		new {
			document.AssigneRedacteur,
			Famille = document.CategoriesFamille,
			Cote = document.CategoriesCote,
			TypeDoc = document.CategoriesTypeDocument,
			document.CompteId,
			document.DateDocument,
			document.DocumentId,
			document.FichierNom,
			document.Libelle,
			document.PersonneId,
		})
	.OrderBy(document => document.DocumentId)
	.Dump();

JsonSerializerOptions GetJsonSerializerOptions() {
	var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
	jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
	return jsonSerializerOptions;
}

/*
AssigneDepartement
AssigneGroup
AssigneRedacteur
AssureurId
CanalPrincipal
CanalSecondaire
CategoriesCote
CategoriesFamille
CategoriesTypeDocument
ChantierId
CodeBarreId
CodeOrigine
Commentaire
CompteId
DateDocument
DateNumerisation
DeposeLe
DeposePar
Docn
DocumentId
DocumentId
DocumentValide
DuplicationId
Extension
FichierNom
FichierNombrePages
FichierTaille
HeureNumerisation
Horodatage
Important
IsHorsWorkFlowSinapps
Libelle
Link
ModifieLe
ModifiePar
MultiCompteId
Nature
NumeroAvenant
NumeroContrat
NumeroGc
NumeroProposition
NumeroSinistre
PeriodeValiditeDebut
PeriodeValiditeFin
PersonneId
PresenceAr
Preview
PreviewLink
Priorite
Provenance
QualiteValideeLe
QualiteValideePar
QualiteValideeValide
ReferenceAttestation
ReferenceSecondaire
RefTiers
RegroupementId
Sens
SousDossierSinistre
Statut
Tenant
TraiteLe
TraitePar
TypeContact
TypeGarantie
VisibiliteExterne
VisibilitePapsExtranet
VuLe
VuPar
*/
