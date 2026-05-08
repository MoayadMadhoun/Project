using Humanizer;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Project.Repositories
{
    public class StudentsRepository
    {
        // Field
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment  _environment;

       // create constructor  
        public StudentsRepository(ApplicationDbContext context , IWebHostEnvironment environment) 
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
        public async Task <List<Student>> GetAllStudents()
        {
           
            return await  StudentDetails().AsNoTracking().ToListAsync();  
        }

        // Search student by id 
        public async Task <Student?> SearchStudentById(int StudentId) 
        {
           
            return await StudentDetails().AsNoTracking().
                FirstOrDefaultAsync(s => s.StudentID == StudentId);
        }

        // Search student by Name

         public async Task <List<Student>> SearchStudentsByName(string StrSearch)  
         {
                 return await StudentDetails().AsNoTracking().
                 Where(s => EF.Functions.Like(s.Name, $"%{StrSearch}%")).ToListAsync();
         }

        //Search student by skills name 
        public async Task< List<Student> > GetStudentsBySkillName(string StrSearch) 
        {    
            return await StudentDetails().AsNoTracking().
           Where(st => st.Skills.Any(ss => EF.Functions.Like(ss.Skill.Name, $"%{StrSearch}%"))).ToListAsync();
        }

        //Search student by Department  name
        public async Task<List<Student>> GetStudentByDepartmentName(string StrSearch) 
        {
            return await StudentDetails().AsNoTracking().
            Where(s =>  EF.Functions.Like(s.Department.Name, $"%{StrSearch}%")).ToListAsync();
        }
        // Search student by specialty name
        public async Task<List<Student>> GetStudentsBySpecialtyName(string StrSearch) 
        {
            return await StudentDetails().AsNoTracking().
            Where(s => s.Specialty != null && EF.Functions.Like(s.Specialty.Name, $"%{StrSearch}%")).ToListAsync();
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
                if (Student == null)  return false;
                   await DeleteAllStudentSkill(id, false);
                    await DeletePortfolioItem(id, false);
                    await DeleteAllStudentReportByStudentId(id, false);
                    _context.Students.Remove(Student);
                    await _context.SaveChangesAsync();
                    DeleteImage(Student.ProfileImagePath );
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
        public async Task <List<PortfolioItem>> GetPortfolioItem(int studentId) 
        {
            //(pi)=> PortfolioItems
            return await _context.PortfolioItems.Where(pi => pi.StudentID == studentId).ToListAsync();
        }
        //delete all PortfolioItems by student id 
       public async Task DeletePortfolioItem(int studentId , bool saveChange = true)
        {
            var PortfolioItems = await GetPortfolioItem(studentId);
            
                _context.PortfolioItems.RemoveRange(PortfolioItems);
            if (saveChange) { await _context.SaveChangesAsync(); }
               

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
        public async Task DeleteStudentSkills(StudentSkill StudentSkill )
        {
           _context.StudentSkills.Remove(StudentSkill);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> DeleteStudentSkill(int studentId, string studentSkillId)
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

        // delete  all Student Skills By student id 
        public async Task DeleteAllStudentSkill(int studentId , bool saveChange =true )
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
        public async  Task< List<Skill> > GetAllSkills() 
        {
          return  await _context.Skills.AsNoTracking().ToListAsync();
        }

        //get all specialty
        public async Task<List<Specialty>> GetAllSpecialty()
        {
            return await _context.Specialties.AsNoTracking().ToListAsync();
        }

        //get skill by search name 
        public async Task<List<Skill>> SearchSkillsByName( string strSearch)
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
        public async Task <TrainingApplication?> GetApplicationById(int id ) 
            {
              return await  _context.TrainingApplications.AsNoTracking().
                Include(tp=>tp.TrainingOpportunity).
                FirstOrDefaultAsync(tp => tp.ApplicationID == id);
            }

        // get all Training Application for student by student Id
        public async Task<List<TrainingApplication>> GetAllApplicationByStudentId(int studentId)
        {
            return await _context.TrainingApplications.AsNoTracking().
                Include(tp => tp.TrainingOpportunity).Where(tp => tp.StudentID == studentId).ToListAsync();
        }

        // Get all  Training Terms for all Opportunities
        public async Task<List<TrainingTerm>> GetAllTrainingTerm()
        {
            return await _context.TrainingTerms.AsNoTracking().Include(tt => tt.Opportunities).ToListAsync();
        }
        //search for an Opportunity by its title
        public async Task <List<TrainingOpportunity>> GetOpportunityByTitle(string strSearch)
        {
            return await _context.TrainingOpportunities.AsNoTracking().Where(to => EF.Functions.Like(to.Title, $"%{strSearch}%")).ToListAsync();
            
        }
        // Get  Training Placement By Id 
        public async Task <TrainingPlacement?> GetTrainingPlacementById(int id)
        {
            return await _context.TrainingPlacements.AsNoTracking().FirstOrDefaultAsync(tp => tp.PlacementID == id);
        }

        // Retrieve the training term for a specific opportunity
        public async Task<TrainingTerm ? > GetTrainingTermByOpportunityId(int id ) 
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
           FirstOrDefaultAsync(se => se.TrainingPlacement.PlacementID == TrainingPlacementId && se.TrainingPlacement.StudentID== studentId);   
        }
        // View all evaluations for the student based on the student ID number 
        public async Task<List<StudentEvaluation>> GetAllEvaluationByStudentId(int studentId)
        {
            return await _context.StudentEvaluations.Include(se => se.TrainingPlacement).
            Where(se => se.TrainingPlacement.StudentID == studentId).ToListAsync();
        }

        // Show attendance records for the training opportunities you participated in
        public async Task <List<AttendanceRecord>> GetAllAttendanceRecordByStudentId(int studentId)
        {
            return await _context.AttendanceRecords.Include(ar => ar.TrainingPlacement).
            Where(se => se.TrainingPlacement.StudentID == studentId).ToListAsync();
        }
        // View the attendance schedule for a specific training program
        public async Task<List<AttendanceRecord>> GetAttendanceRecord(int PlacementID , int StudentId )
        {
            return await _context.AttendanceRecords.Include(ar => ar.TrainingPlacement).
            Where(se => se.TrainingPlacement.PlacementID == PlacementID && se.TrainingPlacement.StudentID == StudentId).ToListAsync();
        }
        // remove  student image 
        private void DeleteImage(string imgPath)
        {
            if (string.IsNullOrEmpty(imgPath)) return;
            var filePath = Path.Combine(_environment.WebRootPath, imgPath.TrimStart('/'));
            if (File.Exists(filePath)) { File.Delete(filePath); }
        }
        // Delete Cv File
        private void DeleteCvFile(string CvPath)
        {
            if (string.IsNullOrEmpty(CvPath)) return;
            var filePath = Path.Combine(_environment.WebRootPath, CvPath.TrimStart('/'));
            if (File.Exists(filePath)) { File.Delete(filePath); }
        }

        // View the evaluation for a specific opportunity based on the opportunity name and student ID
        public async Task<List<StudentEvaluation>> SearchEvaluationsByOpportunityName(int studentId , string strSearch) { 
        return await _context.StudentEvaluations.Include(se=>se.TrainingPlacement).
                ThenInclude(tp=>tp.TrainingOpportunity).
                Where
                (
                se => se.TrainingPlacement.StudentID == studentId && 
                EF.Functions.Like(se.TrainingPlacement.TrainingOpportunity.Title, $"%{strSearch}%")

                ).ToListAsync();
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
        public async Task DeleteAllStudentReportByStudentId(int id , bool saveChange =true)
        {
            var StudentReports =  await _context.StudentReports.Where(sr => sr.StudentID == id).ToListAsync();
          
                _context.StudentReports.RemoveRange(StudentReports);
            if (saveChange) { await _context.SaveChangesAsync(); }    
        }

        //delete a report by report id 
        public async Task<bool> DeleteStudentReportByReportId(int id)
        {
            var StudentReport = await  _context.StudentReports.FirstOrDefaultAsync(sr => sr.ReportID == id);
            if (StudentReport != null )
            {
                 DeleteReportFile(StudentReport.FilePath);
                _context.StudentReports.Remove(StudentReport);
                await _context.SaveChangesAsync();
                return true;

            }
            return false;
        }
        // Get Active Students 
        public async Task<List<Student>> GetActiveStudents() 
        {
          return await StudentDetails().AsNoTracking().
                 Where(s => s.Status == "Active").ToListAsync();  
        }
        // Get InActive  Students
        public async Task<List<Student>> GetInActiveStudents()
        {
            return await StudentDetails().AsNoTracking().
                   Where(s => s.Status == "InActive").ToListAsync();
        }
        //Get Graduated Students
        public async Task<List<Student>> GetGraduatedStudents()
        {
            return await StudentDetails().AsNoTracking().
                   Where(s => s.Status == "Graduated").ToListAsync();
        }
        //delete report File  
        private void DeleteReportFile(string ReportPath)
        {
            if (string.IsNullOrEmpty(ReportPath)) return;
            var filePath = Path.Combine(_environment.WebRootPath, ReportPath.TrimStart('/'));
            if (File.Exists(filePath)) { File.Delete(filePath); }
        }
        // Get approved applications for a student by student ID
        public async Task <List<TrainingApplication>> GetApprovedApplicationsByStudentId(int studentId ) 
        { 
           return await _context.TrainingApplications.Include(tp=>tp.TrainingOpportunity).
                AsNoTracking().Where(tp=> tp.StudentID == studentId && tp.Status== "Approved").ToListAsync();      
        }
        // Get pending applications for a student by student ID
        public async Task<List<TrainingApplication>> GetPendingApplicationsByStudentId(int studentId)
        {
            return await _context.TrainingApplications.Include(tp => tp.TrainingOpportunity).
                 AsNoTracking().Where(tp => tp.StudentID == studentId && tp.Status == "Pending").ToListAsync();
        }

        // Get rejected applications for a student by student ID
        public async Task<List<TrainingApplication>> GetRejectedApplicationsByStudentId(int studentId)
        {
            return await _context.TrainingApplications.Include(tp => tp.TrainingOpportunity).
                 AsNoTracking().Where(tp => tp.StudentID == studentId && tp.Status == "rejected").ToListAsync();
        }
        public async Task<List<TrainingApplication>> GetApplicationsByStudentIdAndStatus(int studentId, string status)
        {
            return await _context.TrainingApplications
                .Include(tp => tp.TrainingOpportunity)
                .AsNoTracking()
                .Where(tp => tp.StudentID == studentId && EF.Functions.Like(tp.Status, $"%{status}%"))
                .ToListAsync();
           
        }


    }
}
