-- ѕредставление дл€ отображени€ информации об участках трубопровода с указанием начального и конечного узлов, а также названи€ тепловой сети
CREATE VIEW ViewPipelineSections AS
SELECT ps.SectionID, ps.SectionNumber, hp1.PointName AS StartNode, hp2.PointName AS EndNode, ps.PipelineLength, ps.Diameter, ps.Thickness, ps.LastRepairDate
FROM PipelineSection ps
INNER JOIN HeatPoint hp1 ON ps.StartNodeNumber = hp1.PointID
INNER JOIN HeatPoint hp2 ON ps.EndNodeNumber = hp2.PointID;