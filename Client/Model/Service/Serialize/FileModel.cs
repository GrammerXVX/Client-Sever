using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model.Service.Serialize
{
    public record FileModel
    {
        public string? FileName { get; set; }
        public string? FileType { get; set; }
    }
}
