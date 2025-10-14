namespace Sigaa.Api.Common.Scraping.Browsing.Sessions.Strategies;

internal sealed class SessionStrategyProvider : ISessionStrategyProvider
{
    private readonly IServiceProvider serviceProvider;

    public SessionStrategyProvider(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }
    
    public T GetStrategy<T>() where T : ISessionStrategy => serviceProvider.GetRequiredService<T>();
}