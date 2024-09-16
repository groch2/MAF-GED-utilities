<Query Kind="Statements">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Nodes</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeUncapsulator>false</IncludeUncapsulator>
</Query>

const string gedWebApiBaseAddress = "https://api-ged-intra.int.maf.local";
//const string gedWebApiBaseAddress = "http://localhost:44363";
var httpClient = new HttpClient {
	BaseAddress = new Uri($"{gedWebApiBaseAddress}/v2/Documents/")
};
const string documentId = "20240807143132004322207355";
var documentJson = await httpClient.GetStringAsync(documentId);
var wantedProperties = new HashSet<string>(new string[] {
	"AssigneDepartement",
	"AssigneGroup",
	"AssigneRedacteur",
	"AssureurId",
	"CanalPrincipal",
	"CanalSecondaire",
	"CategoriesCote",
	"CategoriesFamille",
	"CategoriesTypeDocument",
	"ChantierId",
	"CodeBarreId",
	"CodeOrigine",
	"Commentaire",
	"CompteId",
	"DateDocument",
	"DateNumerisation",
	"DeposeLe",
	"DeposePar",
	"Docn",
	"DocumentId",
	"DocumentValide",
	"DuplicationId",
	"Extension",
	"FichierNom",
	"FichierNombrePages",
	"FichierTaille",
	"HeureNumerisation",
	"Horodatage",
	"Important",
	"IsHorsWorkFlowSinapps",
	"Libelle",
	"Link",
	"ModifieLe",
	"ModifiePar",
	"MultiCompteId",
	"Nature",
	"NumeroAvenant",
	"NumeroContrat",
	"NumeroGc",
	"NumeroProposition",
	"NumeroSinistre",
	"PeriodeValiditeDebut",
	"PeriodeValiditeFin",
	"PersonneId",
	"PresenceAr",
	"Preview",
	"PreviewLink",
	"Priorite",
	"Provenance",
	"QualiteValideeLe",
	"QualiteValideePar",
	"QualiteValideeValide",
	"ReferenceAttestation",
	"ReferenceSecondaire",
	"RefTiers",
	"RegroupementId",
	"Sens",
	"SousDossierSinistre",
	"Statut",
	"Tenant",
	"TraiteLe",
	"TraitePar",
	"TypeContact",
	"TypeGarantie",
	"VisibiliteExterne",
	"VisibilitePapsExtranet",
	"VuLe",
	"VuPar",
	}, StringComparer.OrdinalIgnoreCase);
SortFirstLevelPropertiesAlphabetically(documentJson, wantedProperties).Dump();

static JsonObject SortFirstLevelPropertiesAlphabetically(string jsonString, HashSet<string> wantedProperties) =>
	new JsonObject(
	    JsonDocument
			.Parse(jsonString)
			.RootElement
			.EnumerateObject()
			.Where(prop =>
				!prop.Name.StartsWith("@odata.", StringComparison.OrdinalIgnoreCase) &&
				wantedProperties.Contains(prop.Name))
			.OrderBy(
				jsonProperty => jsonProperty.Name,
				StringComparer.InvariantCultureIgnoreCase)
			.Select(
				jsonProperty =>
					new KeyValuePair<string, JsonNode>(
						key: jsonProperty.Name,
						value: JsonNode.Parse(jsonProperty.Value.GetRawText()))));

/* LISTE DE TOUTES LES PROPRIÉTÉS DES DOCUMENTS TRIÉES PAR ORDRE ALPHABÉTIQUE
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