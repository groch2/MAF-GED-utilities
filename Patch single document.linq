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

const string ENVIRONMENT_CODE = "int";
async Task Main()
{
	const string documentId = "20240130102212266277270137";
	var libellePatchValue = Guid.NewGuid().ToString("N").ToUpperInvariant();
	var documentPatch =
		JsonSerializer.SerializeToNode(
			new {
				ModifiePar = "ROD",
				Libelle = libellePatchValue
			});
	var patchDocumentResponse =
		await PatchDocument(
			documentId: documentId,
			documentPatch: documentPatch);
	new {
		patchDocumentResponse.StatusCode,
		patchDocumentResponse.ReasonPhrase
	}.Dump();
	patchDocumentResponse.EnsureSuccessStatusCode();	
	
	var document = await GetDocumentById(documentId);
	var actualDocumentLibelle = document.Libelle;
	var isExpectedLibelle = actualDocumentLibelle == libellePatchValue;
	new {
		isExpectedLibelle,
		newLibelle = document.Libelle,
		expectedLibelle = libellePatchValue
	}.Dump();
	System.Diagnostics.Debug.Assert(isExpectedLibelle);
}

HttpClient httpClient =
	new HttpClient {
		BaseAddress = new Uri($"https://api-ged-intra.{ENVIRONMENT_CODE}.maf.local/v2/Documents/") };

async Task<HttpResponseMessage> PatchDocument(string documentId, JsonNode documentPatch) {
	//documentPatch["@odata.type"] = "MAF.GED.Domain.Model.Document";
	var requestContent = JsonContent.Create(documentPatch);
	return await httpClient.PatchAsync(documentId, requestContent);
}

JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
async Task<MAF.GED.Domain.Model.Document> GetDocumentById(string documentId) =>
	await httpClient.GetFromJsonAsync<MAF.GED.Domain.Model.Document>(documentId, jsonSerializerOptions);
	
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
