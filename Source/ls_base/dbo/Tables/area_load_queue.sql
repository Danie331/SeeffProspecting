CREATE TABLE [dbo].[area_load_queue] (
    [queue_id]   INT      IDENTITY (1, 1) NOT NULL,
    [area_id]    INT      NOT NULL,
    [load_start] DATETIME NULL,
    [load_end]   DATETIME NULL
);

