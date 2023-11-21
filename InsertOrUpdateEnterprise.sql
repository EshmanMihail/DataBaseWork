CREATE PROCEDURE InsertOrUpdateEnterprise
  @EnterpriseID INT,
  @EnterpriseName NVARCHAR(50),
  @ManagementOrganization NVARCHAR(50)
AS
BEGIN
  IF EXISTS (SELECT 1 FROM Enterprise WHERE EnterpriseID = @EnterpriseID)
  BEGIN
    -- Обновление данных
    UPDATE Enterprise
    SET EnterpriseName = @EnterpriseName,
        ManagementOrganization = @ManagementOrganization
    WHERE EnterpriseID = @EnterpriseID;
  END
  ELSE
  BEGIN
    -- Вставка новых данных
    INSERT INTO Enterprise (EnterpriseID, EnterpriseName, ManagementOrganization)
    VALUES (@EnterpriseID, @EnterpriseName, @ManagementOrganization);
  END
END;