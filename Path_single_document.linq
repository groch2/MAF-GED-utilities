<Query Kind="Program">
  <Reference>C:\TeamProjects\GED API\MAF.GED.API.Host\bin\Debug\net6.0\MAF.GED.Domain.Model.dll</Reference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Json</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Nodes</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeLinqToSql>true</IncludeLinqToSql>
</Query>

async Task Main()
{
	const string documentId = "20240124092341136631582011";
	var libellePatchValue = Guid.NewGuid().ToString("N").ToUpperInvariant();
	var patchContent =
		JsonSerializer.SerializeToNode(
			new {
				ModifiePar = "ROD",
				Libelle = libellePatchValue
			});
	var patchResponse = await PatchDocument(documentId, patchContent);
	new { patchResponse.StatusCode, patchResponse.ReasonPhrase }.Dump();
	patchResponse.EnsureSuccessStatusCode();	
	
	var document = await GetDocumentById(documentId);
	var actualDocumentLibelle = document.Libelle;
	var isExpectedLibelle = actualDocumentLibelle == libellePatchValue;
	System.Diagnostics.Debug.Assert(isExpectedLibelle);
	new {
		isExpectedLibelle,
		newLibelle = document.Libelle,
		expectedLibelle = libellePatchValue
	}.Dump();
}

HttpClient httpClient =
	new HttpClient {
		BaseAddress = new Uri("http://localhost:44363/v2/Documents/") };

async Task<HttpResponseMessage> PatchDocument(string documentId, JsonNode patchContent) {
	patchContent["@odata.type"] = "MAF.GED.Domain.Model.Document";
	var requestContent = JsonContent.Create(patchContent);
	return await httpClient.PatchAsync(documentId, requestContent);
}

JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
async Task<MAF.GED.Domain.Model.Document> GetDocumentById(string documentId) {
	var documentJson = await httpClient.GetStringAsync(documentId);
	var document = JsonSerializer.Deserialize<MAF.GED.Domain.Model.Document>(documentJson, jsonSerializerOptions);
	return document;
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
