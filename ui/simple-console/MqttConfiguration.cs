namespace mqtt.demo
{
    public class MqttConfiguration
    {
        public MqttConfiguration(string? ipAddress, int port, string? topic)
        {
            Subscribe(topic ?? String.Empty);
            IpAddress = ipAddress;
            Port = port;
            MyClientId = $"mqtt.demo.id-{DateTime.UtcNow.Millisecond}";
        }
        public void Subscribe(string? newTopic)
        {
            if (string.IsNullOrEmpty(newTopic))
            {
                throw new ArgumentNullException(nameof(newTopic), "New Topic must not be null or empty");
            } else
            {
                Topic = newTopic;
            }
        }
        public string? Topic { get; private set; }
        public string? IpAddress { get; set; }
        public int Port { get; set; }
        public string? MyClientId { get; }
        public override string ToString()
        {
            return $"Svr: {IpAddress} Port: {Port} ClientId: {MyClientId}";
        }
    }
}
