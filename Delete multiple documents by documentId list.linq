<Query Kind="Statements">
  <Namespace>System.Data.SqlClient</Namespace>
  <IncludeLinqToSql>true</IncludeLinqToSql>
</Query>

using var connection = new SqlConnection("Server=bdd-ged.int.maf.local;Database=GEDMAF;Trusted_Connection=True;");
using var command = connection.CreateCommand();
var documentIdList = string.Join(',', File.ReadAllLines(@"C:\Users\deschaseauxr\AppData\Local\Temp\doc_id_a_supprimer.txt").Select(documentId => $"'{documentId}'"));
command.CommandText = @$"DELETE FROM [dbo].[ARCHEAMAF] WHERE [ID_DOC] IN ({documentIdList})";
//command.CommandText.Dump();
connection.Open();
var nbAffectedRows = command.ExecuteNonQuery();
new { nbAffectedRows }.Dump();
