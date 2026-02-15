using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientService.Application;

namespace PatientService.Api;

[ApiController]
[Route("api/patients")]
[Authorize(Roles = "Admin,Doctor")]
public class PatientsController(IPatientService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreatePatientRequest request, CancellationToken ct)
        => Ok(await service.CreateAsync(request, User, ct));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdatePatientRequest request, CancellationToken ct)
        => Ok(await service.UpdateAsync(id, request, User, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await service.DeleteAsync(id, User, ct);
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        => Ok(await service.SearchAsync(q, page, pageSize, User, ct));
}

public interface IPatientService
{
    Task<PatientDto> CreateAsync(CreatePatientRequest request, System.Security.Claims.ClaimsPrincipal user, CancellationToken ct);
    Task<PatientDto> UpdateAsync(Guid id, UpdatePatientRequest request, System.Security.Claims.ClaimsPrincipal user, CancellationToken ct);
    Task DeleteAsync(Guid id, System.Security.Claims.ClaimsPrincipal user, CancellationToken ct);
    Task<PagedResult<PatientDto>> SearchAsync(string? query, int page, int pageSize, System.Security.Claims.ClaimsPrincipal user, CancellationToken ct);
}
