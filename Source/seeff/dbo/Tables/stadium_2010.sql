CREATE TABLE [dbo].[stadium_2010] (
    [pk_stadium_2010_id]   INT           IDENTITY (1, 1) NOT NULL,
    [fk_city_2010_id]      INT           NULL,
    [stadium_2010_name]    NVARCHAR (50) NULL,
    [stadium_2010_page]    VARCHAR (100) NULL,
    [stadium_2010_active]  BIT           NULL,
    [stadium_2010_deleted] BIT           NULL
);

