<Query Kind="Statements">
  <Reference>C:\TeamProjects\GED_API\MAF.GED.API.Host\bin\Debug\net8.0\MAF.GED.Domain.Model.dll</Reference>
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
var libelles = string.Join(',', File.ReadAllLines(@"C:\Users\deschaseauxr\AppData\Local\Temp\new 2.txt").Select(libelle => $"'{libelle}'"));
var actual_documents = await httpClient.GetStringAsync($"?$filter=libelle in ({libelles})");
var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
JsonDocument
	.Parse(actual_documents)
	.RootElement.GetProperty("value")
	.EnumerateArray()
	.Select(document => JsonSerializer.Deserialize<MAF.GED.Domain.Model.Document>(document, jsonSerializerOptions))
	.Select(document =>
		new {
			document.AssigneRedacteur,
			document.CategoriesCote,
			document.CategoriesFamille,
			document.CategoriesTypeDocument,
			document.CompteId,
			document.DateDocument,
			document.DocumentId,
			document.Extension,
			document.FichierNom,
			document.Libelle,
			document.PersonneId,
			document.DateNumerisation,
			document.DeposeLe,
			document.DeposePar,
			document.VuLe,
			document.VuPar,
			document.QualiteValideeLe,
			document.QualiteValideePar,
			document.QualiteValideeValide,			
			document.PeriodeValiditeDebut,
			document.PeriodeValiditeFin,
			document.TypeGarantie,
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
