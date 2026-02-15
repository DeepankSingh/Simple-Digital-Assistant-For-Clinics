using Polly;

namespace NotificationService.Infrastructure;

public static class HttpClientRegistration
{
    public static IServiceCollection AddAppointmentHttpClient(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpClient<IAppointmentClient, AppointmentClient>(c =>
        {
            c.BaseAddress = new Uri(config["ServiceEndpoints:AppointmentService"]!);
            c.Timeout = TimeSpan.FromSeconds(10);
        })
        .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(200 * attempt)))
        .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

        return services;
    }
}

public class AppointmentClient(HttpClient client) : IAppointmentClient
{
    public async Task<IReadOnlyCollection<AppointmentReminderDto>> GetAppointmentsForDayAsync(DateOnly day, CancellationToken ct)
    {
        var response = await client.GetAsync($"api/appointments/reminders?day={day:yyyy-MM-dd}", ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<AppointmentReminderDto>>(cancellationToken: ct) ?? [];
    }
}
