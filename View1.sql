-- Представление для отображения информации о тепловых сетях с указанием названия предприятия
CREATE VIEW ViewHeatNetworks AS
SELECT hn.NetworkID, hn.NetworkName, hn.NetworkNumber, e.EnterpriseName, hn.NetworkType
FROM HeatNetwork hn
INNER JOIN Enterprise e ON hn.EnterpriseID = e.EnterpriseID;