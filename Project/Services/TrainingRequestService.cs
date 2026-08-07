using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Pages.University.ViewModels;
using Project.Repositories;
using Project.Repostory;

namespace Project.Services;

public class TrainingRequestService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UniversityRepository _universityRepo;

    public TrainingRequestService(
        ApplicationDbContext dbContext,
        UniversityRepository universityRepo
        )
    {
        _dbContext = dbContext;
        _universityRepo = universityRepo;
    }

    public async Task CreateTrainingRequestAsync(
        TrainingRequestVM model,
        AspNetUser user,
        AspNetRoleScope scope,
        bool isUniversityAdmin)
    {
        var university = await _universityRepo.GetByIdAsync(scope.UniversityID!.Value);

        var request = new TrainingOpportunityRequest
        {
            Title = model.Title,
            Description = model.Description,
            PreferredStartDate = model.PreferredStartDate,
            PreferredEndDate = model.PreferredEndDate,
            RequestedSeats = model.RequestedSeats,
            Notes = model.Notes,
            TermID = model.TermID,
            Status = model.Status,
            UniversityID = university!.UniversityID,
            CreatedAt = DateTime.UtcNow

        };

        if (isUniversityAdmin)
        {
            request.UniversityAdminID = user.Id;
            request.DepartmentHeadID = null;
        }
        else
        {
            request.DepartmentHeadID = user.Id;
            request.UniversityAdminID = null;
        }

        await _dbContext.TrainingOpportunityRequests.AddAsync(request);
        await _dbContext.SaveChangesAsync();

        foreach (var specialtyId in model.SelectedSpecialties)
        {
            await _dbContext.RequestSpecialties.AddAsync(new RequestSpecialty
            {
                RequestID = request.RequestID,
                SpecialtyID = specialtyId
            });
        }

        foreach (var skill in model.Skills.Where(x => x.Selected))
        {
            await _dbContext.RequestSkills.AddAsync(new RequestSkill
            {
                RequestID = request.RequestID,
                SkillID = skill.SkillId,
                IsRequired = skill.IsRequired
            });
        }

        if (request.Status == TrainingOpportunityRequest.RequestStatus.Published)
        {
            await _dbContext.OpportunityRequestInstitutions.AddAsync(
                new OpportunityRequestInstitution
                {
                    RequestID = request.RequestID,
                    InstitutionID = model.InstitutionID,
                    Status = OpportunityRequestInstitution.OpportunityRequestInstitutionStatus.Invited,
                    SentAt = DateTime.UtcNow
                });
        }

        await _dbContext.SaveChangesAsync();
    }


    public async Task<bool> UpdateTrainingRequestAsync(
      int requestId,
      EditTrainingRequestVM model,
      AspNetUser user,
      AspNetRoleScope scope)
    {
        var university = await _universityRepo.GetByIdAsync(scope.UniversityID!.Value);
        if (university == null) return false;

        var request = await _dbContext.TrainingOpportunityRequests
            .FirstOrDefaultAsync(tr => tr.RequestID == requestId);

        if (request == null) return false;
        if (request.UniversityID != university.UniversityID) return false;

        // --- تحديث بيانات الطلب الأساسية ---
        request.Title = model.Title;
        request.Description = model.Description;
        request.PreferredStartDate = model.PreferredStartDate;
        request.PreferredEndDate = model.PreferredEndDate;
        request.RequestedSeats = model.RequestedSeats;
        request.ApplicationDeadline = model.ApplicationDeadline;
        request.Notes = model.Notes;
        request.TermID = model.TermID;
        request.Status = model.Status;
        

        _dbContext.TrainingOpportunityRequests.Update(request);

        // --- تحديث التخصصات: حذف القديمة وإضافة الجديدة ---
        var oldSpecialties = _dbContext.RequestSpecialties
            .Where(rs => rs.RequestID == requestId);
        _dbContext.RequestSpecialties.RemoveRange(oldSpecialties);

        foreach (var specialtyId in model.SelectedSpecialties)
        {
            await _dbContext.RequestSpecialties.AddAsync(new RequestSpecialty
            {
                RequestID = requestId,
                SpecialtyID = specialtyId
            });
        }

        // --- تحديث المهارات: حذف القديمة وإضافة الجديدة ---
        var oldSkills = _dbContext.RequestSkills
            .Where(rs => rs.RequestID == requestId);
        _dbContext.RequestSkills.RemoveRange(oldSkills);

        foreach (var skill in model.Skills.Where(s => s.Selected))
        {
            await _dbContext.RequestSkills.AddAsync(new RequestSkill
            {
                RequestID = requestId,
                SkillID = skill.SkillId,
                IsRequired = skill.IsRequired
            });
        }

        // --- تحديث المؤسسة المرتبطة بالطلب ---
        var oldInstitutions = _dbContext.OpportunityRequestInstitutions
            .Where(x => x.RequestID == requestId);

        _dbContext.OpportunityRequestInstitutions.RemoveRange(oldInstitutions);

        if (request.Status == TrainingOpportunityRequest.RequestStatus.Published)
        {
            await _dbContext.OpportunityRequestInstitutions.AddAsync(
                new OpportunityRequestInstitution
                {
                    RequestID = requestId,
                    InstitutionID = model.InstitutionID,
                    Status = OpportunityRequestInstitution.OpportunityRequestInstitutionStatus.Invited,
                    SentAt = DateTime.UtcNow
                });
        }

        await _dbContext.SaveChangesAsync();
        return true;
    }
}