USE HeatSchemeStorage

SET NOCOUNT ON;
DECLARE @RowCount INT = 1,
	@MinDiameter DECIMAL(10,2) = 10.0,
	@MaxDiameter DECIMAL(10,2) = 50.0,
	@MinThickness DECIMAL(10,2) = 1.0,
	@MaxThickness DECIMAL(10,2) = 10.0,
	@MinLength DECIMAL(10,2) = 100.0,
	@MaxLength DECIMAL(10,2) = 1000.0,
	@StartDate DATE = '2000-01-01',
	@EndDate DATE = '2023-12-31',
	@HeatPointCount INT;

-- Получение количества записей в таблице HeatPoint
SELECT @HeatPointCount = COUNT(*) FROM HeatPoint;

-- Начало транзакции
BEGIN TRAN;

WHILE @RowCount <= 20000
BEGIN
	DECLARE @StartNodeNumber INT,
		@EndNodeNumber INT,
		@Diameter DECIMAL(10,2),
		@Thickness DECIMAL(10,2),
		@Length DECIMAL(10,2);

	-- Генерация случайных значений для внешних ключей StartNodeNumber и EndNodeNumber
	SELECT @StartNodeNumber = NodeNumber FROM HeatPoint WHERE PointID = FLOOR(1 + RAND() * @HeatPointCount);
	SELECT @EndNodeNumber = NodeNumber FROM HeatPoint WHERE PointID = FLOOR(1 + RAND() * @HeatPointCount);

	-- Генерация случайных значений для диаметра, толщины и длины
	SET @Diameter = @MinDiameter + RAND() * (@MaxDiameter - @MinDiameter);
	SET @Thickness = @MinThickness + RAND() * (@MaxThickness - @MinThickness);
	SET @Length = @MinLength + RAND() * (@MaxLength - @MinLength);

	-- Генерация случайной даты между @StartDate и @EndDate
	DECLARE @RandomDate DATE = DATEADD(DAY, CAST((RAND() * DATEDIFF(DAY, @StartDate, @EndDate)) AS INT), @StartDate);

	-- Вставка данных в таблицу PipelineSection
	INSERT INTO PipelineSection (SectionID, SectionNumber, StartNodeNumber, EndNodeNumber, PipelineLength, Diameter, Thickness, LastRepairDate)
	VALUES (@RowCount, @RowCount, @StartNodeNumber, @EndNodeNumber, @Length, @Diameter, @Thickness, @RandomDate);
	
	SET @RowCount += 1;
END

-- Завершение транзакции
COMMIT;