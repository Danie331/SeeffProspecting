CREATE TABLE [dbo].[prospecting_property] (
    [prospecting_property_id] INT              IDENTITY (1, 1) NOT NULL,
    [licence_id]              INT              NULL,
    [seeff_area_id]           INT              NULL,
    [development_id]          INT              NULL,
    [lightstone_property_id]  INT              NULL,
    [propstats_id]            INT              NULL,
    [windeed_id]              INT              NULL,
    [erf_no]                  INT              NULL,
    [portion_no]              INT              NULL,
    [property_address]        VARCHAR (255)    NULL,
    [street_or_unit_no]       VARCHAR (255)    NULL,
    [photo_url]               NVARCHAR (256)   NULL,
    [latitude]                DECIMAL (13, 8)  NULL,
    [longitude]               DECIMAL (13, 8)  NULL,
    [age]                     DATETIME         NULL,
    [erf_size]                INT              NULL,
    [dwell_size]              INT              NULL,
    [condition]               VARCHAR (16)     NULL,
    [beds]                    INT              NULL,
    [baths]                   INT              NULL,
    [receptions]              INT              NULL,
    [studies]                 INT              NULL,
    [garages]                 INT              NULL,
    [parking_bays]            INT              NULL,
    [pool]                    BIT              NULL,
    [staff_accomodation]      BIT              NULL,
    [created_date]            DATETIME         CONSTRAINT [DF_prospecting_property_created_date] DEFAULT (getdate()) NULL,
    [updated_date]            DATETIME         NULL,
    [created_by]              UNIQUEIDENTIFIER NULL,
    [lightstone_id_or_ck_no]  VARCHAR (255)    NULL,
    [lightstone_reg_date]     VARCHAR (8)      NULL,
    [comments]                VARCHAR (MAX)    NULL,
    [ss_name]                 VARCHAR (255)    NULL,
    [ss_number]               VARCHAR (50)     NULL,
    [unit]                    VARCHAR (50)     NULL,
    [ss_fh]                   VARCHAR (2)      NULL,
    [last_purch_price]        DECIMAL (18, 2)  NULL,
    [ss_id]                   VARCHAR (10)     NULL,
    [ss_door_number]          VARCHAR (50)     NULL,
    [prospected]              BIT              CONSTRAINT [DF_prospecting_property_prospected] DEFAULT ((0)) NULL,
    CONSTRAINT [PK__prospect__8E5D8F4FDB723D77] PRIMARY KEY CLUSTERED ([prospecting_property_id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20141107-200521]
    ON [dbo].[prospecting_property]([lightstone_property_id] ASC);

