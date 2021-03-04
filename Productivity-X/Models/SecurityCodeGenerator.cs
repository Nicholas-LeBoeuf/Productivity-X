using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace Productivity_X.Models
{
	public class SecurityCodeGenerator
	{
		// Instantiate random number generator.  
		private readonly Random _random = new Random();

		// Generates a random string with a given size.    
		private string RandomString(int size, bool lowerCase = false)
		{
			var builder = new StringBuilder(size);

			// Unicode/ASCII Letters are divided into two blocks
			// (Letters 65–90 / 97–122):   
			// The first group containing the uppercase letters and
			// the second group containing the lowercase.  

			// char is a single Unicode character  
			char offset = lowerCase ? 'a' : 'A';
			const int lettersOffset = 26; // A...Z or a..z: length = 26  

			for (var i = 0; i < size; i++)
			{
				var @char = (char)_random.Next(offset, offset + lettersOffset);
				builder.Append(@char);
			}

			return lowerCase ? builder.ToString().ToLower() : builder.ToString();
		}
		// Generates a random number within a range.      
		private int RandomNumber(int min, int max)
		{
			return _random.Next(min, max);
		}

		public string GetSecurityCode()
		{
			var passwordBuilder = new StringBuilder();

			// 4-Letters lower case   
			passwordBuilder.Append(RandomString(2, true));

			// 4-Digits between 1000 and 9999  
			passwordBuilder.Append(RandomNumber(1000, 9999));

			// 2-Letters upper case  
			passwordBuilder.Append(RandomString(2));
			return passwordBuilder.ToString();
		}
	}
}
