using BusBooking.Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BusBooking.Backend.Services
{
    public class SeatLockBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public SeatLockBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    
                    var expiredLocks = await context.SeatLocks
                        .Where(sl => sl.ExpiryTime < DateTime.UtcNow)
                        .ToListAsync(stoppingToken);

                    if (expiredLocks.Any())
                    {
                        context.SeatLocks.RemoveRange(expiredLocks);
                        await context.SaveChangesAsync(stoppingToken);
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
