using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FilePublisherService.Services; // Ensure this matches the correct namespace

namespace FilePublisherService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilePublisherController : ControllerBase
    {
        private readonly FilePublisherServiceClass _filePublisherService; // Renamed class

        public FilePublisherController(FilePublisherServiceClass filePublisherService)
        {
            _filePublisherService = filePublisherService;
        }

        [HttpPost("publish")]
        public IActionResult PublishFileData([FromBody] string filePath)
        {
            try
            {
                _filePublisherService.PublishFileData(filePath);
                return Ok("File data published successfully.");
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
