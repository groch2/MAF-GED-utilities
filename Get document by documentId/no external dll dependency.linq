<Query Kind="Statements">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Nodes</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeUncapsulator>false</IncludeUncapsulator>
</Query>

var httpClient = new HttpClient {
	BaseAddress = new Uri("https://api-ged-intra.int.maf.local/v2/Documents/")
};
const string documentId = "20240125184202063860128348";
var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
var documentJson = await httpClient.GetStringAsync(documentId);
SortFirstLevelPropertiesAlphabetically(documentJson).Dump();

static JsonObject SortFirstLevelPropertiesAlphabetically(string jsonString) =>
	new JsonObject(
	    JsonDocument
			.Parse(jsonString)
			.RootElement
			.EnumerateObject()
			.Where(prop => !prop.Name.StartsWith("@odata.", StringComparison.OrdinalIgnoreCase))
			.OrderBy(
				jsonProperty => jsonProperty.Name,
				StringComparer.InvariantCultureIgnoreCase)
			.Select(
				jsonProperty =>
					new KeyValuePair<string, JsonNode>(
						key: jsonProperty.Name,
						value: JsonNode.Parse(jsonProperty.Value.GetRawText()))));

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