USE ScmDemand

/* Primary Key Columns cursor ************/
IF EXISTS(SELECT name FROM sysobjects
WHERE name = 'PrimaryKeys' AND type = 'P')
	drop procedure PrimaryKeys
GO

CREATE PROCEDURE PrimaryKeys(@table_name varchar(255))
AS
BEGIN	
	declare	@tableupper as varchar(255)
	select @tableupper = upper(@table_name)
	DECLARE lc_primary SCROLL CURSOR FOR
	SELECT col.name, col.id, col.xtype, col.length
	from syscolumns col, sysobjects tab, sysindexkeys ind
	where tab.id = col.id
	and tab.xtype = 'U'
	and tab.name = @tableupper
	and ind.id = tab.id
	and ind.colid = col.colid
	and (ind.indid = 1 or ind.indid = 2)
		


	OPEN lc_primary

	DECLARE @ls_name as varchar(255)
	DECLARE @li_colid as int
	DECLARE @li_xtype as int
	DECLARE @li_length as int
	DECLARE @temp_print as varchar(255)

	
	print '	}'
	print '	#endregion'
	
	/*FETCH FIRST FROM colcursor INTO @column_name, @column_type
	WHILE @@fetch_status = 0 begin
		IF @@fetch_status = 0
		BEGIN*/
	CLOSE lc_primary
	DEALLOCATE lc_primary
END

GO


/*procedure which generates Colums Array*/
IF EXISTS(SELECT name FROM sysobjects
WHERE name = 'CreateColumns' AND type = 'P')
	drop procedure CreateColumns
GO

CREATE PROCEDURE CreateColumns
AS
BEGIN
	DECLARE @column_name as varchar(255)	
	DECLARE @column_type as int
	DECLARE @return_val as int
	DECLARE @column_name_add as varchar(255)
	DECLARE @csharp_type as varchar(255)
	DECLARE @temp_print as varchar(255)
	
	print '		DataColumn[] tableColumns = new DataColumn[] {'
	FETCH FIRST FROM colcursor INTO @column_name, @column_type
	WHILE @@fetch_status = 0 begin
		select @column_name_add = lower(@column_name)
		exec @return_val = RemoveUnderscore @column_name_add OUTPUT
		EXECUTE GetType @column_type, @csharp_type OUTPUT
		
		SELECT @temp_print = '			new DataColumn("' + @column_name + '", System.Type.GetType('	
		
		IF @csharp_type = 'int' 
		BEGIN
			SELECT @temp_print = @temp_print + '"System.Int32"'
		END ELSE IF @csharp_type = 'string' 
		BEGIN
			SELECT @temp_print = @temp_print + '"System.String"'
		END ELSE IF @csharp_type = 'decimal' 
		BEGIN
			SELECT @temp_print = @temp_print + '"System.Double"'
		END ELSE IF @csharp_type = 'bool' 
		BEGIN
			SELECT @temp_print = @temp_print + '"System.Boolean"'
		END ELSE IF @csharp_type = 'datetime' 
		BEGIN
			SELECT @temp_print = @temp_print + '"System.DateTime"'
		END ELSE IF @csharp_type = 'byte[]' 
		BEGIN
			SELECT @temp_print = @temp_print + '"System.Byte[]"'
		END				
		
		SELECT @temp_print = @temp_print + '))'		

		FETCH NEXT FROM colcursor INTO @column_name, @column_type

		IF @@fetch_status = 0
		BEGIN
				select @temp_print = @temp_print + ','
		END
			
		PRINT @temp_print
		
	END	
	print '		};'
END

GO

/*procedure which generates AddNewRow method*/
IF EXISTS(SELECT name FROM sysobjects
WHERE name = 'AddNewRow' AND type = 'P')
	drop procedure AddNewRow
GO

CREATE PROCEDURE AddNewRow
AS
BEGIN
	DECLARE @column_name as varchar(255)	
	DECLARE @column_type as int
	DECLARE @return_val as int
	DECLARE @column_name_add as varchar(255)
	DECLARE @csharp_type as varchar(255)
	DECLARE @temp_print as varchar(255)
	
	print '		public void AddRow('
	FETCH FIRST FROM colcursor INTO @column_name, @column_type
	WHILE @@fetch_status = 0 begin
		/*FETCH NEXT FROM colcursor INTO @column_name, @column_type*/
		/*IF @@fetch_status = 0*/
		/*BEGIN*/
				
			select @column_name_add = lower(@column_name)
			exec @return_val = RemoveUnderscore @column_name_add OUTPUT
			EXECUTE GetType @column_type, @csharp_type OUTPUT
			/*FETCH NEXT FROM colcursor INTO @column_name, @column_type*/
			select @temp_print = '				' + @csharp_type + ' ' + @column_name_add
			
		/*END*/
		FETCH NEXT FROM colcursor INTO @column_name, @column_type
		IF @@fetch_status = 0
			BEGIN
				select @temp_print = @temp_print + ','
			END
		ELSE
			BEGIN
				select @temp_print = @temp_print + ') {'
			END
		PRINT @temp_print
	END	
	print '			DataRow row = this.NewRow();'
	
	FETCH FIRST FROM colcursor INTO @column_name, @column_type
	WHILE @@fetch_status = 0 begin
		IF @@fetch_status = 0
		BEGIN
			SELECT @column_name_add = lower(@column_name)
			EXECUTE @return_val = RemoveUnderscore @column_name_add OUTPUT
			PRINT '			Set' + upper(left(@column_name_add, 1)) + right(@column_name_add, len(@column_name_add) - 1) + '(row, ' + @column_name_add + ');'
		END
		FETCH NEXT FROM colcursor INTO @column_name, @column_type
	END	

	print '			this.Tables[0].Rows.Add(row);'	

	print '		}'
END

GO

/*            Upper First
***************************************************************/
if exists (select 1
            from  sysobjects
           where  id = object_id('UpperFirst')
            and   type = 'P')
	drop procedure UpperFirst
go
create procedure UpperFirst
	(@ps_name varchar(255) OUTPUT)
as
begin
	select @ps_name = upper(left(@ps_name, 1)) + right(@ps_name, len(@ps_name) - 1)
	return 0
end
go

/*            Remove Underscore
***************************************************************/
if exists (select 1
            from  sysobjects
           where  id = object_id('RemoveUnderscore')
            and   type = 'P')
	drop procedure RemoveUnderscore
go
create procedure RemoveUnderscore
	(@ps_name varchar(255) OUTPUT)
as
begin
	declare @out_name as varchar(255),
		@li_index as int

	select @li_index = charindex('_', @ps_name, 1)
	select @out_name = @ps_name

	while (@li_index <> 0) begin
		select @out_name = left(@out_name, @li_index - 1) + 
			upper(substring(@out_name, @li_index + 1, 1)) +
			right(@out_name, len(@out_name) - @li_index - 1)
		select @li_index = charindex('_', @out_name, @li_index + 1)
	end

	select @ps_name = @out_name
	return 0
end
go

/*procedure finds out field type*/
IF EXISTS(SELECT name FROM sysobjects
WHERE name = 'GetType' AND type = 'P')
	drop procedure GetType
GO

CREATE PROCEDURE GetType(@column_type as int, @return_type as varchar(255) OUTPUT)
AS
BEGIN
	declare @type_name as varchar(255)

	select @type_name = typ.name
	from syscolumns col, systypes typ
	where col.xusertype = typ.xusertype
	/*and col.id = @pi_colid*/
	and col.xtype = @column_type
	IF @type_name = 'sysname' OR
		@type_name = 'char' OR
		@type_name = 'varchar' OR
		@type_name = 'text' OR
		@type_name = 'nchar' OR
		@type_name = 'nvarchar' OR
		@type_name = 'ntext' 
		
	BEGIN
		SELECT @return_type = 'string'
	END ELSE IF @type_name = 'int' OR
			@type_name = 'smallint' OR
			@type_name = 'tinyint'
	BEGIN
		SELECT @return_type = 'int'
	END ELSE IF @type_name = 'bit' 
	BEGIN
		SELECT @return_type = 'bool'
	END ELSE IF @type_name = 'decimal' OR
			@type_name = 'numeric' OR
			@type_name = 'float' OR
			@type_name = 'real'
	BEGIN
		SELECT @return_type = 'decimal'
	END ELSE IF @type_name = 'datetime' OR
			@type_name = 'smalldatetime'
	BEGIN
		SELECT @return_type = 'DateTime'
	END ELSE IF @type_name = 'binary' OR
		@type_name = 'varbinary' OR
		@type_name = 'image'
	BEGIN
		SELECT @return_type = 'byte[]'
	END
END
GO

--Field length
IF EXISTS(SELECT name FROM sysobjects
WHERE name = 'GetFieldLength' AND type = 'P')
	drop procedure GetFieldLength
GO

CREATE PROCEDURE GetFieldLength(@table_name_par varchar(255))
AS
	BEGIN

	DECLARE lc_fieldlength SCROLL CURSOR FOR
	SELECT col.name, col.xtype, col.length
	FROM syscolumns col, sysobjects tab
	WHERE tab.id = col.id and tab.xtype = 'U' and tab.name = @table_name_par
	

	OPEN lc_fieldlength

	DECLARE @column_name as varchar(255)	
	DECLARE @column_type as int
	DECLARE @column_length as smallint

	print ''
	
	FETCH FIRST FROM lc_fieldlength INTO @column_name, @column_type, @column_length
	WHILE @@fetch_status = 0 begin
		IF @@fetch_status = 0
		BEGIN
			declare @type_name as varchar(255)

			select @type_name = typ.name
			from syscolumns col, systypes typ
			where col.xusertype = typ.xusertype
			/*and col.id = @pi_colid*/
			and col.xtype = @column_type

			IF @type_name = 'sysname' OR
				@type_name = 'nchar' OR
				@type_name = 'nvarchar' 
			BEGIN
						
				print '		public const int ' + upper(@column_name) + '_LENGTH = ' + RTRIM(LTRIM(Str(@column_length/2))) + ';'
			END

			IF @type_name = 'sysname' OR
				@type_name = 'char' OR
				@type_name = 'varchar' 
				
			BEGIN
						
				print '		public const int ' + upper(@column_name) + '_LENGTH = ' + RTRIM(LTRIM(Str(@column_length))) + ';'
			END
		END
		FETCH NEXT FROM lc_fieldlength INTO @column_name, @column_type, @column_length
	END

	DEALLOCATE lc_fieldlength
END
GO

/*procedure which generates table clasess*/
IF EXISTS(SELECT name FROM sysobjects
WHERE name = 'GenerateTableClass' AND type = 'P')
	drop procedure GenerateTableClass
GO

CREATE PROCEDURE GenerateTableClass(@table_name_par varchar(255))
AS
BEGIN
	/*EXECUTE GenerateClassHead @table_name_par*/
	
	/* all columns */
	
	DECLARE colcursor SCROLL CURSOR FOR
	SELECT col.name, col.xtype
	FROM syscolumns col, sysobjects tab
	WHERE tab.id = col.id and tab.xtype = 'U' and tab.name = @table_name_par
	OPEN colcursor
	
	DECLARE @column_name as varchar(255)	
	DECLARE @column_type as int
	DECLARE @csharp_type as varchar(255)
	DECLARE @class_name as varchar(255)	
	DECLARE @column_name_s as varchar(255)
	DECLARE @csharp_type_string as varchar(255)	
	DECLARE @return as int	

	select @class_name = lower(@table_name_par)	
	exec @return = RemoveUnderscore @class_name OUTPUT
	exec @return = UpperFirst @class_name OUTPUT

	
	print '	#region ' + @class_name
	print '	public class ' + @class_name + 'Data'
	print '	{'
	print ''
	print '		#region Constants'
	print '		public const string TABLE_NAME = "' + upper(@table_name_par) + '";'
	

	
	FETCH FIRST FROM colcursor INTO @column_name, @column_type
	WHILE @@fetch_status = 0 begin
		IF @@fetch_status = 0
		BEGIN
			print '		public const string ' + upper(@column_name) + '_FIELD = "' + @column_name + '";'
		END
		FETCH NEXT FROM colcursor INTO @column_name, @column_type
	END

	EXECUTE GetFieldLength @table_name_par

	print '		#endregion'

	EXECUTE PrimaryKeys @table_name_par

		

	
	print ''

	CLOSE colcursor
	DEALLOCATE colcursor
	
END
GO


/* Main program */
print '//*************************************************************************************'
print '//* This classes were generated by ScmDemandDdGenerator.sql script                          *'
print '//*************************************************************************************'
 
print 'using System;'
print ''
print 'namespace OTISCZ.ScmDemand.Model.DataDictionary'
print '{'


DECLARE @debug_print varchar(30)


/* All Tables */
DECLARE all_user_tables SCROLL CURSOR FOR
/*select table_name field*/

SELECT tables.name
FROM sysobjects tables WHERE tables.xtype = 'U' AND upper(tables.name) <> 'DTPROPERTIES' AND upper(tables.name) <> 'SYS%'/*''U' user tables */

OPEN all_user_tables

DECLARE @table_name as varchar(255)
FETCH FIRST FROM all_user_tables INTO @table_name
WHILE @@fetch_status = 0 
BEGIN
	EXECUTE GenerateTableClass @table_name
	/*print '//================================================================================='*/
	FETCH NEXT FROM all_user_tables INTO @table_name
	/*
	print 'after fetch'
	print 'status =' + STR(@@fetch_status)
	IF @@fetch_status = 0
	BEGIN
		print 'if'
		print @table_name
	END*/
END

print '}'

CLOSE all_user_tables
DEALLOCATE all_user_tables
