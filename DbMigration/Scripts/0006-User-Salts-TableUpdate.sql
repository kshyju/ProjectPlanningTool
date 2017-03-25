IF NOT EXISTS ( SELECT  *
                FROM    sys.columns
                WHERE   object_id = OBJECT_ID(N'[dbo].[User]')
                        AND name = 'Salt' )
    BEGIN

        ALTER TABLE [dbo].[User] ADD Salt VARBINARY(30) NULL;  

    END;