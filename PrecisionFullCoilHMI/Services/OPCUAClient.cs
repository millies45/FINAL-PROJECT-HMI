using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using ISession = Opc.Ua.Client.ISession;

namespace PrecisionFullCoilHMI.Services
{
    public class OPCUAClient
    {
        private readonly IHubContext<RealTimeHub> _hubContext;
        private readonly OPCUAClient _opcuaClient;

        private ApplicationInstance _application;
        private Session _session;
        private string _serverUrl;
        private Subscription _subscription;

        public event EventHandler<bool> ConnectionStatusChanged;
        public event EventHandler<NodeValueChangedEventArgs> NodeValueChanged;
        private List<NodeDefinition> _nodes; // List to store nodes being monitored

        public OPCUAClient(string serverUrl)
        {
            _nodes = new List<NodeDefinition>();
            _serverUrl = serverUrl;
            _application = new ApplicationInstance
            {
                ApplicationName = "OPCUAClient",
                ApplicationType = ApplicationType.Client,
                ConfigSectionName = "Opc.Ua.Client",
            };
        }

        public async Task<bool> ConnectWithoutSecurityAsync()
        {
            try
            {
                var configFilePath = "Opc.Ua.Client.Config.xml";
                await _application.LoadApplicationConfiguration(configFilePath, false);
                await _application.CheckApplicationInstanceCertificate(false, 0);

                EndpointDescription selectedEndpoint = CoreClientUtils.SelectEndpoint(_serverUrl, false);

                if (selectedEndpoint == null)
                {
                    throw new Exception("No unsecured endpoint found.");
                }

                var endpointConfig = EndpointConfiguration.Create(_application.ApplicationConfiguration);
                var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfig);

                _session = await Session.Create(
                    _application.ApplicationConfiguration,
                    endpoint,
                    false,
                    "OPCUAClient Session",
                    60000,
                    new UserIdentity(new AnonymousIdentityToken()),
                    null);

                StartSessionMonitoring();
                OnConnectionStatusChanged(true);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection Error: {ex.Message}");
                OnConnectionStatusChanged(false);
                return false;
            }
        }

        private void StopSessionMonitoring()
        {
            // Detach the KeepAlive event handler to stop monitoring
            if (_session != null)
            {
                _session.KeepAlive -= Session_KeepAlive;
            }
        }

        private void StartSessionMonitoring()
        {
            _session.KeepAlive += Session_KeepAlive;
        }

        private void Session_KeepAlive(ISession session, KeepAliveEventArgs e)
        {
            if (!ServiceResult.IsGood(e.Status))
            {
                Console.WriteLine($"Connection lost. Status: {e.Status}");
                OnConnectionStatusChanged(false);

                StopSessionMonitoring();
                // Attempt to reconnect
                Task.Run(async () => await AttemptReconnectionAsync());
            }
        }

        private async Task AttemptReconnectionAsync()
        {
            Console.WriteLine("Attempting to reconnect...");
            OnConnectionStatusChanged(false);
            while (true)
            {
                bool connected = await ConnectWithoutSecurityAsync();
                if (connected)
                {
                    Console.WriteLine("Reconnected successfully.");
                    OnConnectionStatusChanged(true);
                    ResubscribeNodes();
                    break;
                }
                await Task.Delay(10000); // Retry every 10 seconds
            }
        }

        private void ResubscribeNodes()
        {
            foreach (var node in _nodes)
            {
                AddMonitoredItem(node.NodeId);
            }
        }

        protected virtual void OnConnectionStatusChanged(bool isConnected)
        {
            ConnectionStatusChanged?.Invoke(this, isConnected);
        }



        public NodeDefinition AddNode(string nodeIdString)
        {
            var node = new NodeDefinition(nodeIdString);
            _nodes.Add(node);

            // Add this node to the subscription to monitor its value
            //AddMonitoredItem(nodeIdString);

            return node;
        }

        public List<NodeDefinition> GetNodes()
        {
            return _nodes;
        }

        public void AddMonitoredItem(string nodeIdString, int samplingInterval = 1000, int publishingInterval = 1000)
        {
            if (_session == null || !_session.Connected)
            {
                throw new InvalidOperationException("Session is not connected or is null.");
            }

            if (_subscription == null)
            {
                _subscription = new Subscription(_session.DefaultSubscription)
                {
                    PublishingInterval = publishingInterval, // Set the publishing interval here
                    KeepAliveCount = 10,
                    LifetimeCount = 30,
                    MaxNotificationsPerPublish = 1000,
                    PublishingEnabled = true,
                    Priority = byte.MaxValue
                };
                _session.AddSubscription(_subscription);
                _subscription.Create();
            }

            var monitoredItem = new MonitoredItem(_subscription.DefaultItem)
            {
                StartNodeId = new NodeId(nodeIdString),
                AttributeId = Attributes.Value,
                SamplingInterval = samplingInterval, // Set the sampling interval here
                QueueSize = 10,
                DiscardOldest = true
            };

            monitoredItem.Notification += OnMonitoredItemNotification;
            _subscription.AddItem(monitoredItem);
            _subscription.ApplyChanges();
        }

        private void OnMonitoredItemNotification(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
        {
            foreach (var value in monitoredItem.DequeueValues())
            {
                Console.WriteLine($"Node: {monitoredItem.StartNodeId}, Value: {value.Value}");

                // Update the node's current value
                var node = _nodes.Find(n => n.NodeId == monitoredItem.StartNodeId.ToString());
                if (node != null)
                {
                    node.CurrentValue = value.Value;
                }

                // Raise the NodeValueChanged event
                OnNodeValueChanged(monitoredItem.StartNodeId, value.Value);
            }
        }

        protected virtual void OnNodeValueChanged(NodeId nodeId, object newValue)
        {
            NodeValueChanged?.Invoke(this, new NodeValueChangedEventArgs(nodeId, newValue));
        }



        public DataValue ReadNodeValue(string nodeIdString)
        {
            if (!SessionAvailable())
            {
                throw new InvalidOperationException("Session is not connected or is null.");
            }

            try
            {
                NodeId nodeId = new NodeId(nodeIdString);
                return _session.ReadValue(nodeId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Read Error: {ex.Message}");
                return null;
            }
        }

        public async Task<object> ReadNodeValueAsync(string nodeIdString)
        {
            if (!SessionAvailable())
            {
                throw new InvalidOperationException("OPC UA session is not available.");
            }

            try
            {
                NodeId nodeId = new NodeId(nodeIdString);
                var dataValue = await _session.ReadValueAsync(nodeId);
                return dataValue.Value; // This could be a bool, int, float, etc.
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Read Error: {ex.Message}");
                throw;
            }
        }

        public void WriteNodeValue(string nodeIdString, object valueToWrite)
        {
            if (!SessionAvailable())
            {
                return;
                //throw new InvalidOperationException("Session is not connected or is null.");
            }

            try
            {
                NodeId nodeId = new NodeId(nodeIdString);
                Variant value = new Variant(valueToWrite);

                //switch (valueToWrite)
                //{
                //    case bool boolValue:
                //        // Convert boolean to 1 or 0
                //        value = new Variant(boolValue); // or use `Variant(boolValue)` if OPC UA server supports bool directly
                //        break;

                //    case int intValue:
                //        value = new Variant(intValue);
                //        break;

                //    case double doubleValue:
                //        value = new Variant(doubleValue);
                //        break;

                //    case float floatValue:
                //        value = new Variant(floatValue);
                //        break;

                //    case string stringValue:
                //        value = new Variant(stringValue);
                //        break;

                //    // Add more cases for other data types as needed

                //    default:
                //        throw new InvalidOperationException($"Unsupported data type: {valueToWrite.GetType()}");
                //}

                WriteValue writeValue = new WriteValue
                {
                    NodeId = nodeId,
                    AttributeId = Attributes.Value,
                    Value = new DataValue(value)
                };

                WriteValueCollection writeValues = new WriteValueCollection { writeValue };

                StatusCodeCollection statusCodes;
                DiagnosticInfoCollection diagnosticInfos;

                _session.Write(
                    null,
                    writeValues,
                    out statusCodes,
                    out diagnosticInfos
                );

                if (statusCodes[0] != Opc.Ua.StatusCodes.Good)  // Fully qualify StatusCodes here
                {
                    Console.WriteLine($"Write operation failed with status code: {statusCodes[0]}");
                }
                else
                {
                    Console.WriteLine("Write operation succeeded.");
                }
            }
            catch (Opc.Ua.ServiceResultException ex)
            {
                Console.WriteLine($"ServiceResultException: {ex.Message}");
                OnConnectionStatusChanged(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Exception: {ex.Message}");
            }
        }

        public bool SessionAvailable()
        {
            bool available = _session != null;
            if (available)
            {
                available = _session.Connected;
            }
            return available;
        }

        public void Disconnect()
        {
            try
            {
                if (_session != null)
                {
                    _session.Close();
                    _session.Dispose();
                    _session = null;
                    OnConnectionStatusChanged(false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Disconnection Error: {ex.Message}");
            }
        }
    }

    public class NodeDefinition
    {
        public string NodeId { get; set; }
        public object CurrentValue { get; set; }

        public NodeDefinition(string nodeId)
        {
            NodeId = nodeId;
        }
    }

    public class NodeValueChangedEventArgs : EventArgs
    {
        public NodeId NodeId { get; }
        public object NewValue { get; }

        public NodeValueChangedEventArgs(NodeId nodeId, object newValue)
        {
            NodeId = nodeId;
            NewValue = newValue;
        }
    }
}
