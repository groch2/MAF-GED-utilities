USE [GEDMAF]
GO

BEGIN TRANSACTION;

DECLARE @FamilleCode AS NVARCHAR(500) = 'DOCUMENTS CONTRAT'
DECLARE @FamilleLibelle AS NVARCHAR(500) = 'documents contrat'
DECLARE @CoteCode AS NVARCHAR(500) = 'GESTION'
DECLARE @CoteLibelle AS NVARCHAR(500) = 'gestion'
DECLARE @TypeDocumentCode AS NVARCHAR(500) = 'AR POSTE'
DECLARE @TypeDocumentLibelle AS NVARCHAR(500) = 'AR posté'

DECLARE @FamilleId AS INT =
	(SELECT MAX([FamilleDocumentId]) FROM [Ref].[FamilleDocument]) + 1
insert into [Ref].[FamilleDocument] ([FamilleDocumentId], [Code], [Libelle], [IsActif])
values  (@FamilleId, @FamilleCode, @FamilleLibelle, 1)

DECLARE @CoteId AS INT =
	(SELECT MAX([CoteDocumentId]) FROM [Ref].[CoteDocument]) + 1
insert into [Ref].[CoteDocument] ([CoteDocumentId], [Code], [Libelle], [FamilleDocumentId], [IsActif])
values (@CoteId, @CoteCode, @CoteLibelle, @FamilleId, 1)

DECLARE @TypeDocumentId AS INT =
	(SELECT MAX([TypeDocumentId]) FROM [Ref].[TypeDocument]) + 1
INSERT INTO [Ref].[TypeDocument] (
	[TypeDocumentId],
	[Code],
	[Libelle],
	[CoteDocumentId],
	[IsActif],
	[CodeTypeDoc],
	[VisibilitePapsExtra],
	[IsTimeline])
VALUES 
	(@TypeDocumentId
	,@TypeDocumentCode
	,@TypeDocumentLibelle
	,@CoteId
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
WHERE [TypeDocument].[TypeDocumentId] = @TypeDocumentId

ROLLBACK;
GO
