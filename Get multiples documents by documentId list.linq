<Query Kind="Program">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Nodes</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeLinqToSql>true</IncludeLinqToSql>
</Query>

async Task Main() {
	const string documentsIdListFile = @"C:\Users\deschaseauxr\Documents\GED\documentId list.txt";
	var documentsIdList = File.ReadAllLines(documentsIdListFile);
	var documents = await GetDocumentsByDocumentsIdList(documentsIdList);
	documents
		//.Select(document => JsonDocument.Parse(JsonSerializer.Serialize(document)))
		.Dump();
}

const string ENVIRONMENT_CODE = "int";
HttpClient httpClient = new HttpClient { BaseAddress = new Uri($"https://api-ged-intra.{ENVIRONMENT_CODE}.maf.local/v2/Documents/") };
async Task<IEnumerable<Dictionary<string, object>>> GetDocumentsByDocumentsIdList(IEnumerable<string> documentsIdList) {
	var nbDocumentsIdInEachGroup = GetNbDocumentsIdInEachGroup();
	const char separator = ',';
	var documents =
		(await Task.WhenAll(
	        documentsIdList
	        .Chunk(nbDocumentsIdInEachGroup)
	        .Select(async documentsIdList => {
	            var commaSeparatedDocumentsIdList = string.Join(separator, documentsIdList.Select(documentId => $"'{documentId}'"));
				var queryString = $"?$filter=documentId in ({commaSeparatedDocumentsIdList})&$select=assigneRedacteur,assureurId,canalPrincipal,categoriesCote,categoriesFamille,categoriesTypeDocument,chantierId,codeOrigine,commentaire,compteId,dateDocument,dateNumerisation,deposeLe,deposePar,docn,documentId,extension,fichierNom,fichierNombrePages,fichierTaille,heureNumerisation,horodatage,important,libelle,modifieLe,modifiePar,numeroGc,numeroSinistre,periodeValiditeDebut,periodeValiditeFin,personneId,presenceAr,previewLink,qualiteValideeLe,qualiteValideePar,qualiteValideeValide,regroupementId,sens,sousDossierSinistre,statut,traiteLe,traitePar,typeGarantie,vuLe,vuPar";
				//Environment.Exit(0);
	            var json_documents = await httpClient.GetStringAsync(queryString);
				return 
				JsonDocument
					.Parse(json_documents)
					.RootElement.GetProperty("value")
					.EnumerateArray()
					.Select(jsonElement => JsonNode.Parse(jsonElement.ToString()))
					.Select(jsonNode =>
						new Dictionary<string, object> {
							{ "AssigneRedacteur", jsonNode["assigneRedacteur"]?.ToString() },
							{ "DocumentId", jsonNode["documentId"]?.ToString() },
							{ "Libelle", jsonNode["libelle"]?.ToString() },
							//{ "Commentaire", jsonNode["commentaire"]?.ToString() },
							//{ "TypeGarantie", jsonNode["typeGarantie"]?.ToString() },
							//{ "FichierNom", jsonNode["fichierNom"]?.ToString() },
							{ "Famille", jsonNode["categoriesFamille"]?.ToString() },
							{ "Côte", jsonNode["categoriesCote"]?.ToString() },
							{ "TypeDocument", jsonNode["categoriesTypeDocument"]?.ToString() },
							//{ "DeposeLe", GetDateOnly(jsonNode["deposeLe"]?.GetValue<DateTime>()) },
							//{ "DeposePar", jsonNode["deposePar"]?.ToString() },
							//{ "VuLe", GetDateOnly(jsonNode["vuLe"]?.GetValue<DateTime>()) },
							//{ "VuPar", jsonNode["vuPar"]?.ToString() },
							//{ "QualiteValideeLe", GetDateOnly(jsonNode["qualiteValideeLe"]?.GetValue<DateTime>()) },
							//{ "QualiteValideePar", jsonNode["qualiteValideePar"]?.ToString() },
							//{ "QualiteValideeValide", jsonNode["qualiteValideeValide"]?.ToString() },
							//{ "TraiteLe", GetDateOnly(jsonNode["traiteLe"]?.GetValue<DateTime>()) },
							//{ "TraitePar", jsonNode["traitePar"]?.ToString() },
							//{ "ModifieLe", GetDateOnly(jsonNode["modifieLe"]?.GetValue<DateTime>()) },
							//{ "ModifiePar", jsonNode["modifiePar"]?.ToString() },
							//{ "NumeroContrat", jsonNode["numeroContrat"]?.ToString() },
							{ "NumeroSinistre", jsonNode["numeroSinistre"]?.ToString() },
							//{ "ChantierId", jsonNode["chantierId"]?.ToString() },
							//{ "AssureurId", jsonNode["assureurId"]?.ToString() },
							{ "CompteId", jsonNode["compteId"]?.ToString() },
							{ "PersonneId", jsonNode["personneId"]?.ToString() },
							//{ "Sens", jsonNode["sens"]?.ToString() },
							//{ "SousDossierSinistre", jsonNode["sousDossierSinistre"]?.ToString() },
							//{ "Important", jsonNode["important"]?.ToString() },
							//{ "PeriodeValiditeDebut", GetDateOnly(jsonNode["periodeValiditeDebut"]?.GetValue<DateTime>()) },
							//{ "PeriodeValiditeFin", GetDateOnly(jsonNode["periodeValiditeFin"]?.GetValue<DateTime>()),
							//{ "Statut", jsonNode["statut"]?.ToString() }
						});
			        })))
				.SelectMany(documents => documents)
				.OrderBy(document => document["DocumentId"])
				.ToArray();
	return documents;
	
	int GetNbDocumentsIdInEachGroup() {
		const int requestUrlAndQueryLengthLimit = 2048;
		var baseUrlLength = httpClient.BaseAddress.AbsoluteUri.Length;
		const byte separatorLength = 1; // ","
		const int documentIdLength = 26; // "20241027021216767823360255".Length
		var nbDocumentsIdInEachGroup =
		    (requestUrlAndQueryLengthLimit - baseUrlLength) / (documentIdLength + separatorLength + 2);
		return nbDocumentsIdInEachGroup;
	}
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