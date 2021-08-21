using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKAvaloniaPlayer.Models
{
	public class SaveAccountModel
	{
		public string? Name { get; set; }
		public long UserID { get; set; }
		public string? Token { get; set; }
	}
}