using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UpdatingProjects.Models;

namespace UpdatingProjects.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsDbsController : ControllerBase
    {
        private readonly UpdatingDbContext _context;
        private readonly IConfiguration _config;

        public ProjectsDbsController(UpdatingDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        
        [HttpGet]
        public async Task<IEnumerable<Tuple<string, IEnumerable<Tuple<string, IEnumerable<ProjectsDb>>>>>> GetProjectsDbs()
        {
            var projects = await _context.ProjectsDbs.ToListAsync();
            var byProjectCategoryAndTitle =
                projects.GroupBy(pc => pc.ProjectCategory)
                        .Select(pcl => new Tuple<string, IEnumerable<Tuple<string, IEnumerable<ProjectsDb>>>>(pcl.Key, 
                            pcl.ToList().GroupBy(pt => pt.ProjectTitle)
                                        .Select(ptl => new Tuple<string, IEnumerable<ProjectsDb>>(ptl.Key,
                                        ptl.OrderBy(ik => ik.ImageCategory).ToList()))
                         ));
                
            return byProjectCategoryAndTitle;                
               
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectsDb>> GetProjectsDb(int id)
        {
            var projectsDb = await _context.ProjectsDbs.FindAsync(id);

            if (projectsDb == null)
            {
                return NotFound();
            }

            return projectsDb;
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProjectsDb(int id, [FromForm] ProjectsDb projectsDb)
        {
            try
            {
                projectsDb.Id = id;
                _context.Entry(projectsDb).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return StatusCode(StatusCodes.Status202Accepted);

            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }           
        }
        
        [HttpPost]
        public async Task<IActionResult> PostProjectsDb([FromForm] ProjectsDb projectsDb)
        {
            try
            {
                string imageUrlForDb = await SaveImage(projectsDb.ImageFile, projectsDb.ImageName, projectsDb.ProjectCategory);

                if (imageUrlForDb != null)
                {
                    projectsDb.Url = imageUrlForDb;
                    _context.ProjectsDbs.Add(projectsDb);
                    await _context.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status201Created);
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }                     
        }
       
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProjectsDb(int id)
        {
            try
            {
                var projectToDelete = await _context.ProjectsDbs.FindAsync(id);
                if (projectToDelete == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }

                string fileDeleted = await DeleteImage(projectToDelete.Url);

                if (fileDeleted != null)
                {
                    _context.ProjectsDbs.Remove(projectToDelete);
                    await _context.SaveChangesAsync();

                    return StatusCode(StatusCodes.Status202Accepted);
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            } 
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }           
        }

        private bool ProjectsDbExists(int id)
        {
            return _context.ProjectsDbs.Any(e => e.Id == id);
        }
        public async Task<string>  SaveImage(IFormFile imageFile, string imageName, string projectCategory)
        {              
            string imageFileName = DateTime.Now.ToString("yyMMddHHmmssf") + "_" + imageName;
            string imageUrlForDb = $"/projects-images/{projectCategory}/{imageFileName}";
            string imageFilePath = Path.Join(_config["BaseFilePath"], "projects-images", projectCategory, imageFileName);
            using (var fileStream = new FileStream(imageFilePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            return imageUrlForDb;                   
        }

        public async Task<string> DeleteImage(string url)
        {
            return await Task.Run(() =>
            {                
                url.Replace("/", "\\");
                string fileToDelete = Path.Join(_config["BaseFilePath"], url);
                if (System.IO.File.Exists(fileToDelete))
                {
                    System.IO.File.Delete(fileToDelete);
                    return "Success";
                }
                else
                {
                    return null;
                }               
            });
        }
    }
}
