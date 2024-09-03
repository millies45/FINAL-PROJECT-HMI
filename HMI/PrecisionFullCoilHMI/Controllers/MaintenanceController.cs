using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrecisionFullCoilHMI.Data;
using PrecisionFullCoilHMI.Services;

namespace PrecisionFullCoilHMI.Controllers
{
    public class MaintenanceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly OPCUAService _opcuaService;
     
        public MaintenanceController(ApplicationDbContext context, OPCUAService opcuaService)
        {
            _context = context;
            _opcuaService = opcuaService;
        }

        // GET: Maintenance
        public async Task<IActionResult> Index()
        {
            var tags = await _context.Tags.ToListAsync();
            return View(tags);
        }

        // POST: Maintenance/JogCommand
        [HttpPost]
        public IActionResult JogCommand(string nodeId, bool jogStatus)
        {
            _opcuaService.SetDigitalOutputJog(nodeId, jogStatus);
            return Ok();
        }
    }
}
