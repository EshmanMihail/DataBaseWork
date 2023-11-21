
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
    -- ���������� ������
    UPDATE HeatPoint
    SET PointName = @PointName,
        NetworkID = @NetworkID,
        NodeNumber = @NodeNumber
    WHERE PointID = @PointID;
  END
  ELSE
  BEGIN
    -- ������� ����� ������
    INSERT INTO HeatPoint (PointID, PointName, NetworkID, NodeNumber)
    VALUES (@PointID, @PointName, @NetworkID, @NodeNumber);
  END
END;