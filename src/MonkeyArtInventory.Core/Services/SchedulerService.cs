using System.Timers;
using MonkeyArtInventory.Core.Models;
using Timer = System.Timers.Timer;

namespace MonkeyArtInventory.Core.Services;

public class SchedulerService : IDisposable
{
    private readonly SettingsService _settingsService;
    private readonly ReportService _reportService;
    private readonly EmailService _emailService;
    private Timer? _timer;
    private bool _isRunning;
    private Action<string>? _logAction;

    public SchedulerService(
        SettingsService settingsService,
        ReportService reportService,
        EmailService emailService)
    {
        _settingsService = settingsService;
        _reportService = reportService;
        _emailService = emailService;
    }

    public event EventHandler<string>? StatusChanged;

    public bool IsRunning => _isRunning;

    public string GetNextRunDescription()
    {
        var settings = _settingsService.Current;
        if (!settings.SchedulerEnabled || !settings.EmailEnabled)
        {
            return "Desactivado";
        }

        var nextRun = GetNextRunTime();
        if (nextRun.HasValue)
        {
            var dayName = GetDayName(settings.SchedulerDay);
            return $"{dayName} a las {settings.SchedulerHour:00}:{settings.SchedulerMinute:00} (próximo: {nextRun.Value:dd/MM/yyyy HH:mm})";
        }

        return "No programado";
    }

    public void SetLogAction(Action<string> logAction)
    {
        _logAction = logAction;
    }

    public void Start()
    {
        if (_isRunning) return;

        var settings = _settingsService.Current;
        if (!settings.SchedulerEnabled)
        {
            Log("Scheduler no habilitado");
            return;
        }

        if (!_emailService.IsConfigured())
        {
            Log("Email no configurado, scheduler no puede iniciar");
            return;
        }

        // Check every minute
        _timer = new Timer(60000); // 1 minute
        _timer.Elapsed += OnTimerElapsed;
        _timer.AutoReset = true;
        _timer.Start();

        _isRunning = true;
        Log($"Scheduler iniciado. Próximo envío: {GetNextRunDescription()}");
        StatusChanged?.Invoke(this, "Scheduler iniciado");
    }

    public void Stop()
    {
        if (!_isRunning) return;

        _timer?.Stop();
        _timer?.Dispose();
        _timer = null;
        _isRunning = false;

        Log("Scheduler detenido");
        StatusChanged?.Invoke(this, "Scheduler detenido");
    }

    public void Restart()
    {
        Stop();
        Start();
    }

    /// <summary>
    /// Send the report immediately (manual trigger for testing)
    /// </summary>
    public async Task<(bool Success, string Message)> SendNowAsync()
    {
        if (!_emailService.IsConfigured())
        {
            return (false, "El correo no está configurado");
        }

        try
        {
            var settings = _settingsService.Current;

            // Generate the report
            Log("Generando informe semanal...");
            var report = await _reportService.BuildWeeklyReportAsync();

            // Export to PDF
            Log("Exportando PDF...");
            var pdfPath = _reportService.ExportWeeklyReport(report, settings.ReportsDirectory);

            // Send email
            Log("Enviando por correo...");
            var (success, message) = await _emailService.SendReportAsync(pdfPath, "Informe Semanal Manual - Monkey Art Inventory");

            if (success)
            {
                Log($"✓ Informe enviado exitosamente");
                StatusChanged?.Invoke(this, $"Último envío manual: {DateTime.Now:dd/MM/yyyy HH:mm}");
                return (true, "Informe enviado correctamente");
            }
            else
            {
                Log($"✗ Error al enviar: {message}");
                return (false, message);
            }
        }
        catch (Exception ex)
        {
            Log($"✗ Error: {ex.Message}");
            return (false, ex.Message);
        }
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        try
        {
            CheckAndSendReport();
        }
        catch (Exception ex)
        {
            Log($"Error en scheduler: {ex.Message}");
        }
    }

    private void CheckAndSendReport()
    {
        var settings = _settingsService.Current;
        if (!settings.SchedulerEnabled || !_emailService.IsConfigured())
        {
            return;
        }

        var now = DateTime.Now;
        
        // Check if it's the scheduled day and time
        if (now.DayOfWeek != settings.SchedulerDay)
        {
            return;
        }

        if (now.Hour != settings.SchedulerHour || now.Minute != settings.SchedulerMinute)
        {
            return;
        }

        // Check if we already ran this week
        if (settings.LastScheduledRun.HasValue)
        {
            var lastRun = settings.LastScheduledRun.Value;
            var daysSinceLastRun = (now - lastRun).TotalDays;
            if (daysSinceLastRun < 1) // Already ran today
            {
                return;
            }
        }

        // Time to send the report!
        Log("Iniciando envío automático de informe...");
        SendScheduledReport();
    }

    private async void SendScheduledReport()
    {
        try
        {
            var settings = _settingsService.Current;

            // Generate the report
            Log("Generando informe semanal...");
            var report = await _reportService.BuildWeeklyReportAsync();

            // Export to PDF
            Log("Exportando PDF...");
            var pdfPath = _reportService.ExportWeeklyReport(report, settings.ReportsDirectory);

            // Send email
            Log("Enviando por correo...");
            var (success, message) = await _emailService.SendReportAsync(pdfPath, "Informe Semanal Automático - Monkey Art Inventory");

            if (success)
            {
                Log($"✓ Informe enviado exitosamente: {message}");
                
                // Update last run time
                settings.LastScheduledRun = DateTime.Now;
                _settingsService.Save();
                
                StatusChanged?.Invoke(this, $"Último envío: {DateTime.Now:dd/MM/yyyy HH:mm}");
            }
            else
            {
                Log($"✗ Error al enviar: {message}");
                StatusChanged?.Invoke(this, $"Error: {message}");
            }
        }
        catch (Exception ex)
        {
            Log($"✗ Error en envío automático: {ex.Message}");
            StatusChanged?.Invoke(this, $"Error: {ex.Message}");
        }
    }

    private DateTime? GetNextRunTime()
    {
        var settings = _settingsService.Current;
        var now = DateTime.Now;
        
        // Find next occurrence of the scheduled day
        var daysUntilTarget = ((int)settings.SchedulerDay - (int)now.DayOfWeek + 7) % 7;
        
        var nextRun = now.Date.AddDays(daysUntilTarget)
            .AddHours(settings.SchedulerHour)
            .AddMinutes(settings.SchedulerMinute);

        // If the time already passed today, add 7 days
        if (nextRun <= now)
        {
            nextRun = nextRun.AddDays(7);
        }

        return nextRun;
    }

    private static string GetDayName(DayOfWeek day)
    {
        return day switch
        {
            DayOfWeek.Sunday => "Domingo",
            DayOfWeek.Monday => "Lunes",
            DayOfWeek.Tuesday => "Martes",
            DayOfWeek.Wednesday => "Miércoles",
            DayOfWeek.Thursday => "Jueves",
            DayOfWeek.Friday => "Viernes",
            DayOfWeek.Saturday => "Sábado",
            _ => day.ToString()
        };
    }

    private void Log(string message)
    {
        var logMessage = $"[{DateTime.Now:HH:mm:ss}] {message}";
        _logAction?.Invoke(logMessage);
        System.Diagnostics.Debug.WriteLine($"[Scheduler] {logMessage}");
    }

    public void Dispose()
    {
        Stop();
    }
}
