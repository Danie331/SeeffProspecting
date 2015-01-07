CREATE TABLE [dbo].[log] (
    [pk_log_id]         INT          IDENTITY (1, 1) NOT NULL,
    [log_type]          VARCHAR (50) NOT NULL,
    [log_function_name] VARCHAR (50) NULL,
    [log_session_id]    VARCHAR (50) NULL,
    [log_description]   TEXT         NULL,
    [log_datetime]      DATETIME     NULL,
    CONSTRAINT [PK_log] PRIMARY KEY CLUSTERED ([pk_log_id] ASC)
);

