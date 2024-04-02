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
		//var documentId = getItemFromFields("fields.documentId");
		//var timestamp = DateTime.Parse(getItemFromFields("@timestamp"));
		return new {
			exceptionsClassName,
			exceptionsMessage
		};
	})
	.GroupBy(logEntry => logEntry)
	.Select(group => {
		var groupData = group.First();
		return new {
			groupData.exceptionsClassName,
			groupData.exceptionsMessage,
			count = group.Count()
		};
	})
	.OrderByDescending(exception => exception.count)
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