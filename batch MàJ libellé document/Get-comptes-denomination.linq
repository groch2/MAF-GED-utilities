<Query Kind="Statements">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

const int compteIdPersonneMorale = 391453;
const int compteIdPersonnePhysique = 19468;
const string vigiApiBaseAddress = "https://api-vigi-intra.int.maf.local/api/v2/";
var client = new  HttpClient { BaseAddress = new Uri(vigiApiBaseAddress) };
(await Task.WhenAll(new [] { compteIdPersonneMorale, compteIdPersonnePhysique }
	.Select(async compteId =>
	{
		var rawJsonCompte = await client.GetStringAsync($"compte?CompteId={compteId}");
		var jsonCompte =
			JsonDocument
				.Parse(rawJsonCompte)
				.RootElement
				.GetProperty("data")
				.EnumerateArray()
				.Single();
		var personneId =
			jsonCompte
				.GetProperty("personneId")
				.GetInt32();
		var personneDenomination =
			jsonCompte
				.GetProperty("personneDenomination")
				.GetString();
		return new { compteId, personneId, personneDenomination };
	})))
	.OrderBy(compte => compte.compteId)
	.Dump();
