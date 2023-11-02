using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AteaScraper.Integrations
{
    public class PublicApiResponse
    {
        public int Count { get; set; }

        public List<Entry> Entries { get; set; }
    }
}
