using System.Text;

namespace PrescriptionService.Infrastructure;

public interface IPdfPrescriptionGenerator
{
    Task<byte[]> GenerateAsync(PrescriptionEntity prescription, CancellationToken ct);
}

public class PdfPrescriptionGenerator : IPdfPrescriptionGenerator
{
    // Mock PDF content for demo; replace with QuestPDF/iText in production.
    public Task<byte[]> GenerateAsync(PrescriptionEntity prescription, CancellationToken ct)
    {
        var content = $"PRESCRIPTION\nId:{prescription.Id}\nPatient:{prescription.PatientId}\nAppointment:{prescription.AppointmentId}\nMedicines:{prescription.MedicinesJson}\nDosage:{prescription.Dosage}\nNotes:{prescription.Notes}";
        return Task.FromResult(Encoding.UTF8.GetBytes(content));
    }
}

public class PrescriptionEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TenantId { get; set; } = default!;
    public Guid PatientId { get; set; }
    public Guid AppointmentId { get; set; }
    public string MedicinesJson { get; set; } = default!;
    public string Dosage { get; set; } = default!;
    public string? Notes { get; set; }
    public DateTime CreatedDateUtc { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = default!;
    public bool IsDeleted { get; set; }
}
