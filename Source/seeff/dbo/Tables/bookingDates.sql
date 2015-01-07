CREATE TABLE [dbo].[bookingDates] (
    [bookingDateId]       BIGINT       IDENTITY (1, 1) NOT NULL,
    [fkBookingId]         BIGINT       NULL,
    [bookingDateIn]       DATETIME     NULL,
    [bookingDateOut]      DATETIME     NULL,
    [fkPropertyReference] NUMERIC (18) NULL,
    CONSTRAINT [PK_bookingDates] PRIMARY KEY CLUSTERED ([bookingDateId] ASC)
);

