using GPMS.Api.DTO;
using GPMS.Core.Models;
using GPMS.Repository.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _context.Projects.Include(p => p.Department).ToListAsync();
            return Ok(items);
        }

        

        [HttpGet("ProjectsDepartment/{deptid}")]
        public async Task<IActionResult> GetItemsByDepartment(int deptid)
        {
            var items = await _context.Projects.Include(p => p.Department).Where(x => x.DeptID == deptid).ToListAsync();
            if (items == null || !items.Any())
            {
                return NotFound($"No items found for department with id {deptid}!");
            }
            return Ok(items);
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
                DeptID = mdl.DeptID
            };
            await _context.Projects.AddAsync(item);
            await _context.SaveChangesAsync();
            return Ok(item);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var item = await _context.Projects.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                return NotFound($"Item with id {id} not found!");
            }
            _context.Projects.Remove(item);
            await _context.SaveChangesAsync();
            return Ok(item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromForm] ProjectDto mdl)
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
