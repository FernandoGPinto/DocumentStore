CREATE DATABASE CompanyDocumentStore 
ON
PRIMARY ( NAME = Doc1,
    FILENAME = 'c:\data\docdat1.mdf'),
FILEGROUP FileStreamGroup1 CONTAINS FILESTREAM( NAME = Doc3,
    FILENAME = 'c:\data\filestream1')
LOG ON  ( NAME = Doclog1,
    FILENAME = 'c:\data\doclog1.ldf')
GO