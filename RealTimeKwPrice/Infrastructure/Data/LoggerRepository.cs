using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class LoggerRepository : ILoggerRepository
    {
        private readonly MySqlDatabase _context;

        public LoggerRepository(MySqlDatabase context)
        {
            _context = context;
        }

        public async Task LogErrorAsync(Logger logger)
        {
            await _context.Loggers.AddAsync(logger); 
            await _context.SaveChangesAsync(); 
        }
    }
}
