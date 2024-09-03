using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using Newtonsoft.Json;
using PrecisionFullCoilHMI.Data;
using PrecisionFullCoilHMI.Models;
using PrecisionFullCoilHMI.Services;

namespace PrecisionFullCoilHMI.Controllers
{
    public class Main : Controller
    {
        private readonly OPCUAService _opcuaService;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IBackupService _backupService;

        public Main(OPCUAService opcuaService, ApplicationDbContext context, IWebHostEnvironment hostingEnvironment, IBackupService backupService)
        {
            _opcuaService = opcuaService;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _backupService = backupService;
        }
        public IActionResult Index()
        {
      
            return View();
        }
        public IActionResult ManualOperation()
        {
            return View();
        }

        [HttpPost]
        public IActionResult JogCommand(string nodeId, bool jogStatus)
        {
            try
            {
                // Assuming you have a service that handles OPC UA writes
                _opcuaService.SetDigitalOutputJog(nodeId, jogStatus);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        public IActionResult BackupRestore()
        {
            return View();
        }


        [HttpPost]
        public IActionResult UpdateJobField(string nodeId, string value)
        {
            try
            {
                object parsedValue;

                // Attempt to parse the value into the appropriate type
                if (short.TryParse(value, out short intValue))
                {
                    parsedValue = intValue;
                }
                else if (float.TryParse(value, out float floatValue))
                {
                    parsedValue = floatValue;
                }
                else
                {
                    // Default to treating the value as a string
                    parsedValue = value;
                }

                // Assuming you have a service that handles writing values to the PLC
                short d = 5;
                _opcuaService.SetVariableValue(nodeId, parsedValue);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }




        //***************************** Backup
        [HttpPost]
        public IActionResult ExportData(string backupFileName)
        {
            try
            {
                string backupDirectory = Path.Combine(_hostingEnvironment.WebRootPath, "backups");
                if (!Directory.Exists(backupDirectory))
                {
                    Directory.CreateDirectory(backupDirectory);
                }

                string filePath = Path.Combine(backupDirectory, $"{backupFileName}.bak");

                _backupService.ExportData(filePath);

                TempData["Message"] = "Backup successful!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error during backup: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RestoreData(IFormFile restoreFile)
        {
            if (restoreFile != null && restoreFile.Length > 0)
            {
                try
                {
                    string uploadsDirectory = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");

                    // Ensure the directory exists
                    if (!Directory.Exists(uploadsDirectory))
                    {
                        Directory.CreateDirectory(uploadsDirectory);
                    }

                    string filePath = Path.Combine(uploadsDirectory, restoreFile.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await restoreFile.CopyToAsync(stream);
                    }

                    // Logic to restore data from the file
                    _backupService.RestoreData(filePath);

                    TempData["Message"] = "Restore successful!";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error during restore: {ex.Message}";
                }
            }
            else
            {
                TempData["Error"] = "Please select a valid file to restore.";
            }

            return RedirectToAction("BackupRestore");
        }



    }
}
