using System.Diagnostics;
using System.Linq;

namespace Armature.Core
{
  public static class QueryExtension
  {
    /// <summary>
    ///   Adds the <paramref name="query" /> into <paramref name="parentPatternTreeNode" />, if <paramref name="parentPatternTreeNode" /> already contains the matcher equal to
    ///   <paramref name="query" /> it does not add it but return the existent one. This method is used to add on a build plan or
    ///   make a several registrations regarding one Unit separately.
    /// </summary>
    /// <remarks>Call it first and then fill returned <see cref="IPatternTreeNode" /> with build actions or perform other needed actions.</remarks>
    [DebuggerStepThrough]
    public static T AddSubQuery<T>(this IPatternTreeNode parentPatternTreeNode, T query, bool addOrGet = true) where T : IPatternTreeNode
    {
      if(parentPatternTreeNode.Children.Contains(query))
        return addOrGet
                 ? (T) parentPatternTreeNode.Children.First(_ => _.Equals(query))
                 : throw new ArmatureException(string.Format("There is already matcher added '{0}'", query));

      parentPatternTreeNode.Children.Add(query);
      return query;
    }
  }
}
