using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrescriptionService.Application;

namespace PrescriptionService.Api;

[ApiController]
[Route("api/prescriptions")]
[Authorize(Roles = "Admin,Doctor")]
public class PrescriptionsController(IPrescriptionService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreatePrescriptionRequest request, CancellationToken ct)
        => Ok(await service.CreateAsync(request, User, ct));

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id, CancellationToken ct)
    {
        var bytes = await service.DownloadPdfAsync(id, User, ct);
        return File(bytes, "application/pdf", $"prescription-{id}.pdf");
    }
}
