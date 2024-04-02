<Query Kind="Statements">
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Nodes</Namespace>
</Query>

var rawText = File.ReadAllText(@"C:\Users\deschaseauxr\Documents\GED\batch de génération de preview\surcharge de la RAM - plantage de OPCON\quantité de documents générés par preview.json");
var jsonDocument = JsonDocument.Parse(rawText);
jsonDocument
	.RootElement
	.GetProperty("rawResponse")
	.GetProperty("hits")
	.GetProperty("hits")
	.EnumerateArray()
	.Select(hit => {
		var fields = hit.GetProperty("fields");
		var message = fields.GetProperty("message.keyword").EnumerateArray().First().GetString();
		return new { message, fields};
	})
	.Where(hit =>
		hit.message.EndsWith(" previews à générer "))
	.Select(hit => {
		var fields = hit.fields;
		var timestamp = DateTime.Parse(fields.GetProperty("@timestamp").EnumerateArray().First().GetString());
		var message = hit.message;
		var nbPreviewGenerated = int.Parse(Regex.Match(pattern: @"^GeneratePreviews --- (\b\d+\b) previews à générer $", input: message).Groups[1].Value);
		return new { timestamp, nbPreviewGenerated};
	})
	.OrderBy(item => item.timestamp)
	.Dump();