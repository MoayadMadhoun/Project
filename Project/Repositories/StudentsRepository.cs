using Humanizer;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using static Project.Models.Student;
using static Project.Models.TrainingApplication;
using static System.Net.Mime.MediaTypeNames;

namespace Project.Repositories
{
    public class StudentsRepository
    {
        // Field
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        // create constructor  
        public StudentsRepository(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        private IQueryable<Student> StudentDetails()
        {
            return _context.Students
                .Include(s => s.Skills).ThenInclude(ss => ss.Skill)
                .Include(s => s.PortfolioItems)
                .Include(s => s.Department)
                .Include(s => s.Specialty);
        }
        // Get all student With Details
        public async Task<List<Student>> GetAllStudents()
        {
            return await StudentDetails().AsNoTracking().ToListAsync();

        }
        public async Task<Student?> GetStudentByUserId(string userId)
        {

            return await StudentDetails().FirstOrDefaultAsync(s => s.UserID == userId);
        }
        // get student By Id 
        public async Task<Student?> GetStudentById(int studentId)
        {

            return await StudentDetails().FirstOrDefaultAsync(s => s.StudentID == studentId);
        }
        // Search student
        public async Task<List<Student>> SearchStudent
            (string? name = null, string? department = null, string? skill = null, string? specialty = null)
        {
            var query = StudentDetails().AsNoTracking();

            // Search by Name
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(s =>
                    EF.Functions.Like(s.Name, $"%{name}%"));
            }

            // Search by Department
            if (!string.IsNullOrWhiteSpace(department))
            {
                query = query.Where(s =>
                    EF.Functions.Like(s.Department.Name, $"%{department}%"));
            }

            // Search by Skill
            if (!string.IsNullOrWhiteSpace(skill))
            {
                query = query.Where(s =>
                    s.Skills.Any(ss =>
                        EF.Functions.Like(ss.Skill.Name, $"%{skill}%")));
            }

            // Search by Specialty
            if (!string.IsNullOrWhiteSpace(specialty))
            {
                query = query.Where(s =>
                    s.Specialty != null &&
                    EF.Functions.Like(s.Specialty.Name, $"%{specialty}%"));
            }

            return await query.ToListAsync();
        }

        //add student 
        public async Task AddStudent(Student newStudent)
        {
            _context.Students.Add(newStudent);
            await _context.SaveChangesAsync();
        }

        // update student 
        public async Task UpdateStudent(Student Student)
        {
            _context.Students.Update(Student);
            await _context.SaveChangesAsync();

        }

        //delete student BY id 
        public async Task<bool> DeleteStudentById(int id)
        {
            var Student = await StudentDetails().FirstOrDefaultAsync(s => s.StudentID == id);
            if (Student == null) return false;
            await DeleteAllStudentSkill(id, false);
            await DeletePortfolioItem(id, false);
            await DeleteAllStudentReportByStudentId(id, false);
            _context.Students.Remove(Student);
            await _context.SaveChangesAsync();
            DeleteImage(Student.ProfileImagePath);
            DeleteCvFile(Student.CVPath);
            return true;
        }


        //add PortfolioItem
        public async Task AddPortfolioItem(PortfolioItem newPortfolioItem)
        {
            _context.PortfolioItems.Add(newPortfolioItem);
            await _context.SaveChangesAsync();
        }

        //update PortfolioItem
        public async Task UpdatePortfolioItem(PortfolioItem PortfolioItem)
        {
            _context.PortfolioItems.Update(PortfolioItem);
            await _context.SaveChangesAsync();

        }
        // get PortfolioItem by student id 
        public async Task<List<PortfolioItem>> GetPortfolioItem(int studentId)
        {
            //(pi)=> PortfolioItems
            return await _context.PortfolioItems.Where(pi => pi.StudentID == studentId).ToListAsync();
        }
        //delete all PortfolioItems by student id 
        public async Task DeletePortfolioItem(int studentId, bool saveChange = true)
        {
            var PortfolioItems = await GetPortfolioItem(studentId);

            _context.PortfolioItems.RemoveRange(PortfolioItems);
            if (saveChange) { await _context.SaveChangesAsync(); }


        }
        //Get ProfileStatus status BY student ID
        public async Task<bool> GetProfileStatus(int studentId)
        {
            bool status = true;
            var PortfolioItems = await GetPortfolioItem(studentId);
            if (!PortfolioItems.Any()) return false;// not have a PortfolioItems
            foreach (var item in PortfolioItems)
            {
                if (string.IsNullOrEmpty(item.Description)) { status = false; }
                if (string.IsNullOrEmpty(item.Title)) { status = false; }
                switch (item.Type)
                {
                    case PortfolioItem.ItemType.Certificate:
                        if (string.IsNullOrWhiteSpace(item.FilePath)) { status = false; }
                        break;
                    case PortfolioItem.ItemType.Project:
                        if (string.IsNullOrWhiteSpace(item.Url)) { status = false; }
                        break;
                    case PortfolioItem.ItemType.Presentation:
                        if (string.IsNullOrWhiteSpace(item.FilePath)) { status = false; }
                        break;
                    case PortfolioItem.ItemType.Report:
                        if (string.IsNullOrWhiteSpace(item.FilePath)) { status = false; }
                        break;
                    case PortfolioItem.ItemType.Link:
                        if (string.IsNullOrWhiteSpace(item.Url)) { status = false; }
                        break;
                    case PortfolioItem.ItemType.Other:
                        if (string.IsNullOrWhiteSpace(item.Url) && string.IsNullOrWhiteSpace(item.FilePath)) { status = false; }
                        break;

                }
            }
            return status;
        }
        // add new skills
        public async Task AddStudentSkills(StudentSkill NewStudentSkill)
        {
            _context.StudentSkills.Add(NewStudentSkill);
            await _context.SaveChangesAsync();
        }

        // Get All Skills BY student id 
        public async Task<List<StudentSkill>> GetStudentSkills(int studentId)
        {
            // (ss) => Student Skill
            return await _context.StudentSkills.Where(ss => ss.StudentID == studentId).ToListAsync();
        }
        // delete  Student Skills 
        public async Task DeleteStudentSkills(StudentSkill StudentSkill)
        {
            _context.StudentSkills.Remove(StudentSkill);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> DeleteStudentSkill(int studentId, int studentSkillId)
        {
            var studentSkill = await _context.StudentSkills
                .FirstOrDefaultAsync(s => s.StudentID == studentId && s.StudentSkillID == studentSkillId);

            if (studentSkill != null)
            {
                _context.StudentSkills.Remove(studentSkill);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task AddAsync(Student student)
        {

            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
        }

        // delete  all Student Skills By student id 
        public async Task DeleteAllStudentSkill(int studentId, bool saveChange = true)
        {
            var studentSkill = await _context.StudentSkills.
             Where(s => s.StudentID == studentId).ToListAsync();
            _context.StudentSkills.RemoveRange(studentSkill);
            if (saveChange) { await _context.SaveChangesAsync(); }


        }

        // update student skills
        public async Task UpdateStudentSkills(StudentSkill StudentSkill)
        {
            _context.StudentSkills.Update(StudentSkill);
            await _context.SaveChangesAsync();
        }

        //Get all skills
        public async Task<List<Skill>> GetAllSkills()
        {
            return await _context.Skills.AsNoTracking().ToListAsync();
        }

        //get all specialty
        public async Task<List<Specialty>> GetAllSpecialty()
        {
            return await _context.Specialties.AsNoTracking().ToListAsync();
        }

        //get skill by search name 
        public async Task<List<Skill>> SearchSkillsByName(string strSearch)
        {

            return await _context.Skills.AsNoTracking().Where(s => EF.Functions.Like(s.Name, $"%{strSearch}%")).ToListAsync();

        }

        //get specialty by search name 
        public async Task<List<Specialty>> SearchSpecialtyByName(string strSearch)
        {

            return await _context.Specialties.AsNoTracking().Where(s => EF.Functions.Like(s.Name, $"%{strSearch}%")).ToListAsync();

        }
        // Add Training Application

        public async Task AddTrainingApplication(TrainingApplication newTrainingApplication)
        {
            _context.TrainingApplications.Add(newTrainingApplication);
            await _context.SaveChangesAsync();
        }

        //Tracking the status of specific Training Application using Application id
        public async Task<TrainingApplication?> GetApplicationById(int id)
        {
            return await _context.TrainingApplications.AsNoTracking().
              Include(tp => tp.TrainingOpportunity).
              FirstOrDefaultAsync(tp => tp.ApplicationID == id);
        }

        // get all Training Application for student by student Id
        public IQueryable<TrainingApplication> GetAllApplicationByStudentId(int studentId)
        {
            return _context.TrainingApplications.AsNoTracking().
                Include(tp => tp.TrainingOpportunity).ThenInclude(to => to.TrainingInstitution).
                                                      Where(tp => tp.StudentID == studentId);
        }
        // get IQueryable all Training Application for student by student Id use in Paginated List
        public IQueryable<TrainingApplication> GetTrainingApplications(int studentId)
        {
            return _context.TrainingApplications.
                Include(tp => tp.TrainingOpportunity).ThenInclude(t => t.TrainingInstitution).
                Where(tp => tp.StudentID == studentId);
        }

        // Get all  Training Terms for all Opportunities
        public async Task<List<TrainingTerm>> GetAllTrainingTerm()
        {
            return await _context.TrainingTerms.AsNoTracking().Include(tt => tt.Opportunities).ToListAsync();
        }
        //search for an Opportunity by its title
        public async Task<List<TrainingOpportunity>> GetOpportunityByTitle(string strSearch)
        {
            return await _context.TrainingOpportunities.AsNoTracking().Where(to => EF.Functions.Like(to.Title, $"%{strSearch}%")).ToListAsync();

        }
        // Get  Training Placement By Student Id 
        public async Task<TrainingPlacement?> GetTrainingPlacementByStudentId(int id)
        {
            return await _context.TrainingPlacements.AsNoTracking().FirstOrDefaultAsync(tp => tp.StudentID == id);
        }

        // Retrieve the training term for a specific opportunity
        public async Task<TrainingTerm?> GetTrainingTermByOpportunityId(int id)
        {
            return await _context.TrainingTerms.
                Include(tt => tt.Opportunities).
                FirstOrDefaultAsync(tt => tt.Opportunities.Any(o => o.OpportunityID == id));
        }
        //Get All Opportunity
        public async Task<List<TrainingOpportunity>> GetAllOpportunity()
        {
            return await _context.TrainingOpportunities.ToListAsync();
        }

        // show a evaluation for  Opportunity by OpportunityId
        public async Task<StudentEvaluation?> GetEvaluationStudent(int TrainingPlacementId, int studentId)
        {
            return await _context.StudentEvaluations.Include(se => se.TrainingPlacement).
           FirstOrDefaultAsync(se => se.TrainingPlacement.PlacementID == TrainingPlacementId && se.TrainingPlacement.StudentID == studentId);
        }
        // View all evaluations for the student based on the student ID number 
        public IQueryable<StudentEvaluation> GetAllEvaluationByStudentId(int studentId)
        {
            return  _context.StudentEvaluations.Include(se => se.TrainingPlacement).
            Where(se => se.TrainingPlacement.StudentID == studentId);
        }

        // View the attendance schedule 
        public async Task<List<AttendanceRecord>> GetAttendanceRecord(int studentId, int? placementId = null)
        {
            var query = _context.AttendanceRecords
                .Include(ar => ar.TrainingPlacement)
                .AsQueryable();

            // Filter by Student
            query = query.Where(ar =>
                ar.TrainingPlacement.StudentID == studentId);

            // Filter by Placement if exists
            if (placementId != null)
            {
                query = query.Where(ar =>
                    ar.TrainingPlacement.PlacementID == placementId);
            }

            return await query.ToListAsync();
        }

        // remove  student image 
        private void DeleteImage(string? imgPath)
        {
            if (string.IsNullOrEmpty(imgPath)) return;
            var filePath = Path.Combine(_environment.WebRootPath, imgPath.TrimStart('/'));
            if (File.Exists(filePath)) { File.Delete(filePath); }
        }
        // Delete Cv File
        private void DeleteCvFile(string? CvPath)
        {
            if (string.IsNullOrEmpty(CvPath)) return;
            var filePath = Path.Combine(_environment.WebRootPath, CvPath.TrimStart('/'));
            if (File.Exists(filePath)) { File.Delete(filePath); }
        }

        // View the evaluation for a specific opportunity based on the opportunity name and student ID
        public async Task<List<StudentEvaluation>> SearchEvaluationsByOpportunityName(int studentId, string strSearch)
        {
            return await _context.StudentEvaluations.Include(se => se.TrainingPlacement).
                    ThenInclude(tp => tp.TrainingOpportunity).
                    Where
                    (
                    se => se.TrainingPlacement.StudentID == studentId &&
                    EF.Functions.Like(se.TrainingPlacement.TrainingOpportunity.Title, $"%{strSearch}%")

                    ).ToListAsync();
        }
        // Get all Student report by student is 
        public IQueryable<StudentReport> GetStudentReportsAsync(int studentId){

            return  _context.StudentReports.Include(sr => sr.Placement).Where(sr => sr.StudentID == studentId);
        }

        public async Task<StudentReport?> GetStudentReport(int ReportId)
        {
            return await _context.StudentReports
                .Include(sr => sr.Placement)
                .FirstOrDefaultAsync(sr => sr.ReportID == ReportId);
        }
        // create a report
        public async Task AddStudentReport(StudentReport NewStudentReport)
        {
            _context.StudentReports.Add(NewStudentReport);
            await _context.SaveChangesAsync();

        }
        //update a report
        public async Task UpdateStudentReport(StudentReport StudentReport)
        {
            _context.StudentReports.Update(StudentReport);
            await _context.SaveChangesAsync();

        }
        //delete all report by Student Id
        public async Task DeleteAllStudentReportByStudentId(int id, bool saveChange = true)
        {
            var StudentReports = await _context.StudentReports.Where(sr => sr.StudentID == id).ToListAsync();

            _context.StudentReports.RemoveRange(StudentReports);
            if (saveChange) { await _context.SaveChangesAsync(); }
        }

        //delete a report by report id 
        public async Task<bool> DeleteStudentReportByReportId(int id)
        {
            var StudentReport = await _context.StudentReports.FirstOrDefaultAsync(sr => sr.ReportID == id);
            if (StudentReport != null)
            {

                DeleteReportFile(StudentReport.FilePath);
                _context.StudentReports.Remove(StudentReport);
                await _context.SaveChangesAsync();
                return true;

            }
            return false;
        }
        //delete report File  
        private void DeleteReportFile(string? ReportPath)
        {
            if (string.IsNullOrEmpty(ReportPath)) return;
            var filePath = Path.Combine(_environment.WebRootPath, ReportPath.TrimStart('/'));
            if (File.Exists(filePath)) { File.Delete(filePath); }
        }
        // Get students By Status 
        public async Task<List<Student>> GetStudentsByStatus(StudentStatus status)
        {
            return await StudentDetails()
                .AsNoTracking()
                .Where(s => s.Status == status)
                .ToListAsync();
        }
        // Get Application Training  By Status and student Id
        public async Task<List<TrainingApplication>> GetApplicationsByStatus(int studentId, ApplicationStatus status)
        {
            return await _context.TrainingApplications
                .Include(tp => tp.TrainingOpportunity)
                .AsNoTracking()
                .Where(tp => tp.StudentID == studentId && tp.Status == status)
                .ToListAsync();
        }

        // Soft Delete  Make student Not Active 
        public async Task<bool> DeleteSoftAsync(int StudentId)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentID == StudentId);
            if (student == null)
                return false;
            student.Status = StudentStatus.Inactive;
            await _context.SaveChangesAsync();
            return true;

        }
        public IQueryable<Student> GetStudentsForUniversity(int UniversityId)
        {
            return _context.Students
                .Include(s => s.Department)
                .Include(s => s.Specialty)
                .Include(s=>s.Applications)
                .ThenInclude(a=>a.TrainingOpportunity)
                .Where(s => s.UniversityID == UniversityId).AsQueryable();
        }
    }
}