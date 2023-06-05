using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GabinetePsicologia.Shared
{
    public class ChatPerson
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool hasNotViewMessage { get; set; } = false;
        public DateTime? lastMessage { get; set; }
        public bool isOnline { get; set; } = false;
        public string? UserName { get; set; }
        
    }
}
