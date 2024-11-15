using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataConsumerService.Data;
using DataConsumerService.Model;  // Add this line for the correct namespace

namespace DataConsumerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileDataController : ControllerBase
    {
        private readonly DataContext _context;

        public FileDataController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FileData>>> GetFileData([FromQuery] string name, [FromQuery] int pageNo = 1, [FromQuery] int pageSize = 10)
        {
            IQueryable<FileData> query = _context.FileData;

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(f => f.Name.Contains(name)); // Filter by name
            }

            var pagedResult = await query
                .Skip((pageNo - 1) * pageSize) // Skip the items of the previous pages
                .Take(pageSize) // Take the number of items for the current page
                .ToListAsync();

            return Ok(pagedResult);
        }
    }
}
