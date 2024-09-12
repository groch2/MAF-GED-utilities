<Query Kind="Statements">
  <Namespace>System.Data.SqlClient</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Nodes</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeLinqToSql>true</IncludeLinqToSql>
</Query>

var httpClient = new HttpClient { BaseAddress = new Uri("https://api-ged-intra.int.maf.local/v2/Documents/") };
const string documentsIdListFile =
	@"C:\Users\deschaseauxr\Documents\Document entrant - remplacer GED WS par web api GED\documents id list.txt";
var documentsIdList = File.ReadAllLines(documentsIdListFile);
var missingDocumentsIdList =
	await GetMissingDocumentsIdList(documentsIdList);
new { missingDocumentsIdList }.Dump();

async Task<IEnumerable<string>> GetMissingDocumentsIdList(IEnumerable<string> documentsIdList) {
	var formattedDocumentsIdList =
		string.Join(',', File.ReadAllLines(documentsIdListFile).Select(documentId => $"'{documentId}'"));
	var jsonGEDdocuments = await httpClient.GetStringAsync($"?$filter=documentId in ({formattedDocumentsIdList})&$select=documentId&$orderby=documentId");
	var existingDocumentIdList =
		JsonDocument
			.Parse(jsonGEDdocuments)
			.RootElement.GetProperty("value")
			.EnumerateArray()
			.Select(document => document.GetProperty("documentId").GetString());
	var missingDocuments =
		documentsIdList.Except(existingDocumentIdList, StringComparer.OrdinalIgnoreCase);
	return missingDocuments;
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
