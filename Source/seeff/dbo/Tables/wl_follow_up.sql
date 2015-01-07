CREATE TABLE [dbo].[wl_follow_up] (
    [pk_follow_up_id] INT           IDENTITY (1, 1) NOT NULL,
    [fk_enquiry_id]   INT           NOT NULL,
    [follow_up_date]  DATETIME      NOT NULL,
    [comment]         VARCHAR (500) NOT NULL,
    [time_stamp]      DATETIME      CONSTRAINT [DF_wl_follow_up_time_stamp] DEFAULT (getdate()) NOT NULL
);

