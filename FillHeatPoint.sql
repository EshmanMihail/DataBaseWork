USE HeatSchemeStorage

SET NOCOUNT ON;
DECLARE @Symbol CHAR(52) = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz',
	@Position INT,
	@i INT,
	@NameLimit INT,
	@RowCount INT,
	@MinNumberSymbols INT,
	@MaxNumberSymbols INT,
	@PointName NVARCHAR(50),
	@NetworkID INT,
	@NodeNumber INT;

-- ������ ����������
BEGIN TRAN;

-- ���������� ������� HeatPoint
SELECT @i = 0 FROM dbo.HeatPoint WITH (TABLOCKX) WHERE 1 = 0;

SET @RowCount = 1;
SET @MinNumberSymbols = 5;
SET @MaxNumberSymbols = 30;

WHILE @RowCount <= 20000 -- ���������� 500 �������
BEGIN
	SET @NameLimit = @MinNumberSymbols + RAND() * (@MaxNumberSymbols - @MinNumberSymbols); -- ����� ����� �� 5 �� 30 ��������
	SET @i = 1;
	SET @PointName = '';
	SET @NetworkID = @RowCount % 500 + 1; -- ��������� ������������� NetworkID �� 1 �� 500
	SET @NodeNumber = @RowCount;

	WHILE @i <= @NameLimit
	BEGIN
		SET @Position = RAND() * 52;
		SET @PointName = @PointName + SUBSTRING(@Symbol, @Position, 1);

		SET @i = @i + 1;
	END

	INSERT INTO dbo.HeatPoint (PointID, PointName, NetworkID, NodeNumber)
	VALUES (@RowCount, @PointName, @NetworkID, @NodeNumber);

	SET @RowCount += 1;
END

-- ���������� ����������
COMMIT;