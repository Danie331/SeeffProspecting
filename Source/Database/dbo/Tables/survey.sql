CREATE TABLE [dbo].[survey] (
    [survey_id]   INT           IDENTITY (1, 1) NOT NULL,
    [survey_name] VARCHAR (250) NOT NULL,
    [survey_desc] VARCHAR (MAX) NOT NULL,
    [start_date]  DATETIME      NOT NULL,
    [end_date]    DATETIME      NOT NULL,
    [remind]      BIT           NOT NULL
);

