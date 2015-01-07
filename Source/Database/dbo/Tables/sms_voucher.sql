CREATE TABLE [dbo].[sms_voucher] (
    [voucher_id]      INT           IDENTITY (1, 1) NOT NULL,
    [voucher_no]      VARCHAR (50)  NOT NULL,
    [voucher_text]    VARCHAR (160) NOT NULL,
    [registration_id] INT           NOT NULL,
    [redeem_count]    INT           NOT NULL
);

