<Query Kind="Statements" />

var lines =
	File
		.ReadAllLines(@"C:\Users\deschaseauxr\Documents\GED\Mapping web api GED ARCHEAMAF.txt");
Enumerable
	.Range(0, lines.Length / 2)
	.Select(n => n * 2)
	.Select(n => new { property = lines[n], column = lines[n + 1] })
	.OrderBy(item => item.property, StringComparer.OrdinalIgnoreCase)
	.Dump();
	