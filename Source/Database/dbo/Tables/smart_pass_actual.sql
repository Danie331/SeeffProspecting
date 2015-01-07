CREATE TABLE [dbo].[smart_pass_actual] (
    [smart_pass_id]     INT             NOT NULL,
    [referral_type]     VARCHAR (20)    NOT NULL,
    [created_by]        INT             NOT NULL,
    [created_date]      DATETIME        NOT NULL,
    [updated_by]        INT             NOT NULL,
    [updated_date]      DATETIME        NOT NULL,
    [value_from]        DECIMAL (18, 2) NULL,
    [value_to]          DECIMAL (18, 2) NULL,
    [no_beds]           INT             NULL,
    [no_baths]          INT             NULL,
    [property_id]       INT             NULL,
    [property_desc]     VARCHAR (350)   NULL,
    [property_geo_code] VARCHAR (50)    NULL,
    [division]          VARCHAR (50)    NULL,
    [category]          VARCHAR (50)    NULL,
    [area]              VARCHAR (250)   NULL,
    [score]             INT             NULL,
    CONSTRAINT [PK_smart_pass_actual] PRIMARY KEY CLUSTERED ([smart_pass_id] ASC)
);

