using Armature.Core;
using FakeItEasy;
using FakeItEasy.Creation;

namespace Tests.Common
{
	public class BuildSessionFakeOptionsBuilder : FakeOptionsBuilder<IBuildSession>
	{
		protected override void BuildOptions(IFakeOptions<IBuildSession> options) =>
			options.ConfigureFake(fake => fake.BuildResult = null);
	}
}