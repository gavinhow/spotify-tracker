namespace Gavinhow.SpotifyStatistics.Web.CQRS;

public interface IQuery<in TRequest, TResult>
{
  Task<TResult> QueryAsync(TRequest request, CancellationToken cancellationToken);
}