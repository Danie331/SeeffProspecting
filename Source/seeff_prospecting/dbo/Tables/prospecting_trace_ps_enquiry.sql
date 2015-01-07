CREATE TABLE [dbo].[prospecting_trace_ps_enquiry] (
    [prospecting_trace_ps_enquiry_id] INT              IDENTITY (1, 1) NOT NULL,
    [prospecting_property_id]         INT              NOT NULL,
    [user]                            UNIQUEIDENTIFIER NOT NULL,
    [date_of_enquiry]                 DATETIME         NOT NULL,
    [successful]                      BIT              NOT NULL,
    [id_number]                       VARCHAR (13)     DEFAULT ('xxxxxxxxxxxxx') NOT NULL,
    PRIMARY KEY CLUSTERED ([prospecting_trace_ps_enquiry_id] ASC),
    CONSTRAINT [FK__prospecti__prosp__49C3F6B7] FOREIGN KEY ([prospecting_property_id]) REFERENCES [dbo].[prospecting_property] ([prospecting_property_id])
);

