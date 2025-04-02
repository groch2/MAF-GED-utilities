DECLARE @FamilleCode AS NVARCHAR(500) = '__FamilleCode__'
DECLARE @FamilleLibelle AS NVARCHAR(500) = '__FamilleLibelle__'
DECLARE @CoteCode AS NVARCHAR(500) = '__CoteCode__'
DECLARE @CoteLibelle AS NVARCHAR(500) = '__CoteLibelle__'
DECLARE @TypeDocumentCode AS NVARCHAR(500) = '__TypeDocumentCode__'
DECLARE @TypeDocumentLibelle AS NVARCHAR(500) = '__TypeDocumentLibelle__'

DECLARE @FamilleId AS INT = (
  SELECT TOP 1 [FamilleDocumentId]
  FROM [Ref].[FamilleDocument]
  WHERE [Code] = @FamilleCode)
--print('famille id: ' + CAST(@FamilleId as varchar(10)))

if @FamilleId is null
begin
  SET @FamilleId = (SELECT MAX([FamilleDocumentId]) FROM [Ref].[FamilleDocument]) + 1
  insert into [Ref].[FamilleDocument] ([FamilleDocumentId], [Code], [Libelle], [IsActif])
  values  (@FamilleId, @FamilleCode, @FamilleLibelle, 1)
end

DECLARE @CoteId AS INT = (
  SELECT TOP 1 [CoteDocumentId]
  FROM [Ref].[CoteDocument]
  WHERE [FamilleDocumentId] = @FamilleId and [Code] = @CoteCode)
--print('cote id: ' + CAST(@CoteId as varchar(10)))

if @CoteId is null
begin
  set @CoteId = (SELECT MAX([CoteDocumentId]) FROM [Ref].[CoteDocument]) + 1
  insert into [Ref].[CoteDocument] ([CoteDocumentId], [Code], [Libelle], [FamilleDocumentId], [IsActif])
  values (@CoteId, @CoteCode, @CoteLibelle, @FamilleId, 1)
end

DECLARE @TypeDocumentId AS INT = (
  SELECT TOP 1 [TypeDocumentId]
  FROM [Ref].[TypeDocument]
  WHERE [CoteDocumentId] = @CoteId and [Code] = @TypeDocumentCode)
--print('type document id: ' + CAST(@TypeDocumentId as varchar(10)))

if @TypeDocumentId is null
begin
  set @TypeDocumentId = (
    SELECT MAX([TypeDocumentId]) FROM [Ref].[TypeDocument]) + 1
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
end

-- Vérification
SELECT
	[FamilleDocument].[FamilleDocumentId] AS [Famille id],
	[FamilleDocument].[Code] AS [Famille code],
	[CoteDocument].[CoteDocumentId] AS [Cote id],
	[CoteDocument].[Code] AS [Cote code],
	[TypeDocument].[TypeDocumentId] AS [Type document id],
	[TypeDocument].[Code] AS [Type document code]
FROM [Ref].[FamilleDocument]
JOIN [Ref].[CoteDocument]
	ON [FamilleDocument].[FamilleDocumentId] = [CoteDocument].[FamilleDocumentId]
JOIN [Ref].[TypeDocument]
	ON [CoteDocument].[CoteDocumentId] = [TypeDocument].[CoteDocumentId]
WHERE [TypeDocument].[TypeDocumentId] = @TypeDocumentId
