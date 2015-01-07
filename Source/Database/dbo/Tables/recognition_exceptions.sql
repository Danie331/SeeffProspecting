CREATE TABLE [dbo].[recognition_exceptions] (
    [recognition_exception_id] INT           IDENTITY (1, 1) NOT NULL,
    [sps_transaction_id]       INT           NOT NULL,
    [error_discription]        VARCHAR (500) NOT NULL
);

