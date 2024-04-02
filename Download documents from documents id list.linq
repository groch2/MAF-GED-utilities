<Query Kind="Statements">
  <Namespace>System.Data.SqlClient</Namespace>
  <IncludeLinqToSql>true</IncludeLinqToSql>
</Query>

const string environmentCode = "int";
const string stockerServerName = $"dns{environmentCode}gedstocker01";
const string databaseServer = $"bdd-ged.{environmentCode}.maf.local";
const string databaseName = "GEDMAF";
var document_id_list =
	File
		.ReadAllLines(@"C:\Users\deschaseauxr\Documents\GED\batch de génération de preview\surcharge de la RAM - plantage de OPCON\identifiants ARCHEAMAF des documents dont la génération de preview a échoué.txt")
		.Select(docId => $"'{docId}'");
const string connectionString = $"Server={databaseServer};Database={databaseName};Integrated Security=True";
using var connection = new SqlConnection(connectionString);
connection.Open();
using var command = connection.CreateCommand();
command.CommandText = 
	@$"SELECT [LINK], CONCAT('\\{stockerServerName}\Stockers\', REPLACE(REPLACE(REPLACE(RIGHT([LINK], LEN([LINK]) - 3), '-', '\'), '/', '\'), ' 1.PDF', '')) AS [FILE_PATH]
FROM [dbo].[ARCHEAMAF]
WHERE [ID_DOC] IN
({string.Join(',', document_id_list)})";
using var reader = command.ExecuteReader();
var dataTable = new DataTable();
dataTable.Load(reader);
const string destinationDirectory = @"C:\Users\deschaseauxr\Documents\GED\batch de génération de preview\surcharge de la RAM - plantage de OPCON\documents de test\documents de stocker";
if (Directory.Exists(destinationDirectory)) {
	"Directory exists and is about to be deleted, continue ?".Dump();
	Console.ReadLine();
	Directory.Delete(destinationDirectory, true);
}
Directory.CreateDirectory(destinationDirectory);
dataTable
	.AsEnumerable()
	.Select(dataRow => {
		var destination_file_name = dataRow[0].ToString().Replace('/', '_').Substring(3);
		var filePath = dataRow[1].ToString();
		return new { filePath, destination_file_name };
	})
	.ToList()
	.ForEach(file =>
		File.Copy(
			sourceFileName: file.filePath,
			destFileName: Path.Combine(destinationDirectory, file.destination_file_name),
			overwrite: false));
