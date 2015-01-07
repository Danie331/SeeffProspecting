CREATE TABLE [dbo].[sps_sbf_split] (
    [exception_id] INT           IDENTITY (1, 1) NOT NULL,
    [license_id]   INT           NOT NULL,
    [branch_id]    INT           NOT NULL,
    [start_date]   DATETIME      NOT NULL,
    [end_date]     DATETIME      NOT NULL,
    [split_name]   VARCHAR (250) NOT NULL
);

