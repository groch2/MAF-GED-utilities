<Query Kind="Program">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <RemoveNamespace>System.Text.RegularExpressions</RemoveNamespace>
</Query>

const string ENVIRONMENT_CODE = "int";
async static Task Main() {
	//typeof(Famille).GetProperties().Select(p => $"Famille{p.Name} = famille.{p.Name},").Dump();
	//typeof(Cote).GetProperties().Select(p => $"Cote{p.Name} = cote.{p.Name},").Dump();
	//typeof(TypeDocument).GetProperties().Select(p => $"TypeDocument{p.Name} = typeDocument.{p.Name},").Dump();
	//Environment.Exit(0);

	var httpClient = new HttpClient { BaseAddress = new Uri($"https://api-ged-intra.{ENVIRONMENT_CODE}.maf.local/v2/") };
	var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

	var famillesListJsonResponse =
		await httpClient.GetStringAsync("Familles?$select=familleDocumentId,code,libelle&$filter=isActif eq true");
	var famillesList =
		JsonDocument
			.Parse(famillesListJsonResponse)
			.RootElement.GetProperty("value")
			.EnumerateArray()
			.Select(item =>
				JsonSerializer.Deserialize<Famille>(
				item.GetRawText(),
				jsonSerializerOptions));

	var cotesListJsonResponse =
		await httpClient.GetStringAsync("Cotes?$select=coteDocumentId,code,codeCouleur,libelle,familleDocumentId&$filter=isActif eq true");
	var cotesList =
		JsonDocument
			.Parse(cotesListJsonResponse)
			.RootElement.GetProperty("value")
			.EnumerateArray()
			.Select(item =>
				JsonSerializer.Deserialize<Cote>(
					item.GetRawText(),
					jsonSerializerOptions));

	var typesDocumentListJsonResponse =
		await httpClient.GetStringAsync("TypesDocuments?$select=typeDocumentId,code,libelle,coteDocumentId&$filter=isActif eq true");
	var typesDocumentList =
		JsonDocument
			.Parse(typesDocumentListJsonResponse)
			.RootElement.GetProperty("value")
			.EnumerateArray()
			.Select(item =>
				JsonSerializer.Deserialize<TypeDocument>(
					item.GetRawText(),
					jsonSerializerOptions));

	famillesList
		.SelectMany(
			famille => cotesList
				.Where(cote =>
					cote.FamilleDocumentId == famille.FamilleDocumentId)
				.SelectMany(cote =>
					typesDocumentList
						.Where(typeDocument =>
							typeDocument.CoteDocumentId == cote.CoteDocumentId)
						.Select(typeDocument =>
							new TriptyqueHierarchy(
								Famille:
									new TriptyqueItem(
										Id: famille.FamilleDocumentId,
										Code: famille.Code,
										Libellé: famille.Libelle),
								Cote:
									new TriptyqueItem(
										Id: cote.CoteDocumentId,
										Code: cote.Code,
										Libellé: cote.Libelle),
								TypeDocument:
									new TriptyqueItem(
										Id: typeDocument.TypeDocumentId,
										Code: typeDocument.Code,
										Libellé: typeDocument.Libelle))
				))
				.OrderBy(triptyque => triptyque.Famille.Code, StringComparer.OrdinalIgnoreCase)
				.ThenBy(triptyque => triptyque.Cote.Code, StringComparer.OrdinalIgnoreCase)
				.ThenBy(triptyque => triptyque.TypeDocument.Code, StringComparer.OrdinalIgnoreCase)
				//.Where(t => (
				//	AreStringEqualCaseInsensitive(t.Famille.Code, "DOCUMENTS PERSONNES") && 
				//		AreStringEqualCaseInsensitive(t.Cote.Code, "IDENTITE") && (
				//			AreStringEqualCaseInsensitive(t.TypeDocument.Code, "PIECE IDENTITE") ||
				//			AreStringEqualCaseInsensitive(t.TypeDocument.Code, "KBIS"))) || (
				//	AreStringEqualCaseInsensitive(t.Famille.Code, "DOCUMENTS CONTRAT") &&
				//		AreStringEqualCaseInsensitive(t.Cote.Code, "SOUSCRIPTION") &&
				//		AreStringEqualCaseInsensitive(t.TypeDocument.Code, "COPIE PIECE IDENTITE")))
				.Select(t =>
					new {
						FamilleDocumentId = t.Famille.Id,
						Famille = t.Famille.Code,
						CoteDocumentId = t.Cote.Id,
						Cote = t.Cote.Code,
						TypeDocumentId = t.TypeDocument.Id,
						TypeDocument = t.TypeDocument.Code,
					}))
				.Where(t => AreStringEqualCaseInsensitive(t.Famille, "DOCUMENTS MAF CONSEIL"))
				//.Where(t =>
				//	AreStringEqualCaseInsensitive(t.Famille, "DOCUMENTS CONTRAT") ||
				//	AreStringEqualCaseInsensitive(t.Famille, "DOCUMENTS EMOA") ||
				//	AreStringEqualCaseInsensitive(t.Famille, "DOCUMENTS PAPS"))
				.Dump();
}

record Famille(int FamilleDocumentId, string Code, string Libelle);
record Cote(int CoteDocumentId, string Code, string CodeCouleur, string Libelle, int FamilleDocumentId);
record TypeDocument(int TypeDocumentId, string Code, string Libelle, int CoteDocumentId);
record TriptyqueItem(int Id, string Code, string Libellé);
record TriptyqueHierarchy(TriptyqueItem Famille, TriptyqueItem Cote, TriptyqueItem TypeDocument);

static bool AreStringEqualCaseInsensitive(string a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
