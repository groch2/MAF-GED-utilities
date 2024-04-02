<Query Kind="Statements">
  <Namespace>System.Text.Json</Namespace>
</Query>

var raw_text = File.ReadAllText(@"C:\Users\deschaseauxr\Documents\GED\batch de génération de preview\surcharge de la RAM - plantage de OPCON\résultat de recherche ELK.json");
JsonDocument
	.Parse(raw_text)
	.RootElement
	.GetProperty("hits")
	.GetProperty("hits")
	.EnumerateArray()
	.Select(item => {
		var fields = item.GetProperty("fields");
		var message = fields.GetProperty("message").EnumerateArray().First().GetString();
		var sourceContext = fields.GetProperty("fields.SourceContext").EnumerateArray().First().GetString();
		return new { message, sourceContext };
	})
	.Where(errorLog => errorLog.sourceContext == "MAF.GED.Batch.Domain.Services.PreviewService")
	.Select(errorLog => errorLog.message)
	.GroupBy(_ => _)
	.Select(group => {
		var firstItem = group.First();
		return new { errorMessage = firstItem, count = group.Count() };
	})
	.Dump();
