using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrecisionFullCoilHMI.Data;
using PrecisionFullCoilHMI.Models;
using PrecisionFullCoilHMI.Services;
using System.Linq;
using System.Threading.Tasks;


namespace PrecisionFullCoilHMI.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly OPCUAService _opcuaClient; // Assuming you have a service for OPC UA communication

        public SettingsController(ApplicationDbContext context, OPCUAService opcuaClient)
        {
            _context = context;
            _opcuaClient = opcuaClient;
        }

        public async Task<IActionResult> Index()
        {
            // Fetch only the tags with the definition 'Setting'
            var settingsTags = await _context.Tags
                .Where(tag => tag.Definition == (int)TagDefinition.Setting)
                .OrderBy(tag => tag.Id)
                .ToListAsync();

            // Read values from the OPC UA server
            foreach (var tag in settingsTags)
            {
                try
                {
                 var v = await _opcuaClient.ReadNodeValueAsync(tag.NodeId);
                        if (v != null) {
                            tag.Value = v;
                        }
                }
                catch (Exception)
                {

                    
                }
       
            }

            return View(settingsTags);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateValue(int id, object newValue)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            // Update the value in the PLC using OPC UA
            _opcuaClient.SetVariableValue(tag.NodeId, newValue);

            // Save the value in the database (if needed)
            tag.Value = newValue;
            _context.Tags.Update(tag);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RefreshValues()
        {
            // Here, add logic to read all the settings values from the PLC
            await ReadAllSettingsValuesFromPLC();
            return RedirectToAction(nameof(Index));
        }

        private async Task ReadAllSettingsValuesFromPLC()
        {
            var settingsTags = await _context.Tags
                .Where(tag => tag.Definition == (int)TagDefinition.Setting)
                .ToListAsync();
            try
            {
                foreach (var tag in settingsTags)
                {
                    tag.Value = await _opcuaClient.ReadNodeValueAsync(tag.NodeId);
                    _context.Tags.Update(tag);
                }
            }
            catch (Exception)
            {

            }



            await _context.SaveChangesAsync();
        }
    }
}
