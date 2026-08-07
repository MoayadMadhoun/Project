using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
using Project.Repositories;

namespace Project.Pages.Institution.CurrentTraining;

public class StudentDetailsModel : PageModel
{
    private readonly StudentsRepository _studentsRepository;

    public StudentDetailsModel(
        StudentsRepository studentsRepository)
    {
        _studentsRepository = studentsRepository;
    }

    [BindProperty(SupportsGet = true)]
    public int PlacementId { get; set; }

    public int PlacementID { get; set; }

    public int StudentID { get; set; }

    public string StudentName { get; set; } = "";

    public string StudentNumber { get; set; } = "";

    public string Specialty { get; set; } = "";

    public decimal? GPA { get; set; }

    public string Email { get; set; } = "";

    public string PhoneNumber { get; set; } = "";

    public string OpportunityTitle { get; set; } = "";

    public string InstitutionName { get; set; } = "";

    public string InstitutionSupervisor { get; set; } = "";

    public string UniversitySupervisor { get; set; } = "";

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int ReportsCount { get; set; }

    public int EvaluationsCount { get; set; }

    public int AttendanceCount { get; set; }

    public int AbsentCount { get; set; }

    public async Task<IActionResult> OnGetAsync(int placementId)
    {
        var placement =
            await _studentsRepository
                .GetPlacementDetailsAsync(placementId);

        if (placement == null)
            return NotFound();

        PlacementID = placement.PlacementID;

        StudentID = placement.StudentID;

        StudentName =
            placement.Student?.Name ?? "";

        StudentNumber =
            placement.Student?.StudentNumber ?? "";

        Specialty =
            placement.Student?.Specialty?.Name ?? "";

        GPA =
            placement.Student?.GPA;

        Email =
            placement.Student?.User?.Email ?? "";

        PhoneNumber =
            placement.Student?.PhoneNumber ?? "";

        OpportunityTitle =
            placement.TrainingOpportunity?.Title ?? "";

        InstitutionName =
            placement.TrainingInstitution?.Name ?? "";

        InstitutionSupervisor =
            placement.InstitutionSupervisor?.FullName ?? "";

        UniversitySupervisor =
            placement.UniversitySupervisor?.FullName ?? "";

        StartDate = placement.StartDate;

        EndDate = placement.EndDate;

        ReportsCount =
            placement.StudentReports.Count;

        EvaluationsCount =
            placement.StudentEvaluations.Count;

        AttendanceCount =
            placement.AttendanceRecords.Count(x =>
                x.Status ==
                AttendanceRecord.AttendanceStatus.Present);

        AbsentCount =
            placement.AttendanceRecords.Count(x =>
                x.Status ==
                AttendanceRecord.AttendanceStatus.Absent);

        return Page();
    }
}