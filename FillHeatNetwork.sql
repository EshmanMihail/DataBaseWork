USE HeatSchemeStorage

SET NOCOUNT ON;
DECLARE @Symbol CHAR(52) = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz',
	@Position INT,
	@k INT,
	@i INT,
	@NameLimit INT,
	@MinNumberSymbols INT,
	@MaxNumberSymbols INT,
	@NetworkName NVARCHAR(50),
	@NetworkNumber INT,
	@EnterpriseID INT,
	@NetworkType NVARCHAR(20);

-- Начало транзакции
BEGIN TRAN;

-- Заполнение таблицы HeatNetwork
SET @MinNumberSymbols = 5;
SET @MaxNumberSymbols = 30;

SET @i = 1;
WHILE @i <= 500 -- Заполнение 500 записей
BEGIN
	SET @NameLimit = @MinNumberSymbols + RAND() * (@MaxNumberSymbols - @MinNumberSymbols); -- Длина имени от 5 до 30 символов
	SET @NetworkName = '';
	SET @NetworkNumber = FLOOR(RAND() * 1000) + 1; -- Случайное число от 1 до 1000
	SET @NetworkType = CASE WHEN RAND() > 0.5 THEN 'Type A' ELSE 'Type B' END; -- Случайный выбор между 'Type A' и 'Type B'

	SET @Position = 1; -- Установка начального значения позиции
	WHILE @Position <= @NameLimit
	BEGIN
		SET @k = RAND() * 52;
		SET @NetworkName = @NetworkName + SUBSTRING(@Symbol, @k, 1);
		SET @Position = @Position + 1;
	END

	SET @EnterpriseID = FLOOR(RAND() * 500) + 1; -- Случайный выбор из существующих EnterpriseID

	INSERT INTO dbo.HeatNetwork (NetworkID, NetworkName, NetworkNumber, EnterpriseID, NetworkType)
	VALUES (@i, @NetworkName, @NetworkNumber, @EnterpriseID, @NetworkType);

	SET @i += 1; -- Увеличение переменной @i

END

-- Завершение транзакции
COMMIT;