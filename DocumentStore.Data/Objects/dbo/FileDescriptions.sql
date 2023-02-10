USE [CompanyDocumentStore]
GO

DROP TABLE IF EXISTS [dbo].[FileDescriptions]
GO

SET ANSI_NULLS ON
GO
 
SET QUOTED_IDENTIFIER ON
GO
 
CREATE TABLE [dbo].[FileDescriptions](
    [StreamId] uniqueidentifier NOT NULL,
    [FileName] nvarchar(255) NOT NULL,
    [Description] nvarchar(255) NULL,
    [SectionId] int NOT NULL
	CONSTRAINT [PK_FileDescriptions] PRIMARY KEY CLUSTERED 
	(
		[StreamId] ASC
	) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	CONSTRAINT FK_StreamId FOREIGN KEY (StreamId) REFERENCES FileRepository(stream_id) ON DELETE CASCADE,
	CONSTRAINT FK_SectionId FOREIGN KEY (SectionId) REFERENCES Sections(Id)
) ON [PRIMARY]
GO