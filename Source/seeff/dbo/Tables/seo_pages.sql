CREATE TABLE [dbo].[seo_pages] (
    [pk_seo_page_id]            INT           IDENTITY (1, 1) NOT NULL,
    [seo_page_region]           VARCHAR (255) NULL,
    [seo_page_url]              VARCHAR (255) NULL,
    [seo_page_copy]             TEXT          NULL,
    [seo_page_image]            VARCHAR (255) NULL,
    [seo_page_areaId]           INT           NULL,
    [seo_page_branchId]         INT           NULL,
    [seo_page_golfestateId]     INT           NULL,
    [seo_page_meta_title]       TEXT          NULL,
    [seo_page_meta_description] TEXT          NULL,
    [seo_page_meta_keywords]    TEXT          NULL,
    [seo_page_copy_additional]  TEXT          NULL,
    [seo_rental_page]           VARCHAR (1)   DEFAULT ('0') NOT NULL,
    [seo_image_title]           VARCHAR (100) NULL,
    [seo_image_alt]             VARCHAR (100) NULL,
    CONSTRAINT [UQ__seo_pages__475C8B58] UNIQUE NONCLUSTERED ([pk_seo_page_id] ASC) WITH (FILLFACTOR = 90)
);

