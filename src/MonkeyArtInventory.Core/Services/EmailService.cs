using System.Net;
using System.Net.Mail;
using MonkeyArtInventory.Core.Models;

namespace MonkeyArtInventory.Core.Services;

public class EmailService
{
    private readonly SettingsService _settingsService;

    public EmailService(SettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public bool IsConfigured()
    {
        var settings = _settingsService.Current;
        return settings.EmailEnabled &&
               !string.IsNullOrWhiteSpace(settings.SmtpServer) &&
               !string.IsNullOrWhiteSpace(settings.SmtpUsername) &&
               !string.IsNullOrWhiteSpace(settings.SmtpPassword) &&
               !string.IsNullOrWhiteSpace(settings.EmailFrom) &&
               !string.IsNullOrWhiteSpace(settings.EmailTo);
    }

    public async Task<(bool Success, string Message)> SendReportAsync(string reportPath, string subject = "")
    {
        if (!IsConfigured())
        {
            return (false, "El correo electrónico no está configurado. Ve a Configuración para configurarlo.");
        }

        var settings = _settingsService.Current;

        if (string.IsNullOrWhiteSpace(subject))
        {
            subject = $"Informe Monkey Art Inventory - {DateTime.Now:dd/MM/yyyy}";
        }

        try
        {
            using var client = new SmtpClient(settings.SmtpServer, settings.SmtpPort)
            {
                Credentials = new NetworkCredential(settings.SmtpUsername, settings.SmtpPassword),
                EnableSsl = settings.UseSsl,
                Timeout = 30000
            };

            using var message = new MailMessage
            {
                From = new MailAddress(settings.EmailFrom, "Monkey Art Inventory"),
                Subject = subject,
                Body = $@"Hola,

Adjunto encontrarás el informe de inventario generado automáticamente.

Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}

Saludos,
Monkey Art Inventory",
                IsBodyHtml = false
            };

            // Agregar destinatarios (puede haber varios separados por ; o ,)
            var recipients = settings.EmailTo.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var recipient in recipients)
            {
                message.To.Add(recipient.Trim());
            }

            // Adjuntar el archivo
            if (File.Exists(reportPath))
            {
                var attachment = new Attachment(reportPath);
                message.Attachments.Add(attachment);
            }
            else
            {
                return (false, $"No se encontró el archivo: {reportPath}");
            }

            await client.SendMailAsync(message);
            return (true, $"Informe enviado correctamente a {settings.EmailTo}");
        }
        catch (SmtpException ex)
        {
            return (false, $"Error de SMTP: {ex.Message}. Verifica las credenciales y la configuración del servidor.");
        }
        catch (Exception ex)
        {
            return (false, $"Error al enviar el correo: {ex.Message}");
        }
    }

    public async Task<(bool Success, string Message)> SendTestEmailAsync()
    {
        if (!IsConfigured())
        {
            return (false, "El correo electrónico no está configurado correctamente.");
        }

        var settings = _settingsService.Current;

        try
        {
            using var client = new SmtpClient(settings.SmtpServer, settings.SmtpPort)
            {
                Credentials = new NetworkCredential(settings.SmtpUsername, settings.SmtpPassword),
                EnableSsl = settings.UseSsl,
                Timeout = 30000
            };

            using var message = new MailMessage
            {
                From = new MailAddress(settings.EmailFrom, "Monkey Art Inventory"),
                Subject = "Prueba de configuración - Monkey Art Inventory",
                Body = $@"¡Hola!

Este es un correo de prueba de Monkey Art Inventory.

Si estás recibiendo este mensaje, la configuración de correo está funcionando correctamente.

Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}

Saludos,
Monkey Art Inventory",
                IsBodyHtml = false
            };

            var recipients = settings.EmailTo.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var recipient in recipients)
            {
                message.To.Add(recipient.Trim());
            }

            await client.SendMailAsync(message);
            return (true, $"Correo de prueba enviado correctamente a {settings.EmailTo}");
        }
        catch (SmtpException ex)
        {
            return (false, $"Error de SMTP: {ex.Message}");
        }
        catch (Exception ex)
        {
            return (false, $"Error: {ex.Message}");
        }
    }
}
