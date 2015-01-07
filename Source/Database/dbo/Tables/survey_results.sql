CREATE TABLE [dbo].[survey_results] (
    [survey_result_id]        INT           IDENTITY (1, 1) NOT NULL,
    [survey_id]               INT           NOT NULL,
    [survey_option_id]        INT           NOT NULL,
    [survey_option_values_id] INT           NOT NULL,
    [option_value]            VARCHAR (MAX) NOT NULL,
    [registration_id]         INT           NOT NULL
);

