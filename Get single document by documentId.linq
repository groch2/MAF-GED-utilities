<Query Kind="Statements">
  <Namespace>System.Data.SqlClient</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Nodes</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeLinqToSql>true</IncludeLinqToSql>
</Query>

const string ENVIRONMENT_CODE = "int";
var httpClient = new HttpClient { BaseAddress = new Uri($"https://api-ged-intra.{ENVIRONMENT_CODE}.maf.local/v2/Documents/") };
const string documentId = "20250207114735834732358072";
var queryString = $"?$filter=documentId eq '{documentId}'&$select=assigneRedacteur,assureurId,canalPrincipal,categoriesCote,categoriesFamille,categoriesTypeDocument,chantierId,codeOrigine,commentaire,compteId,dateDocument,dateNumerisation,deposeLe,deposePar,docn,documentId,extension,fichierNom,fichierNombrePages,fichierTaille,heureNumerisation,horodatage,important,libelle,modifieLe,modifiePar,numeroGc,numeroSinistre,periodeValiditeDebut,periodeValiditeFin,personneId,presenceAr,previewLink,qualiteValideeLe,qualiteValideePar,qualiteValideeValide,regroupementId,sens,statut,traiteLe,traitePar,typeGarantie,vuLe,vuPar";
var json_document = await httpClient.GetStringAsync(queryString);
var document =
	JsonDocument
		.Parse(json_document)
		.RootElement.GetProperty("value")
		.EnumerateArray()
		.Select(jsonElement => JsonNode.Parse(jsonElement.ToString()))
		.Select(jsonNode =>
			new {
				AssigneRedacteur = jsonNode["assigneRedacteur"]?.ToString(),
				DocumentId = jsonNode["documentId"]?.ToString(),
				Libelle = jsonNode["libelle"]?.ToString(),
				//Commentaire = jsonNode["commentaire"]?.ToString(),
				//TypeGarantie = jsonNode["typeGarantie"]?.ToString(),
				//FichierNom = jsonNode["fichierNom"]?.ToString(),
				Famille = jsonNode["categoriesFamille"]?.ToString(),
				Côte = jsonNode["categoriesCote"]?.ToString(),
				TypeDocument = jsonNode["categoriesTypeDocument"]?.ToString(),
				//DeposeLe = GetDateOnly(jsonNode["deposeLe"]?.GetValue<DateTime>()),
				//DeposePar = jsonNode["deposePar"]?.ToString(),
				//VuLe = GetDateOnly(jsonNode["vuLe"]?.GetValue<DateTime>()),
				//VuPar = jsonNode["vuPar"]?.ToString(),
				//QualiteValideeLe = GetDateOnly(jsonNode["qualiteValideeLe"]?.GetValue<DateTime>()),
				//QualiteValideePar = jsonNode["qualiteValideePar"]?.ToString(),
				//QualiteValideeValide = jsonNode["qualiteValideeValide"]?.ToString(),
				//TraiteLe = GetDateOnly(jsonNode["traiteLe"]?.GetValue<DateTime>()),
				//TraitePar = jsonNode["traitePar"]?.ToString(),
				//ModifieLe = GetDateOnly(jsonNode["modifieLe"]?.GetValue<DateTime>()),
				//ModifiePar = jsonNode["modifiePar"]?.ToString(),
				//NumeroContrat = jsonNode["numeroContrat"]?.ToString(),
				NumeroSinistre = jsonNode["numeroSinistre"]?.ToString(),
				//ChantierId = jsonNode["chantierId"]?.ToString(),
				//AssureurId = jsonNode["assureurId"]?.ToString(),
				CompteId = jsonNode["compteId"]?.ToString(),
				PersonneId = jsonNode["personneId"]?.ToString(),
				//Sens = jsonNode["sens"]?.ToString(),
				//Important = jsonNode["important"]?.ToString(),
				//PeriodeValiditeDebut = GetDateOnly(jsonNode["periodeValiditeDebut"]?.GetValue<DateTime>()),
				//PeriodeValiditeFin = GetDateOnly(jsonNode["periodeValiditeFin"]?.GetValue<DateTime>()),
				//Statut = jsonNode["statut"]?.ToString()
			});
document.Dump();

/*
assigneDepartement
assigneGroup
assigneRedacteur
assureurId
canalPrincipal
canalSecondaire
categoriesCote
categoriesFamille
categoriesTypeDocument
chantierId
codeBarreId
codeOrigine
commentaire
compteId
dateDocument
dateNumerisation
deposeLe
deposePar
docn
documentId
documentValide
duplicationId
extension
fichierNom
fichierNombrePages
fichierTaille
heureNumerisation
horodatage
important
isHorsWorkFlowSinapps
libelle
link
modifieLe
modifiePar
multiCompteId
nature
numeroAvenant
numeroContrat
numeroGc
numeroProposition
numeroSinistre
periodeValiditeDebut
periodeValiditeFin
personneId
presenceAr
preview
previewLink
priorite
provenance
qualiteValideeLe
qualiteValideePar
qualiteValideeValide
referenceAttestation
referenceSecondaire
refTiers
regroupementId
sens
sousDossierSinistre
statut
tenant
traiteLe
traitePar
typeContact
typeGarantie
visibiliteExterne
visibilitePapsExtranet
vuLe
vuPar
*/
