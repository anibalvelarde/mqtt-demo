using System;

namespace mqtt.demo
{
	internal class Site
	{
		private readonly List<Machine> assets = new List<Machine>();
		public string Name { get; }
		public string Address { get; }
		public string ErpAssetCode { get; }
		public DateTime CreaedDate { get; }
		public IEnumerable<Machine> Machines {
			get { return assets; }
		}

		public Site(string name, string address, int machines, string assetTag)
		{
			ErpAssetCode = assetTag ?? throw new ArgumentNullException(nameof(assetTag));
			Name = name;
			Address = address;
			CreaedDate = DateTime.UtcNow;

            for (int i = 0; i < machines; i++)
            {
				assets.Add(Machine.MachineFactory(
					$"{MakeupBrand()}-{string.Format("{0:0000}",DateTime.UtcNow.Millisecond)}",
					$"{assetTag}-mchn-{string.Format("{0:000}", DateTime.UtcNow.Millisecond)}"));
			}
		}

        private string MakeupBrand()
        {
			var rnd = new Random();
			var luckyRoll = rnd.Next(1, 10);
            string? brand = luckyRoll switch
            {
                1 or 2 => "HAAS",
                3 or 4 or 5 => "Samsumg",
                8 or 9 => "WayCool",
                _ => "ElCheapo",
            };
            return brand;

		}
    }
}