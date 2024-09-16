use Contrat
go

select t.TABLE_CATALOG, t.TABLE_SCHEMA, t.TABLE_NAME, TABLE_TYPE
from INFORMATION_SCHEMA.TABLES t
where t.TABLE_NAME like '%questionnaire%'