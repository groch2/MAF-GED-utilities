<Query Kind="Statements">
  <Namespace>System.Text.Json</Namespace>
</Query>

var raw_text = File.ReadAllText(@"C:\Users\deschaseauxr\Documents\GED\batch de génération de preview\surcharge de la RAM - plantage de OPCON\exceptions enregistrées dans les logs ELK.json");
var documentsIdWithExistingError =
	JsonDocument
		.Parse(raw_text)
		.RootElement
		.GetProperty("hits")
		.GetProperty("hits")
		.EnumerateArray()
		.Select(item => {
			var fields = item.GetProperty("fields");
			Func<string, string> getItemFromFields =
				(string fieldName) => GetFirstItemOfJsonElement(fields, fieldName);
			var documentId = getItemFromFields("fields.documentId");
			return documentId;
		})
		.Distinct()
		.ToArray();
var documentsIdWithPreviewTagFalse =
	File.ReadAllLines(@"C:\Users\deschaseauxr\Documents\GED\batch de génération de preview\documents dont le flag de preview est à false.txt");
var documents_without_preview_without_error =
	documentsIdWithPreviewTagFalse
		.Except(
			second: documentsIdWithExistingError,
			StringComparer.OrdinalIgnoreCase);
new {
	nombre_de_documents_sans_preview_et_sans_erreur =
		documents_without_preview_without_error.Count()
}.Dump();

string GetFirstItemOfJsonElement(JsonElement jsonElement, string propertyName) {
	JsonElement property;
	try {
		property =
			jsonElement
				.GetProperty(propertyName);
	} catch (KeyNotFoundException) {
		return string.Empty;
	}
	return 
		property
			.EnumerateArray()
			.First()
			.GetString();
}