<Query Kind="Statements">
  <Namespace>System.Data.SqlClient</Namespace>
  <IncludeLinqToSql>true</IncludeLinqToSql>
</Query>

using var connection = new SqlConnection("Server=bdd-ged.int.maf.local;Database=GEDMAF;Trusted_Connection=True;");
var command = connection.CreateCommand();
var documentIdList = string.Join(',', File.ReadAllLines(@"C:\Users\deschaseauxr\AppData\Local\Temp\documents_to_patch.txt").Select(documentId => $"'{documentId}'"));
command.CommandText = @$"UPDATE [dbo].[ARCHEAMAF] SET [CODE_GESTIONNAIRE_REDAC] = 'MOMO', [CODE_REDACTEUR_DEPOSE] = 'XXX', [FAMILLE] = 'DOCUMENTS ENTRANTS', [NO_DOSSIER] = 'DS190001100W' WHERE [ID_DOC] IN ({documentIdList})";
//command.CommandText.Dump();
connection.Open();
var nbAffectedRows = command.ExecuteNonQuery();
new { nbAffectedRows }.Dump();
