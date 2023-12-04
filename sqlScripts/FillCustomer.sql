USE HeatSchemeStorage

SET NOCOUNT ON;
DECLARE @Symbol CHAR(52) = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz',
	@Position INT,
	@i INT,
	@NameLimit INT,
	@RowCount INT,
	@MinNumberSymbols INT,
	@MaxNumberSymbols INT,
	@ConsumerName NVARCHAR(50),
	@NetworkID INT,
	@NodeNumber INT,
	@CalculatedPower DECIMAL;

-- Начало транзакции
BEGIN TRAN;

-- Заполнение таблицы HeatConsumer
SELECT @i = 0 FROM dbo.HeatConsumer WITH (TABLOCKX) WHERE 1 = 0;

SET @RowCount = 1;
SET @MinNumberSymbols = 5;
SET @MaxNumberSymbols = 30;

WHILE @RowCount <= 20000
BEGIN
	SET @NameLimit = @MinNumberSymbols + RAND() * (@MaxNumberSymbols - @MinNumberSymbols); -- Длина имени от 5 до 30 символов
	SET @i = 1;
	SET @ConsumerName = '';
	SET @NetworkID = @RowCount % 500 + 1;
	SET @NodeNumber = @RowCount;
	SET @CalculatedPower = RAND() * 1000;

	WHILE @i <= @NameLimit
	BEGIN
		SET @Position = RAND() * 52;
		SET @ConsumerName = @ConsumerName + SUBSTRING(@Symbol, @Position, 1);

		SET @i = @i + 1;
	END

	INSERT INTO dbo.HeatConsumer (ConsumerID, ConsumerName, NetworkID, NodeNumber, CalculatedPower)
	VALUES (@RowCount, @ConsumerName, @NetworkID, @NodeNumber, @CalculatedPower);

	SET @RowCount += 1;
END

-- Завершение транзакции
COMMIT;