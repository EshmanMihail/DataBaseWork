USE HeatSchemeStorage

SET NOCOUNT ON;
DECLARE @Symbol CHAR(52) = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz',
	@Position INT,
	@i INT,
	@NameLimit INT,
	@RowCount INT,
	@MinNumberSymbols INT,
	@MaxNumberSymbols INT,
	@EnterpriseName NVARCHAR(50),
	@ManagementOrganization NVARCHAR(50);

-- ������ ����������
BEGIN TRAN;

-- ���������� ������� Enterprise
SELECT @i = 0 FROM dbo.Enterprise WITH (TABLOCKX) WHERE 1 = 0;

SET @RowCount = 1;
SET @MinNumberSymbols = 5;
SET @MaxNumberSymbols = 30;

WHILE @RowCount <= 500 -- ���������� 500 �������
BEGIN
	SET @NameLimit = @MinNumberSymbols + RAND() * (@MaxNumberSymbols - @MinNumberSymbols); -- ����� ����� �� 5 �� 30 ��������
	SET @i = 1;
	SET @EnterpriseName = '';
	SET @ManagementOrganization = '';

	WHILE @i <= @NameLimit
	BEGIN
		SET @Position = RAND() * 52;
		SET @EnterpriseName = @EnterpriseName + SUBSTRING(@Symbol, @Position, 1);

		SET @Position = RAND() * 52;
		SET @ManagementOrganization = @ManagementOrganization + SUBSTRING(@Symbol, @Position, 1);

		SET @i = @i + 1;
	END

	INSERT INTO dbo.Enterprise (EnterpriseID, EnterpriseName, ManagementOrganization)
	VALUES (@RowCount, @EnterpriseName, @ManagementOrganization);

	SET @RowCount += 1;
END

-- ���������� ����������
COMMIT;