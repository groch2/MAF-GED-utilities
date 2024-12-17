use GEDMAF
go

with TypeDocumentDoublon as (
  select TypeDocument.Code, TypeDocument.CoteDocumentId
  from ref.TypeDocument
  group by TypeDocument.CoteDocumentId, TypeDocument.Code
  having count(TypeDocument.TypeDocumentId) > 1)
select 
  [Ref].[FamilleDocument].Code as 'famille document code',
  [Ref].[CoteDocument].Code as 'cote document code',
  ref.TypeDocument.Code as 'type document code',
  ref.TypeDocument.TypeDocumentId,
  ref.TypeDocument.CodeTypeDoc,
  ref.TypeDocument.IsActif
from TypeDocumentDoublon
join ref.TypeDocument
on TypeDocumentDoublon.Code = ref.TypeDocument.Code
and TypeDocumentDoublon.CoteDocumentId = ref.TypeDocument.CoteDocumentId
join [Ref].[CoteDocument]
on ref.TypeDocument.CoteDocumentId = [Ref].[CoteDocument].CoteDocumentId
join [Ref].[FamilleDocument]
on [Ref].[CoteDocument].FamilleDocumentId = [Ref].[FamilleDocument].FamilleDocumentId
--where [Ref].[FamilleDocument].Code = 'DOCUMENTS PAPS'
order by [Ref].[FamilleDocument].Code, [Ref].[CoteDocument].Code, ref.TypeDocument.Code, ref.TypeDocument.TypeDocumentId