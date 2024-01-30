<Query Kind="Statements">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text.Json</Namespace>
</Query>

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
var triptyques =
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
								FamilleId = famille.FamilleDocumentId,
								FamilleCode = famille.Code,
								FamilleLibelle = famille.Libelle,

								CoteId = cote.CoteDocumentId,
								CoteCode = cote.Code,
								CoteLibelle = cote.Libelle,

								TypeDocumentId = typeDocument.TypeDocumentId,
								TypeDocumentCode = typeDocument.Code,
								TypeDocumentLibelle = typeDocument.Libelle,
							})
				))
				.OrderBy(tryptique => tryptique.FamilleCode, StringComparer.OrdinalIgnoreCase)
				.ThenBy(tryptique => tryptique.CoteCode, StringComparer.OrdinalIgnoreCase)
				.ThenBy(tryptique => tryptique.TypeDocumentCode, StringComparer.OrdinalIgnoreCase);
triptyques.Dump();
var triptyquesFileContentLines =
	triptyques
		.Select(
			triptyque => new {
				Famille = triptyque.FamilleCode,
				Cote = triptyque.CoteCode,
				TypeDocument = triptyque.TypeDocumentCode
			})
		.Select(triptyque => string.Join(',', new [] { triptyque.Famille, triptyque.Cote, triptyque.TypeDocument }));
File.WriteAllLines(
	contents: triptyquesFileContentLines,
	path: @"C:\Users\deschaseauxr\Documents\GED\triptyques.csv");

record Famille(int FamilleDocumentId, string Code, string Libelle);
record Cote(int CoteDocumentId, string Code, string Libelle, int FamilleDocumentId);
record TypeDocument(int TypeDocumentId, string Code, string Libelle, int CoteDocumentId);
