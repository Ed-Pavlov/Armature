using System.Collections.Generic;
using Armature;
using Armature.Core;
using Armature.Core.BuildActions;
using Armature.Core.BuildActions.Constructor;
using Armature.Core.BuildActions.Parameter;
using Armature.Core.UnitMatchers;
using Armature.Core.UnitMatchers.Parameters;
using Armature.Core.UnitSequenceMatcher;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Functional
{
	public class InstantiatingOrderTest
	{
		[Test]
		public void should_instantiate_as_bfs()
		{
			var expected = new List<string> {nameof(C1), nameof(Singleton), nameof(C2), nameof(C3), nameof(C4), nameof(B1), nameof(B2), nameof(A)};
			
			var target = CreateTarget();

			target.Treat<IA>().AsCreated<A>();
			
			target.Treat<IB1>().AsCreated<B1>();
			target.Treat<IB2>().AsCreated<B2>();

			target.Treat<IC1>().AsCreated<C1>();
			target.Treat<IC2>().AsCreated<C2>();
			target.Treat<IC3>().AsCreated<C3>();
			target.Treat<IC4>().AsCreated<C4>();
			
			target.Treat<ISingleton>().AsCreated<Singleton>().AsSingleton();

			var actual = new List<string>();
			target.Build<IA>(actual);

			actual.Should().ContainInOrder(expected);
		}
		
		private static Builder CreateTarget() =>
			new(BuildStage.Cache, BuildStage.Create)
			{
				new AnyUnitSequenceMatcher
				{
					// inject into constructor
					new LastUnitSequenceMatcher(ConstructorMatcher.Instance)
						.AddBuildAction(
							BuildStage.Create,
							new OrderedBuildActionContainer
							{
								new GetInjectPointConstructorBuildAction(), // constructor marked with [Inject] attribute has more priority
								GetLongestConstructorBuildAction.Instance // constructor with largest number of parameters has less priority
							}),


					new LastUnitSequenceMatcher(ParameterValueMatcher.Instance)
						.AddBuildAction(
							BuildStage.Create,
							new OrderedBuildActionContainer
							{
								CreateParameterValueForInjectPointBuildAction.Instance,
								CreateParameterValueBuildAction.Instance
							})
				}
			};

		private class Base
		{
			protected Base(ICollection<string> collection) => collection.Add(GetType().Name);
		}
		
		private interface IA{ }
		private interface IB1{ }
		private interface IB2{ }
		private interface IC1{ }
		private interface IC2{ }
		private interface IC3{ }
		private interface IC4{ }
		private interface ISingleton{ }
		
		private class A : Base, IA { public A(IB1 b1, IB2 b2, ICollection<string> collection) : base(collection) { } }
		private class B1 : Base, IB1 { public B1(IC1 c1, ISingleton s, IC2 c2, ICollection<string> collection) : base(collection) { } }
		private class B2 : Base, IB2 { public B2(ISingleton s, IC3 c3, IC4 c4, ICollection<string> collection) : base(collection) { } }
		
		private class C1 : Base, IC1 { public C1(ICollection<string> collection) : base(collection) { } }
		private class C2 : Base, IC2 { public C2(ICollection<string> collection) : base(collection) { } }
		private class C3 : Base, IC3 { public C3(ICollection<string> collection) : base(collection) { } }
		private class C4 : Base, IC4 { public C4(ICollection<string> collection) : base(collection) { } }
		
		private class Singleton : Base, ISingleton { public Singleton(ICollection<string> collection) : base(collection) { } }
		
	}
}