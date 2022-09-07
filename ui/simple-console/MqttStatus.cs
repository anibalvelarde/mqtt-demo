using MQTTnet.Client;

namespace mqtt.demo
{
    internal class MqttStatus
    {
        public MqttStatus(string statusText, bool isAllOk, IMqttClient? mqttClient)
        {
            Status = statusText;
            IsAllOk = isAllOk;
            MqttClient = mqttClient;
        }
        public string Status { get; }
        public bool IsAllOk { get; }
        public IMqttClient? MqttClient { get; }
        public override string ToString()
        {
            var state = MqttClient != null && MqttClient.IsConnected;
            return $"Status: {(state ? "on-line" : "off-line")}";
        }
    }
}
