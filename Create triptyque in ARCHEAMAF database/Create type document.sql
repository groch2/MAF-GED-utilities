USE [GEDMAF]
GO

BEGIN TRANSACTION;

DECLARE @CodeFamilleCible AS NVARCHAR(500) = 'GRANDS CHANTIERS'
DECLARE @CodeCoteCible AS NVARCHAR(500) = 'GC'

DECLARE @CoteDocumentId_GC AS INT = (
	SELECT [CoteDocumentId]
	FROM [Ref].[CoteDocument]
	JOIN [Ref].[FamilleDocument]
	ON [CoteDocument].[FamilleDocumentId] = [FamilleDocument].[FamilleDocumentId]
	AND [FamilleDocument].[Code] = @CodeFamilleCible
	WHERE [CoteDocument].[Code] = @CodeCoteCible
)
PRINT(CONCAT('Cote Document Id GC: ', @CoteDocumentId_GC))

DECLARE @NewTypeDocumentId AS INT =
	(SELECT MAX([TypeDocumentId]) FROM [Ref].[TypeDocument]) + 1
PRINT(CONCAT('New Type Document Id: ', @NewTypeDocumentId))

INSERT INTO [Ref].[TypeDocument] (
	[TypeDocumentId],
	[Code],
	[Libelle],
	[CoteDocumentId],
	[IsActif],
	[CodeTypeDoc],
	[VisibilitePapsExtra],
	[IsTimeline])
VALUES (
	  @NewTypeDocumentId
	,'FICHE D ETABLISSEMENT'
	,'fiche d''établissement'
	,@CoteDocumentId_GC
	,1
	,null
	,null
	,null)

-- Vérification
SELECT
	[FamilleDocument].[Code] AS 'Famille',
	[CoteDocument].[Code] AS 'Cote',
	[TypeDocument].[Code] AS 'Type'
FROM [Ref].[FamilleDocument]
JOIN [Ref].[CoteDocument]
	ON [FamilleDocument].[FamilleDocumentId] = [CoteDocument].[FamilleDocumentId]
JOIN [Ref].[TypeDocument]
	ON [CoteDocument].[CoteDocumentId] = [TypeDocument].[CoteDocumentId]
WHERE [TypeDocument].[TypeDocumentId] = @NewTypeDocumentId

ROLLBACK;
GO
