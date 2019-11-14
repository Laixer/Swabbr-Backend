using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.Options
{
    public enum ConnectionStringMode
    {
        Azure,
        Emulator
    }

    public class ConnectionStringOptions
    {
        public ConnectionStringMode Mode { get; set; }
        public string Azure { get; set; }
        public string Emulator { get; set; }

        public string ActiveConnectionString =>
            Mode == ConnectionStringMode.Azure ? Azure : Emulator;
    }
}