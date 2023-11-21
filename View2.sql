-- ѕредставление дл€ отображени€ информации о потребител€х тепла с указанием названи€ тепловой сети
CREATE VIEW ViewHeatConsumers AS
SELECT hc.ConsumerID, hc.ConsumerName, hn.NetworkName, hc.NodeNumber, hc.CalculatedPower
FROM HeatConsumer hc
INNER JOIN HeatNetwork hn ON hc.NetworkID = hn.NetworkID;