using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Productivity_X.Models
{
	public class ToDoTasks
	{
		private int m_nTaskID;
		private string m_sTaskname;
		private bool m_bComplete;
		public ToDoTasks(int taskid, string taskname, bool finished)
		{
			m_nTaskID = taskid;
			m_sTaskname = taskname;
			m_bComplete = finished;
		}

		public string GetTaskName()
		{
			return m_sTaskname;
		}
		public bool IsComplete()
		{
			return m_bComplete;
		}
		public int GetTaskID()
		{
			return m_nTaskID;
		}
	}
}
