CREATE TABLE [dbo].[product] (
    [product_id]        INT           IDENTITY (1, 1) NOT NULL,
    [product_file_name] VARCHAR (250) NOT NULL,
    [product_dir]       VARCHAR (250) NOT NULL,
    [product_version]   VARCHAR (250) NOT NULL,
    [product_action]    VARCHAR (50)  NOT NULL,
    [product_date]      DATETIME      NOT NULL
);

