using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mqtt.demo
{
    internal class Machine
    {
        private DateTime lastStartTimestamp = DateTime.MinValue;

        public static Machine MachineFactory(string brand, string assetTag)
        {
            return new Machine(brand, assetTag);
        }

        private Machine(string brand, string assetTag)
        {
            ErpAssetTag = assetTag ?? throw new ArgumentNullException(nameof(assetTag));    
            Brand = brand ?? "** No Name Machine **";
            IsRunning = false;
            TotalRunningMinutes = 0;
        }
        public string Brand { get; }
        public string ErpAssetTag { get; }
        public bool IsRunning { get; private set; }
        public int TotalRunningMinutes { get; private set; }
        public void Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                lastStartTimestamp = DateTime.UtcNow;
            }
        }
        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning=false;
                TotalRunningMinutes += (DateTime.UtcNow - lastStartTimestamp).Minutes;
            }
        }
    }
}
