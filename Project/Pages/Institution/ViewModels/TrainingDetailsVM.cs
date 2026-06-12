using Project.Models;



namespace Project.Pages.Institution.ViewModels

{

    public class TrainingDetailsVM

    {

        public TrainingOpportunity Opportunity { get; set; } = null!;

        public List<EnrolledTraineeVM> EnrolledTrainees { get; set; } = new();

        public List<string> Technologies { get; set; } = new();

        public string Category { get; set; } = string.Empty;

        public string TargetGroup { get; set; } = string.Empty;

        public string SupervisorName { get; set; } = string.Empty;

        public string StatusLabel { get; set; } = string.Empty;

        public int EnrolledCount { get; set; }

        public int AvailableSeats { get; set; }

        public int FillRatePercent { get; set; }

        public string DurationText { get; set; } = string.Empty;



        public static TrainingDetailsVM FromOpportunity(TrainingOpportunity opportunity, List<string> skillNames)

        {
            var activePlacements = opportunity.TrainingPlacement.ToList();
            //var activePlacements = opportunity.TrainingPlacement

            //    .Where(tp => tp.Status == TrainingPlacement.PlacementStatus.InProgress || tp.Status == TrainingPlacement.PlacementStatus.Completed || tp.Status == TrainingPlacement.PlacementStatus.Completed)

            //    .ToList();



            var enrolledCount = activePlacements.Count;

            var availableSeats = Math.Max(0, opportunity.Capacity - enrolledCount);

            var fillRate = opportunity.Capacity > 0

                ? (int)Math.Round(enrolledCount * 100.0 / opportunity.Capacity)

                : 0;



            var requestSpecialties = opportunity.Request?.Specialties

                .Select(rs => rs.Specialty)

                .Where(s => s != null)

                .ToList() ?? new List<Specialty>();



            var opportunitySpecialties = opportunity.OpportunitySpecialties

                .Select(os => os.Specialty)

                .Where(s => s != null)

                .ToList();



            var allSpecialties = requestSpecialties.Any() ? requestSpecialties : opportunitySpecialties;



            var category = allSpecialties

                .Select(s => s!.Category)

                .FirstOrDefault(c => !string.IsNullOrWhiteSpace(c)) ?? "عام";



            var targetGroup = allSpecialties.Any()

                ? string.Join("، ", allSpecialties.Select(s => $"طلاب {s!.Name}"))

                : "جميع التخصصات";



            var technologies = skillNames.Any()

                ? skillNames

                : opportunity.Request?.Skills

                    .Select(rs => rs.Skill?.Name)

                    .Where(name => !string.IsNullOrWhiteSpace(name))

                    .Select(name => name!)

                    .Distinct()

                    .ToList() ?? new List<string>();



            var supervisorName = activePlacements

                .Select(tp => tp.InstitutionSupervisor?.FullName)

                .FirstOrDefault(name => !string.IsNullOrWhiteSpace(name))

                ?? opportunity.InstitutionOfficer?.FullName

                ?? string.Empty;



            return new TrainingDetailsVM

            {

                Opportunity = opportunity,

                EnrolledTrainees = activePlacements

                    .Select(EnrolledTraineeVM.FromPlacement)

                    .ToList(),

                Technologies = technologies,

                Category = category,

                TargetGroup = targetGroup,

                SupervisorName = supervisorName,

                StatusLabel = GetStatusLabel(opportunity, enrolledCount),

                EnrolledCount = enrolledCount,

                AvailableSeats = availableSeats,

                FillRatePercent = fillRate,

                DurationText = GetDurationText(opportunity.StartDate, opportunity.EndDate)

            };

        }



        public static string GetDurationText(DateTime startDate, DateTime endDate)

        {

            var months = ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month;

            if (months <= 0)

            {

                var days = (endDate - startDate).Days;

                return days > 0 ? $"{days} يوم" : "غير محدد";

            }



            return $"{months} شهر";

        }



        public static string GetStatusLabel(TrainingOpportunity opportunity, int activePlacementCount)

        {

            if (activePlacementCount > 0)

                return "نشط";



            return opportunity.Status switch

            {

                TrainingOpportunity.Opportunity.Open => "مفتوحة",

                TrainingOpportunity.Opportunity.Closed => "مغلقة",

                TrainingOpportunity.Opportunity.Cancelled => "منتهية",

                _ => opportunity.Status.ToString()

            };

        }

    }



    public class EnrolledTraineeVM

    {

        public int StudentId { get; set; }

        public int PlacementId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Major { get; set; } = string.Empty;

        public string AcademicYear { get; set; } = string.Empty;

        public string? ProfileImagePath { get; set; }



        public static EnrolledTraineeVM FromPlacement(TrainingPlacement placement)

        {

            var student = placement.Student;

            return new EnrolledTraineeVM

            {

                StudentId = student.StudentID,

                PlacementId = placement.PlacementID,

                Name = student.Name,

                Major = student.Specialty?.Name ?? student.Department?.Name ?? string.Empty,

                AcademicYear = student.Level ?? string.Empty,

                ProfileImagePath = student.ProfileImagePath

            };

        }

    }

}


