namespace Sigaa.Api.Common.Scraping.Browsing.Sessions.Strategies;

internal interface ISessionStrategyProvider
{
    T GetStrategy<T>() where T : ISessionStrategy;
}