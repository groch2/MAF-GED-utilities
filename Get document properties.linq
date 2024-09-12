<Query Kind="Statements">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

var httpClient = new HttpClient();
var rawXml = await httpClient.GetStringAsync("https://api-ged-intra.int.maf.local/v2/$metadata");
XElement
	.Parse(rawXml)
	.Descendants()
	.First(n => n.Name.LocalName == "EntityType" && n.Attributes().Any(a => a.Name == "Name" && a.Value == "Document"))
	.Descendants()
	.Where(n => n.Name.LocalName == "Property")
	.Select(n =>
		new {
			Name = CapitalizeFirstLetter(n.Attribute("Name").Value),
			//Type = n.Attribute("Type").Value,
			Type = Regex.Replace(input: n.Attribute("Type").Value, pattern: @"\bEdm\.", replacement: string.Empty)
		})
	//.Where(p => p.Type.Contains("Date", StringComparison.OrdinalIgnoreCase))
	//.Select(p => new { p.Name })
	.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
	.Dump();

string CapitalizeFirstLetter(string input) =>
	$"{$"{input[0]}".ToUpperInvariant()}{input.Substring(1)}";
