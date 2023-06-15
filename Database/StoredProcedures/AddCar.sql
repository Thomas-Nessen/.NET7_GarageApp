USE [GarageDB]
GO

--IF OBJECT_ID('stp_AddName', 'P') IS NOT NULL DROP PROCEDURE [stp_AddName]
--GO

CREATE OR ALTER PROCEDURE stp_AddCar 
	@Name NVARCHAR(50),
	@CarID NVARCHAR(50),
	@ParkingSpot NVARCHAR(50)
	--@ParkInd BIT,
	--@StartParkTime DATETIME,
	--@EndParkTime DATETIME

AS
	SET NOCOUNT ON;
	DECLARE @retval INT = 0;
	DECLARE @ParkIND BIT = 1;
	DECLARE @StartParkTime DATETIME2 = CONVERT(DATETIME2(0), GETDATE()); 
	DECLARE @EndParkTime   DATETIME2 = CONVERT(DATETIME2(0), DATEADD(hh,2,GETDATE()))

	BEGIN TRY 
		BEGIN TRANSACTION 
		INSERT INTO [dbo].[Garage] (
			[name]
			,[car_id]
			,[parking_spot] 
			,[parked_ind]
			,[start_park_time] 
			,[end_park_time]
		)
		VALUES(
			@Name,
			@CarID,
			@ParkingSpot,
			@ParkInd,
			@StartParkTime,
			@EndParkTime
			);

		COMMIT TRANSACTION
		RETURN SCOPE_IDENTITY(); -- geeft ID van de inserted regel terug
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION -- draai terug tot voor de transactie
		SET @retval = -1  -- return value. laat me het weten als het mis is.
	END CATCH

	RETURN @retval;
GO

