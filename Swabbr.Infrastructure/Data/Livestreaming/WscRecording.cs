﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Swabbr.Infrastructure.Data.Livestreaming
{
    public partial class WscRecording
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("transcoder_id")]
        public string TranscoderId { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
