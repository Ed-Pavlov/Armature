using System.Collections.Generic;

namespace BeatyBit.Armature.Core;

/// <summary>
/// Collection of build actions grouped by a build stage.
/// </summary>
public class BuildActionBag : Dictionary<object, List<IBuildAction>> { }
