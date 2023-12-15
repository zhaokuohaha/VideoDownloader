using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoDownloader.Models
{
    public class VideoFormat
    {
        public string Id { get; set; }

        public string Ext { get; set; }

        public string Resolution { get; set; }

        public string FileSize { get; set; }
    }
}
