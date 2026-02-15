using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NotificationService.Api;

public class ReminderWorker(ILogger<ReminderWorker> logger, IAppointmentClient appointmentClient) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var nextRun = DateTime.UtcNow.Date.AddDays(1).AddHours(7);
            var delay = nextRun - DateTime.UtcNow;
            if (delay.TotalSeconds > 0)
                await Task.Delay(delay, stoppingToken);

            var targetDay = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
            var appointments = await appointmentClient.GetAppointmentsForDayAsync(targetDay, stoppingToken);

            foreach (var a in appointments)
            {
                logger.LogInformation("[MockWhatsApp] Reminder sent for Appointment={AppointmentId} Patient={PatientId} Tenant={TenantId}", a.Id, a.PatientId, a.TenantId);
            }
        }
    }
}

public record AppointmentReminderDto(Guid Id, string TenantId, Guid PatientId, DateTime StartAtUtc);

public interface IAppointmentClient
{
    Task<IReadOnlyCollection<AppointmentReminderDto>> GetAppointmentsForDayAsync(DateOnly day, CancellationToken ct);
}
