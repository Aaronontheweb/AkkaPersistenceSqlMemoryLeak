IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'SqlReproduction')
BEGIN
    CREATE DATABASE [SqlReproduction];
END
GO

USE [master];
GO

IF NOT EXISTS (SELECT * FROM sys.sql_logins WHERE name = 'sqlrepro-user')
BEGIN
    CREATE LOGIN [sqlrepro-user] WITH PASSWORD = 'password123', CHECK_POLICY = OFF;
    ALTER SERVER ROLE [sysadmin] ADD MEMBER [sqlrepro-user];
END
GO

USE [SqlReproduction];
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'sqlrepro-user')
BEGIN
    CREATE USER [sqlrepro-user] FOR LOGIN [sqlrepro-user];
END
GO

-- Grant read and schema modification permissions
ALTER ROLE [db_owner] ADD MEMBER [sqlrepro-user];
GO
