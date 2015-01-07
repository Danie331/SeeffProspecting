CREATE TABLE [dbo].[smart_acc_batch] (
    [smart_acc_batch_id]      INT             IDENTITY (1, 1) NOT NULL,
    [smart_acc_batch_no]      BIGINT          NOT NULL,
    [smart_acc_units]         INT             NOT NULL,
    [smart_acc_usd_cost]      DECIMAL (18, 2) NOT NULL,
    [smart_acc_purchase_date] DATETIME        NOT NULL,
    [smart_acc_zar_cost]      DECIMAL (18, 2) NULL,
    [smart_acc_unit_cost]     DECIMAL (18, 2) NULL,
    [smart_acc_note]          VARCHAR (200)   NULL,
    [smart_acc_status]        VARCHAR (50)    NULL
);

