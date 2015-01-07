CREATE TABLE [dbo].[pdf_fact_sheet] (
    [pk_pdf_fact_sheet_id]        INT           IDENTITY (1, 1) NOT NULL,
    [pdf_fact_sheet_title]        VARCHAR (150) NULL,
    [pdf_fact_sheet_filename]     VARCHAR (150) NULL,
    [fk_category_id]              INT           NULL,
    [pdf_fact_sheet_date_added]   DATETIME      NULL,
    [pdf_fact_sheet_date_updated] DATETIME      NULL,
    [pdf_fact_sheet_active]       TINYINT       CONSTRAINT [DF_pdf_fact_sheet_pdf_fact_sheet_active] DEFAULT (0) NULL,
    [pdf_fact_sheet_deleted]      TINYINT       CONSTRAINT [DF_pdf_fact_sheet_pdf_fact_sheet_deleted] DEFAULT (0) NULL,
    CONSTRAINT [PK_pdf_fact_sheet] PRIMARY KEY CLUSTERED ([pk_pdf_fact_sheet_id] ASC) WITH (FILLFACTOR = 90)
);

