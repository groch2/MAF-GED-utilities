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

const string environmentCode = "rec";
var httpClient = new HttpClient { BaseAddress = new Uri($"https://api-ged-intra.{environmentCode}.maf.local/v2/Documents/") };
const string documentsIdListFile =
	@"C:\Users\deschaseauxr\Documents\GED\batch de génération de preview\surcharge de la RAM - plantage de OPCON\documents de test upload\documents id in ged api.txt";
var documentsIdList =
	string.Join(',', File.ReadAllLines(documentsIdListFile).Select(documentId => $"'{documentId}'"));
var json_documents = await httpClient.GetStringAsync($"?$filter=documentId in ({documentsIdList})");
var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
JsonDocument
	.Parse(json_documents)
	.RootElement.GetProperty("value")
	.EnumerateArray()
	.Select(document =>
		JsonSerializer.Deserialize<MAF.GED.Domain.Model.Document>(
			json: document.ToString(),
			options: jsonSerializerOptions))
	.Select(document =>
		new {
			document.DocumentId,
			document.Preview,
			document.DateNumerisation,
			document.FichierNom,
			document.Extension
		})
	.OrderBy(document => document.DocumentId)
	.Dump();

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
