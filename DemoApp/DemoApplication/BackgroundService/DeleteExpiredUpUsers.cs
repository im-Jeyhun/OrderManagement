using DemoApplication.Database;

namespace DemoApplication.BackgroundService
{
    public class DeleteExpiredUpUsers : IHostedService, IDisposable
    {
        private ILogger<DeleteExpiredUpUsers> _logger;

        public IServiceProvider Services { get; }

        private Timer _timer;

        public DeleteExpiredUpUsers(IServiceProvider service, ILogger<DeleteExpiredUpUsers> logger)
        {
            Services = service;
            _logger = logger;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {

            Console.WriteLine($"{nameof(DeleteExpiredUpUsers)} is startet");

            _timer = new Timer(DeleteUsers, null, TimeSpan.Zero,TimeSpan.FromSeconds(20));

            return Task.CompletedTask;
        }

        private void DeleteUsers(object state)
        {

            using IServiceScope scope = Services.CreateScope();

            var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            try
            {

                var tokens = dataContext.UserActivations.OrderBy(t => t.TokenExpireDate < DateTime.Now).Select(t => t.User).ToList();

                foreach (var deletedUser in tokens)
                {
                    if (deletedUser!.IsEmailConfirmed == false)
                    {
                        dataContext.Users.Remove(deletedUser);

                    }
                    else
                    {
                        continue;
                    }
                }

                dataContext.SaveChanges();

            }
            catch (Exception e)
            {
                _logger.LogWarning($"Something went wrong while {nameof(DeleteExpiredUpUsers)} working");
                throw e;
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"{nameof(DeleteExpiredUpUsers)} is stoped");

            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}
