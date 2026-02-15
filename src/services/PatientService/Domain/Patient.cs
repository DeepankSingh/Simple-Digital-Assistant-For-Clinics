namespace PatientService.Domain;

public class Patient
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TenantId { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public int Age { get; set; }
    public string Gender { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string? MedicalHistoryNotes { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
