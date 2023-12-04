USE HeatSchemeStorage
go
CREATE PROCEDURE InsertOrUpdateHeatNetwork
  @NetworkID INT,
  @NetworkName NVARCHAR(50),
  @NetworkNumber INT,
  @EnterpriseID INT,
  @NetworkType NVARCHAR(20)
AS
BEGIN
  IF EXISTS (SELECT 1 FROM HeatNetwork WHERE NetworkID = @NetworkID)
  BEGIN
    -- Обновление данных
    UPDATE HeatNetwork
    SET NetworkName = @NetworkName,
        NetworkNumber = @NetworkNumber,
        EnterpriseID = @EnterpriseID,
        NetworkType = @NetworkType
    WHERE NetworkID = @NetworkID;
  END
  ELSE
  BEGIN
    -- Вставка новых данных
    INSERT INTO HeatNetwork (NetworkID, NetworkName, NetworkNumber, EnterpriseID, NetworkType)
    VALUES (@NetworkID, @NetworkName, @NetworkNumber, @EnterpriseID, @NetworkType);
  END
END;