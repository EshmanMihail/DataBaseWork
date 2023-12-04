
USE HeatSchemeStorage
go

CREATE PROCEDURE InsertOrUpdateHeatPoint
  @PointID INT,
  @PointName NVARCHAR(50),
  @NetworkID INT,
  @NodeNumber INT
AS
BEGIN
  IF EXISTS (SELECT 1 FROM HeatPoint WHERE PointID = @PointID)
  BEGIN
    -- Обновление данных
    UPDATE HeatPoint
    SET PointName = @PointName,
        NetworkID = @NetworkID,
        NodeNumber = @NodeNumber
    WHERE PointID = @PointID;
  END
  ELSE
  BEGIN
    -- Вставка новых данных
    INSERT INTO HeatPoint (PointID, PointName, NetworkID, NodeNumber)
    VALUES (@PointID, @PointName, @NetworkID, @NodeNumber);
  END
END;