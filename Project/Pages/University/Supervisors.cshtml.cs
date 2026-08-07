using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Extensions;
using Project.Models;
using Project.Repostory;

namespace Project.Pages.University
{
    public class SupervisorsModel : PageModel
    {
        private readonly UniversityRepository _universityRepository;

        public SupervisorsModel(UniversityRepository universityRepository)
        {
            _universityRepository = universityRepository;
        }

        public PaginatedList<AspNetUser> Supervisors { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        public async Task OnGetAsync()
        {
            var query = _universityRepository.GetUniversitySuperVisorBy();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(x =>
                    x.FullName.Contains(SearchTerm) ||
                    x.Email.Contains(SearchTerm));
            }

            Supervisors = await PaginatedList<AspNetUser>
                .CreateAsync(query, PageSize, PageNumber);
        }
    }
}
