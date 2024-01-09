<Query Kind="Program">
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
	var documentPatch =
		JsonSerializer.SerializeToNode(
			new {
				Libelle = "coucou",
			});
	var patchDocumentResponse =
		await UpdateDocument(
			documentId: "20240108171918478337442374",
			documentPatch: documentPatch);
	patchDocumentResponse.EnsureSuccessStatusCode();
	new { 
		patchDocumentResponse.ReasonPhrase,
		patchDocumentResponse.StatusCode
	}.Dump();
}

HttpClient httpClient = new HttpClient { BaseAddress = new Uri("https://api-ged-intra.int.maf.local/v2/Documents/") };
async Task<HttpResponseMessage> UpdateDocument(string documentId, JsonNode documentPatch) {
	var requestContent = JsonContent.Create(documentPatch);
	return await httpClient.PatchAsync(documentId, requestContent);
}

readonly Random dice = new();
string GetRandomWord() => new string(new int[10].Select(_ => (char)(dice.Next(26) + (int)'A')).ToArray());

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