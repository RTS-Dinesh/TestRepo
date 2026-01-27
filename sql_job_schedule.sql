USE msdb;

DECLARE @job_name sysname = N'Demo SQL Job';
DECLARE @schedule_name sysname = N'Demo SQL Job - Every 5 Minutes';
DECLARE @job_id UNIQUEIDENTIFIER;

IF EXISTS (SELECT 1 FROM msdb.dbo.sysjobs WHERE name = @job_name)
BEGIN
    EXEC msdb.dbo.sp_delete_job @job_name = @job_name;
END;

IF EXISTS (SELECT 1 FROM msdb.dbo.sysschedules WHERE name = @schedule_name)
BEGIN
    EXEC msdb.dbo.sp_delete_schedule @schedule_name = @schedule_name;
END;

EXEC msdb.dbo.sp_add_job
    @job_name = @job_name,
    @enabled = 1,
    @description = N'Example SQL Agent job created via script.',
    @owner_login_name = SUSER_SNAME(),
    @job_id = @job_id OUTPUT;

EXEC msdb.dbo.sp_add_jobstep
    @job_id = @job_id,
    @step_name = N'Run SQL',
    @subsystem = N'TSQL',
    @database_name = N'master',
    @command = N'
        SET NOCOUNT ON;
        SELECT SYSDATETIME() AS job_ran_at;
        -- Replace with your real job logic
    ';

EXEC msdb.dbo.sp_add_schedule
    @schedule_name = @schedule_name,
    @enabled = 1,
    @freq_type = 4,
    @freq_interval = 1,
    @freq_subday_type = 4,
    @freq_subday_interval = 5,
    @active_start_time = 0;

EXEC msdb.dbo.sp_attach_schedule
    @job_id = @job_id,
    @schedule_name = @schedule_name;

EXEC msdb.dbo.sp_add_jobserver
    @job_id = @job_id,
    @server_name = N'(LOCAL)';

EXEC msdb.dbo.sp_start_job @job_name = @job_name;
