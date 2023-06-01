using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GabinetePsicologia.Shared
{
	public class ChatDto
	{
		public Guid Id { get; set; }
		public string IdFrom { get; set; }
		public string FromName { get; set; }
		public string IdTo { get; set; }
		public string ToName { get; set; }
		public string Message { get; set; }
		public DateTime Date { get; set; }
		public bool View { get; set; }

	}
}
