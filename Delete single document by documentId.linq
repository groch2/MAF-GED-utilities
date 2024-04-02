<Query Kind="Statements">
  <Namespace>System.Data.SqlClient</Namespace>
  <IncludeLinqToSql>true</IncludeLinqToSql>
</Query>

const string ENVIRONMENT_CODE = "int";
if (!string.Equals(ENVIRONMENT_CODE, "int", StringComparison.OrdinalIgnoreCase)) {
	$"L'environnement cible est: ${ENVIRONMENT_CODE}".Dump();
	"êtes vous sur de vouloir supprimer ce document ?".Dump();
	Console.ReadLine();
}
using var connection = new SqlConnection($"Server=bdd-ged.{ENVIRONMENT_CODE}.maf.local;Database=GEDMAF;Trusted_Connection=True;");
using var command = connection.CreateCommand();
const string documentId = "20240124134250334550647513";
command.CommandText = @$"DELETE FROM [dbo].[ARCHEAMAF] WHERE [ID_DOC] = '{documentId}'";
//command.CommandText.Dump();
connection.Open();
var nbAffectedRows = command.ExecuteNonQuery();
new { nbAffectedRows }.Dump();
