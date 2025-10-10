namespace Sigapi.Scraping.Browsing.Sessions.Strategies;

internal interface ISessionStrategyProvider
{
    T GetStrategy<T>() where T : ISessionStrategy;
}