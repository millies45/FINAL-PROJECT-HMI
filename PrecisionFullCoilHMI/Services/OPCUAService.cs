using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Opc.Ua;
using PrecisionFullCoilHMI.Data;
using PrecisionFullCoilHMI.Models;
using System;
using System.Threading.Tasks;

namespace PrecisionFullCoilHMI.Services
{
    public class OPCUAService
    {
        private readonly IHubContext<RealTimeHub> _hubContext;
        private readonly OPCUAClient _opcuaClient;
        public ApplicationDbContext _context { get; set; }
        public OPCUAService(IHubContext<RealTimeHub> hubContext, OPCUAClient opcuaClient, ApplicationDbContext context)
        {
            _hubContext = hubContext;
            _opcuaClient = opcuaClient;
            _opcuaClient.NodeValueChanged += OpcuaClient_NodeValueChanged;
            _context = context;
        }

        private void OpcuaClient_NodeValueChanged(object sender, NodeValueChangedEventArgs e)
        {
            var ss = e.NodeId.ToString();
            _hubContext.Clients.All.SendAsync("UpdateVariable", e.NodeId, e.NewValue);
            //switch (e.NodeId.ToString())
            //{
            //    case "ns=4;s=v1":
            //        _hubContext.Clients.All.SendAsync("UpdateVariable", "v1", e.NewValue);
            //        break;
            //    case "ns=4;s=v2":
            //        _hubContext.Clients.All.SendAsync("UpdateVariable", "v2", e.NewValue);
            //        break;
            //    default:
            //        Console.WriteLine($"Unhandled node {e.NodeId} with value {e.NewValue}");
            //        break;
            //}
        }

        public async Task SubscribeToNodes()
        {
            if (!_opcuaClient.SessionAvailable())
            {
                var connected = await _opcuaClient.ConnectWithoutSecurityAsync();
                if (!connected)
                {
                    throw new Exception("Failed to connect to OPC UA server.");
                }
            }

            // Fetch all tags from the database that need to be subscribed
            var tagsToSubscribe = await _context.Tags
                .Where(tag => tag.Subscribe == true)
                .ToListAsync();

            foreach (var tag in tagsToSubscribe)
            {
                if (tag.IsArray && tag.ArrayStart.HasValue && tag.ArrayEnd.HasValue)
                {
                    // Subscribe to array range
                    for (int i = tag.ArrayStart.Value; i <= tag.ArrayEnd.Value; i++)
                    {
                        
                        //string nodeId = $"ns=6;s=Arp.Plc.Eclr/maintask.Tss[{i}].DuctType";
                        string nodeId = tag.NodeId.Replace("*", i.ToString());
                        _opcuaClient.AddNode(nodeId);
                        _opcuaClient.AddMonitoredItem(nodeId, 250, 250); // Adjust sampling/publishing intervals as needed
                    }
                }
                else
                {
                    // Subscribe to a single node
                    if (tag.NodeId.Contains("Position"))
                    {
                        Console.WriteLine("sss");
                    }
                    _opcuaClient.AddNode(tag.NodeId);
                    _opcuaClient.AddMonitoredItem(tag.NodeId, 250, 250); // Adjust sampling/publishing intervals as needed
                }
            }
        }

        public List<DigitalInput> GetDigitalInputs()
        {
            var di = new List<DigitalInput>();
            return di;
        }

        public List<DigitalOutput> GetDigitalOutputs()
        {
            var dov = new List<DigitalOutput>();
            return dov;
        }

        public List<EncoderInput> GetEncoderInputs()
        {
            var di = new List<EncoderInput>();
            return di;
        }

        public void SetDigitalOutputJog(string outputName, bool jogStatus)
        {
           _opcuaClient. WriteNodeValue(outputName, jogStatus);
            Console.WriteLine(outputName +"    -    "+ jogStatus
            );
        }

        public void SetVariableValue(string outputName, object variableValue)
        {
            _opcuaClient.WriteNodeValue(outputName, variableValue);
        }

        public async Task<object> ReadNodeValueAsync(string outputName)
        {
            return await _opcuaClient.ReadNodeValueAsync(outputName);
        }
    }

}
