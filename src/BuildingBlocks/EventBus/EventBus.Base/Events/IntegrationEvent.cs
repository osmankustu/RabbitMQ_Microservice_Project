using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Events
{
    public class IntegrationEvent
    {
        public IntegrationEvent(int id, DateTime createDate)
        {
            Id = id;
            CreateDate = createDate;
        }

        [JsonConstructor]
        public IntegrationEvent()
        {
            Id = new int();
            CreateDate = DateTime.Now;
        }

        [JsonProperty]
        public int Id { get; private set; }
        [JsonProperty]
        public DateTime CreateDate { get; private set; }
    }
}
