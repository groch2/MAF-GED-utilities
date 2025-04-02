<Query Kind="Statements">
  <Namespace>System.Data.SqlClient</Namespace>
  <IncludeLinqToSql>true</IncludeLinqToSql>
</Query>

using var connection = new SqlConnection("Server=bdd-ged.int.maf.local;Database=GEDMAF;Trusted_Connection=True;");
using var command = connection.CreateCommand();
var documentIdList = string.Join(',', File.ReadAllLines(Path.Combine(Path.GetTempPath(), "doc_id_a_supprimer.txt")).Select(docId => $"'{docId}'"));
//documentIdList.Dump();
//Environment.Exit(0);
command.CommandText =
	@$"UPDATE [dbo].[ARCHEAMAF] SET [STATUS_INDEXATION] = 'SUPPRIME' WHERE [ID_DOC] IN ({documentIdList})";
//command.CommandText.Dump();
//Environment.Exit(0);
connection.Open();
var nbAffectedRows = command.ExecuteNonQuery();
new { nbAffectedRows }.Dump();
