CREATE TABLE [dbo].[sps_hub_bank_product] (
    [product_id]       INT           IDENTITY (1, 1) NOT NULL,
    [bank]             VARCHAR (150) NOT NULL,
    [product]          VARCHAR (250) NOT NULL,
    [esp_invoice_code] VARCHAR (50)  NOT NULL
);

