using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DataValidation.DataSanitizer
{
    public class DataSanitizerLogic
    {
        private readonly List<string> _forbiddenWords = new List<string>
        {
         "drop database", "delete from", "truncate table", "--", ";--", "/*", "*/", "@@", "char", "nchar",
         "varchar", "nvarchar", "alter", "begin", "cast", "create", "cursor", "declare", "exec", "execute",
         "fetch", "insert", "kill", "open", "select", "sys", "sysobjects", "syscolumns", "table", "update"
        };

        public (bool IsValid, string SanitizedData) ValidateAndSanitize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return (false, string.Empty);

            string sanitizedInput = input.Trim();

            foreach (var word in _forbiddenWords)
            {
                if (sanitizedInput.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return (false, string.Empty); 
                }
            }

            return (true, sanitizedInput);
        }
    }
}
