using System.Diagnostics;
using System.Linq;

namespace Armature.Core
{
  public static class QueryExtension
  {
    /// <summary>
    ///   Adds the <paramref name="query" /> into <paramref name="parentQuery" />, if <paramref name="parentQuery" /> already contains the matcher equal to
    ///   <paramref name="query" /> it does not add it but return the existent one. This method is used to add on a build plan or
    ///   make a several registrations regarding one Unit separately.
    /// </summary>
    /// <remarks>Call it first and then fill returned <see cref="IQuery" /> with build actions or perform other needed actions.</remarks>
    [DebuggerStepThrough]
    public static T AddSubQuery<T>(this IQuery parentQuery, T query, bool addOrGet = true) where T : IQuery
    {
      if(parentQuery.Children.Contains(query))
        return addOrGet
                 ? (T) parentQuery.Children.First(_ => _.Equals(query))
                 : throw new ArmatureException(string.Format("There is already matcher added '{0}'", query));

      parentQuery.Children.Add(query);
      return query;
    }
  }
}
