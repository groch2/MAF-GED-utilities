<Query Kind="Program">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <RemoveNamespace>System.Text.RegularExpressions</RemoveNamespace>
</Query>

async static Task Main() {
	//typeof(Famille).GetProperties().Select(p => $"Famille{p.Name} = famille.{p.Name},").Dump();
	//typeof(Cote).GetProperties().Select(p => $"Cote{p.Name} = cote.{p.Name},").Dump();
	//typeof(TypeDocument).GetProperties().Select(p => $"TypeDocument{p.Name} = typeDocument.{p.Name},").Dump();
	//Environment.Exit(0);

	var httpClient = new HttpClient { BaseAddress = new Uri("https://api-ged-intra.int.maf.local/v2/") };
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
		await httpClient.GetStringAsync("Cotes?$select=coteDocumentId,code,libelle,familleDocumentId&$filter=isActif eq true");
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
							new {
								Famille =
									new TriptyqueItem(
										Id: famille.FamilleDocumentId,
										Code: famille.Code,
										Libellé: famille.Libelle),
								Cote =
									new TriptyqueItem(
										Id: cote.CoteDocumentId,
										Code: cote.Code,
										Libellé: cote.Libelle),
								TypeDocument =
									new TriptyqueItem(
										Id: typeDocument.TypeDocumentId,
										Code: typeDocument.Code,
										Libellé: typeDocument.Libelle),
							})
				))
				.OrderBy(tryptique => tryptique.Famille.Code, StringComparer.OrdinalIgnoreCase)
				.ThenBy(tryptique => tryptique.Cote.Code, StringComparer.OrdinalIgnoreCase)
				.ThenBy(tryptique => tryptique.TypeDocument.Code, StringComparer.OrdinalIgnoreCase)
				//.Where(t =>
					//AreStringEqualCaseInsensitive(t.Famille.Code, "DOCUMENTS EMOA") && 
					//AreStringEqualCaseInsensitive(t.Cote.Code, "GESTION") &&
					//AreStringEqualCaseInsensitive(t.TypeDocument.Code, "AR POSTE"))
				//.Where(t => t.TypeDocument.Contains("fausse", StringComparison.OrdinalIgnoreCase)
				.Select(t => new {
					Famille = t.Famille.Code,
					Côte = t.Cote.Code,
					TypeDocument = t.TypeDocument.Code 
				})
				.Dump();
}

record Famille(int FamilleDocumentId, string Code, string Libelle);
record Cote(int CoteDocumentId, string Code, string Libelle, int FamilleDocumentId);
record TypeDocument(int TypeDocumentId, string Code, string Libelle, int CoteDocumentId);
record TriptyqueItem(int Id, string Code, string Libellé);

static bool AreStringEqualCaseInsensitive(string a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
