using HotChocolate.Types;

namespace Gavinhow.SpotifyStatistics.Web.Types;

[QueryType]
public class Query
{
  public static Book GetBook()
    => new Book("C# in depth.", new Author("Jon Skeet"));
}