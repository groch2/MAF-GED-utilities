<Query Kind="Statements">
  <Namespace>System.Data.SqlClient</Namespace>
  <Namespace>System.Globalization</Namespace>
  <IncludeLinqToSql>true</IncludeLinqToSql>
</Query>

// ENVIRONEMENT (à mettre à "prd" pour exécuter en production)
const string environmentCode = "int";
const string stockerServerName = $"dns{environmentCode}gedstocker01";
const string databaseServer = $"bdd-ged.{environmentCode}.maf.local";
const string databaseName = "GEDMAF";
const string connectionString = $"Server={databaseServer};Database={databaseName};Integrated Security=True";

var Start_Date =
	new DateTime(year: 2024, month: 03, day: 20);		
var End_Date =
	new DateTime(year: 2024, month: 03, day: 22);
if (End_Date < Start_Date) {
	throw new Exception("la date de fin ne doit pas être inférieure à la date de début");
}

var formatted_Start_Date =
	Start_Date.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
var formatted_End_Date =
	End_Date.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

new {
	Start_Date,
	End_Date
}.Dump();

using var connection = new SqlConnection(connectionString);
connection.Open();
using var command = connection.CreateCommand();
command.CommandText =
	@$"WITH [DOCS_WITHOUT_PREVIEW] AS(
SELECT [DOCN]
      ,[ID_DOC]
	  ,[LINK]
      ,right([LINK], charindex('.', reverse([LINK]) + '.') - 1) as [FILE_EXTENSION]
FROM [dbo].[ARCHEAMAF]
WHERE [DATE_NUM] >= {formatted_Start_Date} AND [DATE_NUM] <= {formatted_End_Date}
AND [PREVIEW] = 0)

SELECT [ID_DOC], [LINK]
FROM [DOCS_WITHOUT_PREVIEW]
WHERE [FILE_EXTENSION] = 'PDF'";
using var reader = command.ExecuteReader();
var dataTable = new DataTable();
dataTable.Load(reader);
var nb_Existing_Preview_Files =
	dataTable
		.AsEnumerable()
		.Count(dataRow => {
			var link = dataRow[1].ToString();
			var previewFilePath = GetPreviewFilePath(link);
			return File.Exists(previewFilePath);
		});
var nb_Documents_With_Preview_Flag_False = dataTable.Rows.Count;
new {
	nb_Documents_With_Preview_Flag_False,
	nb_Existing_Preview_Files
}.Dump();

const string stockerBaseDirectory = @$"\\dns{environmentCode}gedstocker01\Stockers\";
static string GetPreviewFilePath(string link) {
    var linkArray = (link.Split(new char[] { ' ' }, StringSplitOptions.None));
    var formattedLink = linkArray[0].Replace("OB=", "");
	link = formattedLink;

	var numeroFile = link.Replace("-", "\\");
    var numeroFile1 = numeroFile.Replace("/", "\\");

    var dataFile2 = stockerBaseDirectory+ numeroFile1;
	dataFile2 = dataFile2 + "img";
	
    return dataFile2;
}
