use [Contrat]
go

select c.COLUMN_NAME
from INFORMATION_SCHEMA.COLUMNS C
where c.TABLE_CATALOG = 'Contrat'
and c.TABLE_SCHEMA = 'Geco'
and c.TABLE_NAME = 'Chantier'
order by c.COLUMN_NAME
