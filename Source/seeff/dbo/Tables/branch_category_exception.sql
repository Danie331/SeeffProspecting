CREATE TABLE [dbo].[branch_category_exception] (
    [id]           INT      IDENTITY (1, 1) NOT NULL,
    [fk_branch_id] INT      NULL,
    [in_type]      CHAR (1) NULL,
    [out_type]     CHAR (1) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);

