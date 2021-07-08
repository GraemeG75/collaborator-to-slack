using System;

namespace CollaboratorToSlack
{
	class Program
	{
		static void Main(string[] args)
		{
			using (Log log = new Log())
			{
				log.LogMessage(LogLevel.Info, "Startup", "Program.Main(string[] args)", 0);
			}
		}
	}
}
