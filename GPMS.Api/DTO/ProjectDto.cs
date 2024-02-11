namespace GPMS.Api.DTO
{
    public class ProjectDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Year { get; set; }

        public IFormFile Images { get; set; }

        public int DeptID { get; set; }

    }
}
