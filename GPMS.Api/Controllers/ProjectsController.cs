using GPMS.Core.Models;
using GPMS.Repository.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GPMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProjectsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> AllItems()
        {
            var items = await _context.Projects.ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> AllItems(int id)
        {
            var item = await _context.Projects.SingleOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                return NotFound($"Item Code {id} not exists!");
            }
            return Ok(item);
        }

        [HttpGet("Project/Department/{deptid}")]
        public async Task<IActionResult> AllItemsWithCategory(int deptid)
        {
            var item = await _context.Projects.Where(x => x.DeptID == deptid).ToListAsync();
            if (item == null)
            {
                return NotFound($"Category Id {deptid} has no items!");
            }
            return Ok(item);
        }


        [HttpPost]
        public async Task<IActionResult> AddItem([FromForm] Project project)
        {
            using var stream = new MemoryStream();
            await project.Images.CopyToAsync(stream);


            var item = new Project
            {
                Title= project.Title,
                Description = project.Description,
                Year = project.Year,
                DeptID= project.Id,
                Images = stream.ToArray()
            };
            await _context.Projects.AddAsync(item);
            await _context.SaveChangesAsync();
            return Ok(item);
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var item = await _context.Projects.SingleOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                return NotFound($"  item id {id} not exists !");
            }
            _context.Projects.Remove(item);
            await _context.SaveChangesAsync();
            return Ok(item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromForm] Project mdl)
        {
            var item = await _context.Projects.FindAsync(id);
            if (item == null)
            {
                return NotFound($"Item with id {id} not found!");
            }

            var isDepartmentExists = await _context.Departments.AnyAsync(x => x.Id == mdl.DeptID);
            if (!isDepartmentExists)
            {
                return NotFound($"Department with id {mdl.DeptID} not found!");
            }

            if (mdl.Images != null)
            {
                using var stream = new MemoryStream();
                await mdl.Images.CopyToAsync(stream);
                item.Images = stream.ToArray();
            }

            item.Title = mdl.Title;
            item.Description = mdl.Description;
            item.Year = mdl.Year;
            item.DeptID = mdl.DeptID;

            await _context.SaveChangesAsync();
            return Ok(item);
        }


    }
}
