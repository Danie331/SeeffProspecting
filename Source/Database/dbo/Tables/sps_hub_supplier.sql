CREATE TABLE [dbo].[sps_hub_supplier] (
    [supplier_id]      INT           IDENTITY (1, 1) NOT NULL,
    [supplier_name]    VARCHAR (250) NOT NULL,
    [supplier_address] VARCHAR (250) NULL,
    [supplier_city]    VARCHAR (150) NULL,
    [supplier_code]    VARCHAR (10)  NULL,
    [supplier_contact] VARCHAR (50)  NULL,
    [supplier_vat_no]  VARCHAR (50)  NULL,
    [Attention]        VARCHAR (250) NULL
);

