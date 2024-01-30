<Query Kind="Statements">
  <Namespace>System.Data.SqlClient</Namespace>
  <IncludeLinqToSql>true</IncludeLinqToSql>
</Query>

using var connection = new SqlConnection("Server=bdd-ged.int.maf.local;Database=GEDMAF;Trusted_Connection=True;");
using var command = connection.CreateCommand();
const string documentId = "20240124134250334550647513";
command.CommandText = @$"DELETE FROM [dbo].[ARCHEAMAF] WHERE [ID_DOC] = '{documentId}'";
//command.CommandText.Dump();
connection.Open();
var nbAffectedRows = command.ExecuteNonQuery();
new { nbAffectedRows }.Dump();
