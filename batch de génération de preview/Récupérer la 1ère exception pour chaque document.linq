<Query Kind="Statements">
  <Namespace>System.Text.Json</Namespace>
</Query>

var raw_text = File.ReadAllText(@"C:\Users\deschaseauxr\Documents\GED\batch de génération de preview\surcharge de la RAM - plantage de OPCON\exceptions enregistrées dans les logs ELK.json");
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
		var exceptionsClassName = getItemFromFields("exceptions.ClassName");
		var exceptionsMessage = getItemFromFields("exceptions.Message");
		var documentId = getItemFromFields("fields.documentId");
		var timestamp = DateTime.Parse(getItemFromFields("@timestamp"));
		return new {
			documentId,
			exceptions = new {
				ClassName = exceptionsClassName,
				Message = exceptionsMessage
			},
			timestamp
		};
	})
	.GroupBy(logEntryGroup => logEntryGroup.documentId)
	.Select(logEntryGroup => {
		var firstErrorTimeStamp = logEntryGroup.Select(logEntry => logEntry.timestamp).Min();
		return logEntryGroup.First(logEntry => logEntry.timestamp == firstErrorTimeStamp);
	})
	.Dump();

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