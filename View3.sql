-- ѕредставление дл€ отображени€ информации о тепловых пунктах с указанием названи€ тепловой сети
CREATE VIEW ViewHeatPoints AS
SELECT hp.PointID, hp.PointName, hn.NetworkName, hp.NodeNumber
FROM HeatPoint hp
INNER JOIN HeatNetwork hn ON hp.NetworkID = hn.NetworkID;