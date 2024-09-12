<Query Kind="Program">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Json</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Nodes</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeLinqToSql>true</IncludeLinqToSql>
</Query>

const string webApiGedAddress = "https://api-ged-intra.int.maf.local/v2/Documents/";
async Task Main()
{
	const string documentId = "20240306143954243038283375";
	var documentPatch =
System.Text.Json.JsonSerializer.SerializeToNode(System.Text.Json.JsonDocument.Parse(@"{""preview"": true,
""modifiePar"": ""ROD""}"));
	var patchDocumentResponse =
		await PatchDocument(
			documentId: documentId,
			documentPatch: documentPatch);
	new {
		patchDocumentResponse.StatusCode,
		patchDocumentResponse.ReasonPhrase
	}.Dump();
	patchDocumentResponse.EnsureSuccessStatusCode();	
}

HttpClient httpClient =
	new HttpClient {
		BaseAddress = new Uri(webApiGedAddress) };

async Task<HttpResponseMessage> PatchDocument(string documentId, JsonNode documentPatch) {
	var requestContent = JsonContent.Create(documentPatch);
	return await httpClient.PatchAsync(documentId, requestContent);
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
