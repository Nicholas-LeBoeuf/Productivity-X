using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Productivity_X.Models
{
	public class Categories
	{
		private int categoryid;
		private string categoryname;
		private string color;
		private string description;
		private object[] categoryData = new object[3];
		
		public Categories()
		{
		}

		public Categories(object[] savedCategoryData, int nCategoryID)
		{
			categoryData = savedCategoryData;
			//categoryid = Convert.ToInt32(categoryData.ElementAt(1).ToString());
			categoryid = nCategoryID;
			categoryname = Convert.ToString(categoryData.ElementAt(0));	
			color = Convert.ToString(categoryData.ElementAt(1));
			description = Convert.ToString(categoryData.ElementAt(2));
		}

		public string GetCategoryName()
		{
			return categoryname;
		}
	}
}
