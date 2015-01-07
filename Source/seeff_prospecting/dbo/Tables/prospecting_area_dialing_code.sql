CREATE TABLE [dbo].[prospecting_area_dialing_code] (
    [prospecting_area_dialing_code_id] INT           IDENTITY (1, 1) NOT NULL,
    [dialing_code_id]                  INT           NOT NULL,
    [code_desc]                        VARCHAR (100) NOT NULL,
    PRIMARY KEY CLUSTERED ([prospecting_area_dialing_code_id] ASC)
);

