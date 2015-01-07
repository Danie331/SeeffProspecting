CREATE TABLE [dbo].[province_id_lookup] (
    [province_id_lookup]   INT           IDENTITY (1, 1) NOT NULL,
    [lightstone_prov_name] VARCHAR (MAX) NOT NULL,
    [seeff_prov_name]      VARCHAR (MAX) NOT NULL,
    [seeff_area_id]        INT           NOT NULL,
    PRIMARY KEY CLUSTERED ([province_id_lookup] ASC)
);

