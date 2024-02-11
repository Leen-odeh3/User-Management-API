using GPMS.Api.DTO;
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

        [HttpGet("ProjectsDepartment/{deptid}")]
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
        public async Task<IActionResult> AddItem([FromForm] ProjectDto mdl)
        {
            using var stream = new MemoryStream();
            await mdl.Images.CopyToAsync(stream);
            var item = new Project
            {
                Title = mdl.Title,
                Description = mdl.Description,
                Year = mdl.Year,
                Images = stream.ToArray(),
                DeptID= mdl.DeptID
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
                return NotFound($"Item id {id} not exists !");
            }

            var isDepartmentExists = await _context.Departments.AnyAsync(x => x.Id == mdl.DeptID);
            if (!isDepartmentExists)
            {
                return NotFound($"Department id {mdl.DeptID} not exists !");
            }

            if (mdl.Images != null)
            {
                item.Images = mdl.Images;
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
