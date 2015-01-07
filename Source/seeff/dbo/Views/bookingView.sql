CREATE VIEW dbo.bookingView
AS
SELECT     dbo.bookings.*, dbo.bookingDates.bookingDateId AS bookingDateId, dbo.bookingDates.bookingDateIn AS dateIn, 
                      dbo.bookingDates.bookingDateOut AS dateOut, YEAR(dbo.bookingDates.bookingDateIn) AS dateInYear, MONTH(dbo.bookingDates.bookingDateIn) 
                      AS dateInMonth, DAY(dbo.bookingDates.bookingDateIn) AS dateInDay, YEAR(dbo.bookingDates.bookingDateOut) AS dateOutYear, 
                      MONTH(dbo.bookingDates.bookingDateOut) AS dateOutMonth, DAY(dbo.bookingDates.bookingDateOut) AS dateOutDay
FROM         dbo.bookings INNER JOIN
                      dbo.bookingDates ON dbo.bookings.bookingId = dbo.bookingDates.fkBookingId
