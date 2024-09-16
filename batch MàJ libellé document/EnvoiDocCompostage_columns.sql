use [GEDMAF]
go

select c.COLUMN_NAME
from INFORMATION_SCHEMA.COLUMNS c
where c.TABLE_CATALOG = 'GEDMAF'
and c.TABLE_SCHEMA = 'Editions'
and c.TABLE_NAME = 'EnvoiDocumentCompostage'
order by c.COLUMN_NAME