using System.Collections.Generic;

namespace Armature.Core.Sdk;

/// <summary>
/// Collection of build actions grouped by a build stage.
/// </summary>
public class BuildActionBag : Dictionary<object, List<IBuildAction>> { }
