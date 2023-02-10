USE [CompanyDocumentStore]
GO

DROP VIEW IF EXISTS [dbo].[View_FileDescriptions]
GO

SET ANSI_NULLS ON
GO
 
SET QUOTED_IDENTIFIER ON
GO
 
CREATE VIEW [dbo].[View_FileDescriptions]
AS
	SELECT [dbo].[FileDescriptions].[Id], [FileName], [Description], [CreatedTimestamp], [UpdatedTimestamp], [SectionName]
	FROM [dbo].[FileDescriptions]
	INNER JOIN [dbo].[Sections] ON [dbo].[Sections].[Id] = [dbo].[FileDescriptions].[SectionId]
GO