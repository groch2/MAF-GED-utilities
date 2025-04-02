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
readonly HttpClient httpClient = new HttpClient { BaseAddress = new Uri($"https://api-ged-intra.{ENVIRONMENT_CODE}.maf.local/v2/Documents/") };
async Task<IEnumerable<Dictionary<DocProperty, object>>> GetDocumentsByDocumentsIdList(IEnumerable<string> documentsIdList) {
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
						new Dictionary<DocProperty, object> {
							{ DocProperty.AssigneRedacteur, jsonNode["assigneRedacteur"]?.ToString() },
							{ DocProperty.DocumentId, jsonNode["documentId"]?.ToString() },
							{ DocProperty.Libelle, jsonNode["libelle"]?.ToString() },
							{ DocProperty.Commentaire, jsonNode["commentaire"]?.ToString() },
							{ DocProperty.TypeGarantie, jsonNode["typeGarantie"]?.ToString() },
							{ DocProperty.FichierNom, jsonNode["fichierNom"]?.ToString() },
							{ DocProperty.Famille, jsonNode["categoriesFamille"]?.ToString() },
							{ DocProperty.Côte, jsonNode["categoriesCote"]?.ToString() },
							{ DocProperty.TypeDocument, jsonNode["categoriesTypeDocument"]?.ToString() },
							{ DocProperty.DeposeLe, GetDateOnly(jsonNode["deposeLe"]?.GetValue<DateTime>()) },
							{ DocProperty.DeposePar, jsonNode["deposePar"]?.ToString() },
							{ DocProperty.VuLe, GetDateOnly(jsonNode["vuLe"]?.GetValue<DateTime>()) },
							{ DocProperty.VuPar, jsonNode["vuPar"]?.ToString() },
							{ DocProperty.QualiteValideeLe, GetDateOnly(jsonNode["qualiteValideeLe"]?.GetValue<DateTime>()) },
							{ DocProperty.QualiteValideePar, jsonNode["qualiteValideePar"]?.ToString() },
							{ DocProperty.QualiteValideeValide, jsonNode["qualiteValideeValide"]?.ToString() },
							{ DocProperty.TraiteLe, GetDateOnly(jsonNode["traiteLe"]?.GetValue<DateTime>()) },
							{ DocProperty.TraitePar, jsonNode["traitePar"]?.ToString() },
							{ DocProperty.ModifieLe, GetDateOnly(jsonNode["modifieLe"]?.GetValue<DateTime>()) },
							{ DocProperty.ModifiePar, jsonNode["modifiePar"]?.ToString() },
							{ DocProperty.NumeroContrat, jsonNode["numeroContrat"]?.ToString() },
							{ DocProperty.NumeroSinistre, jsonNode["numeroSinistre"]?.ToString() },
							{ DocProperty.ChantierId, jsonNode["chantierId"]?.ToString() },
							{ DocProperty.AssureurId, jsonNode["assureurId"]?.ToString() },
							{ DocProperty.CompteId, jsonNode["compteId"]?.ToString() },
							{ DocProperty.PersonneId, jsonNode["personneId"]?.ToString() },
							{ DocProperty.Sens, jsonNode["sens"]?.ToString() },
							{ DocProperty.SousDossierSinistre, jsonNode["sousDossierSinistre"]?.ToString() },
							{ DocProperty.Important, jsonNode["important"]?.ToString() },
							{ DocProperty.PeriodeValiditeDebut, GetDateOnly(jsonNode["periodeValiditeDebut"]?.GetValue<DateTime>()) },
							{ DocProperty.PeriodeValiditeFin, GetDateOnly(jsonNode["periodeValiditeFin"]?.GetValue<DateTime>()) },
							{ DocProperty.Statut, jsonNode["statut"]?.ToString() }
						});
			        })))
				.SelectMany(documents => documents);
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
	
	DateOnly? GetDateOnly(DateTime? dateTime) =>
		dateTime.HasValue ? DateOnly.FromDateTime(dateTime.Value) : null;
}

enum DocProperty {
	AssigneRedacteur,
	AssureurId,
	ChantierId,
	Commentaire,
	CompteId,
	Côte,
	DeposeLe,
	DeposePar,
	DocumentId,
	Famille,
	FichierNom,
	Important,
	Libelle,
	ModifieLe,
	ModifiePar,
	NumeroContrat,
	NumeroSinistre,
	PeriodeValiditeDebut,
	PeriodeValiditeFin,
	PersonneId,
	QualiteValideeLe,
	QualiteValideePar,
	QualiteValideeValide,
	Sens,
	SousDossierSinistre,
	Statut,
	TraiteLe,
	TraitePar,
	TypeDocument,
	TypeGarantie,
	VuLe,
	VuPar
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