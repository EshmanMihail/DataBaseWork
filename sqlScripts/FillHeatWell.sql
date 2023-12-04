USE HeatSchemeStorage

SET NOCOUNT ON;
DECLARE @Symbol CHAR(52) = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz',
	@Position INT,
	@i INT,
	@NameLimit INT,
	@RowCount INT,
	@MinNumberSymbols INT,
	@MaxNumberSymbols INT,
	@WellName NVARCHAR(50),
	@NetworkID INT,
	@NodeNumber INT;

-- Начало транзакции
BEGIN TRAN;

-- Заполнение таблицы HeatWell
SELECT @i = 0 FROM dbo.HeatWell WITH (TABLOCKX) WHERE 1 = 0;

SET @RowCount = 1;
SET @MinNumberSymbols = 5;
SET @MaxNumberSymbols = 30;

WHILE @RowCount <= 20000
BEGIN
	SET @NameLimit = @MinNumberSymbols + RAND() * (@MaxNumberSymbols - @MinNumberSymbols); -- Длина имени от 5 до 30 символов
	SET @i = 1;
	SET @WellName = '';
	SET @NetworkID = @RowCount % 500 + 1;
	SET @NodeNumber = @RowCount;

	WHILE @i <= @NameLimit
	BEGIN
		SET @Position = RAND() * 52;
		SET @WellName = @WellName + SUBSTRING(@Symbol, @Position, 1);

		SET @i = @i + 1;
	END

	INSERT INTO dbo.HeatWell (WellID, WellName, NetworkID, NodeNumber)
	VALUES (@RowCount, @WellName, @NetworkID, @NodeNumber);

	SET @RowCount += 1;
END

-- Завершение транзакции
COMMIT;