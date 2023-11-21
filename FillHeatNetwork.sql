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

-- ������ ����������
BEGIN TRAN;

-- ���������� ������� HeatNetwork
SET @MinNumberSymbols = 5;
SET @MaxNumberSymbols = 30;

SET @i = 1;
WHILE @i <= 500 -- ���������� 500 �������
BEGIN
	SET @NameLimit = @MinNumberSymbols + RAND() * (@MaxNumberSymbols - @MinNumberSymbols); -- ����� ����� �� 5 �� 30 ��������
	SET @NetworkName = '';
	SET @NetworkNumber = FLOOR(RAND() * 1000) + 1; -- ��������� ����� �� 1 �� 1000
	SET @NetworkType = CASE WHEN RAND() > 0.5 THEN 'Type A' ELSE 'Type B' END; -- ��������� ����� ����� 'Type A' � 'Type B'

	SET @Position = 1; -- ��������� ���������� �������� �������
	WHILE @Position <= @NameLimit
	BEGIN
		SET @k = RAND() * 52;
		SET @NetworkName = @NetworkName + SUBSTRING(@Symbol, @k, 1);
		SET @Position = @Position + 1;
	END

	SET @EnterpriseID = FLOOR(RAND() * 500) + 1; -- ��������� ����� �� ������������ EnterpriseID

	INSERT INTO dbo.HeatNetwork (NetworkID, NetworkName, NetworkNumber, EnterpriseID, NetworkType)
	VALUES (@i, @NetworkName, @NetworkNumber, @EnterpriseID, @NetworkType);

	SET @i += 1; -- ���������� ���������� @i

END

-- ���������� ����������
COMMIT;