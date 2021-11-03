using System;
using System.Collections.Generic;

namespace Armature.Core;

public class AggregateArmatureException : AggregateException
{
  public AggregateArmatureException(IEnumerable<Exception> innerExceptions) : base(innerExceptions) { }
}