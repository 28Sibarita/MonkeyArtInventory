param(
    [string]$ExePath = "C:\\Program Files\\MonkeyArtInventory\\MonkeyArtInventory.App.exe",
    [string]$TaskName = "MonkeyArtInventory Weekly Report",
    [switch]$RunWhenLoggedOff
)

if (-not (Test-Path $ExePath)) {
    Write-Error "Executable not found at $ExePath"
    exit 1
}

$action = New-ScheduledTaskAction -Execute $ExePath -Argument "--weekly-report"
$trigger = New-ScheduledTaskTrigger -Weekly -DaysOfWeek Saturday -At 5pm

if ($RunWhenLoggedOff) {
    $cred = Get-Credential -Message "Enter Windows credentials for the scheduled task"
    Register-ScheduledTask -TaskName $TaskName -Action $action -Trigger $trigger -User $cred.UserName -Password $cred.GetNetworkCredential().Password -Force | Out-Null
    Write-Host "Task created to run whether user is logged on or not."
} else {
    Register-ScheduledTask -TaskName $TaskName -Action $action -Trigger $trigger -Force | Out-Null
    Write-Host "Task created for the current user. Use -RunWhenLoggedOff to store credentials."
}

Write-Host "Logs are written to %LOCALAPPDATA%\\MonkeyArtInventory\\Logs\\weekly_report.log"
