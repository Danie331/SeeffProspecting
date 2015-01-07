CREATE TABLE [dbo].[smart_pass_digest] (
    [digest_id]       BIGINT IDENTITY (1, 1) NOT NULL,
    [smart_pass_id]   INT    NOT NULL,
    [registration_id] INT    NOT NULL,
    [processed]       BIT    CONSTRAINT [DF_smart_pass_digest_processed] DEFAULT ((0)) NOT NULL
);

