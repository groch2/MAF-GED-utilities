<Query Kind="Program">
  <Reference>C:\TeamProjects\GED_API\MAF.GED.API.Host\bin\Debug\net8.0\MAF.GED.Domain.Model.dll</Reference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Nodes</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeLinqToSql>true</IncludeLinqToSql>
</Query>

const string environmentCode = "hom";
async Task Main() {
	var httpClient =
		new HttpClient {
			BaseAddress = new Uri($"https://api-ged-intra.{environmentCode}.maf.local/v2/Documents/")
		};
	var raw_responses =
		await httpClient.GetStringAsync(
			$"?$filter=statut eq 'INDEXE' and categoriesFamille in ('DOCUMENTS CONTRAT','DOCUMENTS EMOA','DOCUMENTS PERSONNES') and personneId eq 9&$select=assigneRedacteur,assureurId,canalPrincipal,categoriesCote,categoriesFamille,categoriesTypeDocument,chantierId,codeOrigine,commentaire,compteId,dateDocument,dateNumerisation,deposeLe,deposePar,docn,documentId,extension,fichierNom,fichierNombrePages,fichierTaille,heureNumerisation,horodatage,important,libelle,modifieLe,modifiePar,numeroGc,numeroSinistre,periodeValiditeDebut,periodeValiditeFin,personneId,presenceAr,previewLink,qualiteValideeLe,qualiteValideePar,qualiteValideeValide,regroupementId,sens,sousDossierSinistre,statut,traiteLe,traitePar,typeGarantie,vuLe,vuPar");
	var documents =
		JsonDocument
			.Parse(raw_responses)
			.RootElement
			.GetProperty("value")
			.EnumerateArray()
			.Select(jsonElement => JsonNode.Parse(jsonElement.ToString()))
			.Select(jsonNode =>
				new {
					DocumentId = jsonNode["documentId"]?.ToString(),
					Libelle = jsonNode["libelle"]?.ToString(),
					CompteId = jsonNode["compteId"]?.ToString(),
					PersonneId = jsonNode["personneId"]?.ToString(),
					Famille = jsonNode["categoriesFamille"]?.ToString(),
					Côte = jsonNode["categoriesCote"]?.ToString(),
					TypeDocument = jsonNode["categoriesTypeDocument"]?.ToString(),
					//AssigneRedacteur = jsonNode["assigneRedacteur"]?.ToString(),
					//DeposeLe = GetDateOnly(jsonNode["deposeLe"]?.GetValue<DateTime>()),
					//DeposePar = jsonNode["deposePar"]?.ToString(),
					//ModifieLe = GetDateOnly(jsonNode["modifieLe"]?.GetValue<DateTime>()),
					//ModifiePar = jsonNode["modifiePar"]?.ToString(),
					//VuLe = GetDateOnly(jsonNode["vuLe"]?.GetValue<DateTime>()),
					//VuPar = jsonNode["vuPar"]?.ToString(),
					//QualiteValideeLe = GetDateOnly(jsonNode["qualiteValideeLe"]?.GetValue<DateTime>()),
					//QualiteValideePar = jsonNode["qualiteValideePar"]?.ToString(),
					//QualiteValideeValide = jsonNode["qualiteValideeValide"]?.ToString(),
					//TraiteLe = GetDateOnly(jsonNode["traiteLe"]?.GetValue<DateTime>()),
					//TraitePar = jsonNode["traitePar"]?.ToString(),
					//PeriodeValiditeDebut = GetDateOnly(jsonNode["periodeValiditeDebut"]?.GetValue<DateTime>()),
					//PeriodeValiditeFin = GetDateOnly(jsonNode["periodeValiditeFin"]?.GetValue<DateTime>()),
					//FichierNom = jsonNode["fichierNom"]?.ToString(),
					//TypeGarantie = jsonNode["typeGarantie"]?.ToString(),
					//NumeroContrat = jsonNode["numeroContrat"]?.ToString(),
					//ChantierId = jsonNode["chantierId"]?.ToString(),
					//Commentaire = jsonNode["commentaire"]?.ToString(),
					//AssureurId = jsonNode["assureurId"]?.ToString(),
					//Statut = jsonNode["statut"]?.ToString()
					//QueueStatus = GetDocumentStatus(jsonNode)
					//Sens = jsonNode["sens"]?.ToString(),
					//Important = jsonNode["important"]?.ToString(),
				}
			)
			.Select(document => {
				return new {
					document.DocumentId,
					document.Libelle,
				};
			});
	documents.Dump();	
}
