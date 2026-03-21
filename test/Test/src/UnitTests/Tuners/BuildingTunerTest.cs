using System;
using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Sdk;
using BeatyBit.Armature.Sdk;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;

namespace Armature.Test.UnitTests;

public class BuildingTunerTest
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  private RootTuner _rootTuner;
  private IBuildStackPattern _treeRoot;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

  [SetUp]
  public void SetUp()
  {
    _treeRoot = new BuildStackPatternTree("test");
    _rootTuner = new RootTuner(_treeRoot);
  }

  #region Constructor Tests

  [Test]
  public void constructor_should_initialize_from_parent()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();

    // --act
    var target = new BuildingTuner<int>(_rootTuner, createNode, unitPattern);

    // --assert
    ((ITuner)target).Parent.Should().BeSameAs(_rootTuner);
    ((ITuner)target).TreeRoot.Should().BeSameAs(_treeRoot);
  }

  [Test]
  public void constructor_should_store_unitPattern()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();

    // --act
    var target = new BuildingTuner<int>(_rootTuner, createNode, unitPattern);

    // --assert
    var internalTuner = (IInternal<IUnitPattern>)target;
    internalTuner.Member1.Should().BeSameAs(unitPattern);
  }

  #endregion

  #region AmendWeight Tests

  [Test]
  public void amendWeight_as_IBuildingTuner_should_change_weight()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    IBuildingTuner<int> target = new BuildingTuner<int>(_rootTuner, createNode, unitPattern);

    // --act
    var result = target.AmendWeight(15);

    // --assert
    ((ITuner)result).Weight.Should().Be(15);
  }

  [Test]
  public void amendWeight_as_IBuildingTuner_should_return_same_instance()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    IBuildingTuner<int> target = new BuildingTuner<int>(_rootTuner, createNode, unitPattern);

    // --act
    var result = target.AmendWeight(15);

    // --assert
    result.Should().BeSameAs(target);
  }

  [Test]
  public void amendWeight_as_ISettingTuner_should_change_weight()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    ISettingTuner target = new BuildingTuner<int>(_rootTuner, createNode, unitPattern);

    // --act
    var result = target.AmendWeight(20);

    // --assert
    ((ITuner)result).Weight.Should().Be(20);
  }

  [Test]
  public void amendWeight_as_ICreationTuner_should_change_weight()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    ICreationTuner target = new BuildingTuner<int>(_rootTuner, createNode, unitPattern);

    // --act
    var result = target.AmendWeight(25);

    // --assert
    ((ITuner)result).Weight.Should().Be(25);
  }

  #endregion

  #region AsInstance Tests

  [Test]
  public void asInstance_should_call_buildStackPatternSubtree()
  {
    // --arrange
    var treeRoot = A.Fake<IBuildStackPattern>();
    var rootTuner = new RootTuner(treeRoot);
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var leafNode = A.Fake<IBuildStackPattern>();

    A.CallTo(() => createNode.Invoke(A<int>._)).Returns(leafNode);
    A.CallTo(() => treeRoot.GetOrAddNode(A<IBuildStackPattern>._)).Returns(leafNode);

    var target = new BuildingTuner<int>(rootTuner, createNode, unitPattern);
    var instance = 42;

    // --act
    target.AsInstance(instance);

    // --assert
    A.CallTo(() => leafNode.AddBuildAction(A<IBuildAction>._, BuildStage.Cache))
      .MustHaveHappenedOnceExactly();
  }

  #endregion

  #region As Tests

  [Test]
  public void as_with_type_should_return_ICreationTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var target = new BuildingTuner<IComparable>(_rootTuner, createNode, unitPattern);

    // --act
    var result = target.As(typeof(string));

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<ICreationTuner>();
  }

  [Test]
  public void as_generic_should_return_ICreationTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var target = new BuildingTuner<IComparable>(_rootTuner, createNode, unitPattern);

    // --act
    var result = target.As<string>();

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<ICreationTuner>();
  }

  [Test]
  public void as_should_throw_for_open_generic_type()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var target = new BuildingTuner<object>(_rootTuner, createNode, unitPattern);

    // --act
    var act = () => target.As(typeof(IComparable<>));

    // --assert
    act.Should().Throw<ArgumentException>()
       .WithMessage("*open generic*")
       .WithParameterName("type");
  }

  [Test]
  public void as_should_accept_tag()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var target = new BuildingTuner<IComparable>(_rootTuner, createNode, unitPattern);

    // --act
    var act = () => target.As(typeof(string), "myTag");

    // --assert
    act.Should().NotThrow();
  }

  [Test]
  public void as_should_add_build_action_to_subtree()
  {
    // --arrange
    var treeRoot = A.Fake<IBuildStackPattern>();
    var rootTuner = new RootTuner(treeRoot);
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var leafNode = A.Fake<IBuildStackPattern>();

    A.CallTo(() => createNode.Invoke(A<int>._)).Returns(leafNode);
    A.CallTo(() => treeRoot.GetOrAddNode(A<IBuildStackPattern>._)).Returns(leafNode);

    var target = new BuildingTuner<IComparable>(rootTuner, createNode, unitPattern);

    // --act
    target.As(typeof(string));

    // --assert
    A.CallTo(() => leafNode.AddBuildAction(A<IBuildAction>._, BuildStage.Create))
      .MustHaveHappenedOnceExactly();
  }

  #endregion

  #region AsIs Tests

  [Test]
  public void asIs_should_return_ISettingTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var target = new BuildingTuner<int>(_rootTuner, createNode, unitPattern);

    // --act
    var result = target.AsIs();

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<ISettingTuner>();
  }

  [Test]
  public void asIs_should_return_same_instance()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var target = new BuildingTuner<int>(_rootTuner, createNode, unitPattern);

    // --act
    var result = target.AsIs();

    // --assert
    result.Should().BeSameAs(target);
  }

  [Test]
  public void asIs_should_add_default_creation_build_action()
  {
    // --arrange
    var treeRoot = A.Fake<IBuildStackPattern>();
    var rootTuner = new RootTuner(treeRoot);
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var leafNode = A.Fake<IBuildStackPattern>();

    A.CallTo(() => createNode.Invoke(A<int>._)).Returns(leafNode);
    A.CallTo(() => treeRoot.GetOrAddNode(A<IBuildStackPattern>._)).Returns(leafNode);

    var target = new BuildingTuner<int>(rootTuner, createNode, unitPattern);

    // --act
    target.AsIs();

    // --assert
    A.CallTo(() => leafNode.AddBuildAction(Default.CreationBuildAction, BuildStage.Create))
      .MustHaveHappenedOnceExactly();
  }

  #endregion

  #region AsCreated Tests

  [Test]
  public void asCreated_with_type_should_return_ISettingTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var target = new BuildingTuner<IComparable>(_rootTuner, createNode, unitPattern);

    // --act
    var result = target.AsCreated(typeof(string));

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<ISettingTuner>();
  }

  [Test]
  public void asCreated_generic_should_return_ISettingTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var target = new BuildingTuner<IComparable>(_rootTuner, createNode, unitPattern);

    // --act
    var result = target.AsCreated<string>();

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<ISettingTuner>();
  }

  [Test]
  public void asCreated_should_accept_tag()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var target = new BuildingTuner<IComparable>(_rootTuner, createNode, unitPattern);

    // --act
    var act = () => target.AsCreated(typeof(string), "myTag");

    // --assert
    act.Should().NotThrow();
  }

  #endregion

  #region AsCreatedWith Tests

  [Test]
  public void asCreatedWith_no_args_should_return_ISettingTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var target = new BuildingTuner<int>(_rootTuner, createNode, unitPattern);

    // --act
    var result = target.AsCreatedWith(() => 42);

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<ISettingTuner>();
  }

  [Test]
  public void asCreatedWith_1_arg_should_return_ISettingTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var target = new BuildingTuner<string>(_rootTuner, createNode, unitPattern);

    // --act
    var result = target.AsCreatedWith<int>(x => x.ToString());

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<ISettingTuner>();
  }

  [Test]
  public void asCreatedWith_2_args_should_return_ISettingTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var target = new BuildingTuner<string>(_rootTuner, createNode, unitPattern);

    // --act
    var result = target.AsCreatedWith<int, int>((x, y) => $"{x},{y}");

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<ISettingTuner>();
  }

  [Test]
  public void asCreatedWith_3_args_should_return_ISettingTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var target = new BuildingTuner<string>(_rootTuner, createNode, unitPattern);

    // --act
    var result = target.AsCreatedWith<int, int, int>((x, y, z) => $"{x},{y},{z}");

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<ISettingTuner>();
  }

  [Test]
  public void asCreatedWith_4_args_should_return_ISettingTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var target = new BuildingTuner<string>(_rootTuner, createNode, unitPattern);

    // --act
    var result = target.AsCreatedWith<int, int, int, int>((a, b, c, d) => $"{a},{b},{c},{d}");

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<ISettingTuner>();
  }

  [Test]
  public void asCreatedWith_5_args_should_return_ISettingTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var target = new BuildingTuner<string>(_rootTuner, createNode, unitPattern);

    // --act
    var result = target.AsCreatedWith<int, int, int, int, int>((a, b, c, d, e) => $"{a},{b},{c},{d},{e}");

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<ISettingTuner>();
  }

  [Test]
  public void asCreatedWith_6_args_should_return_ISettingTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var target = new BuildingTuner<string>(_rootTuner, createNode, unitPattern);

    // --act
    var result = target.AsCreatedWith<int, int, int, int, int, int>((a, b, c, d, e, f) => $"{a},{b},{c},{d},{e},{f}");

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<ISettingTuner>();
  }

  [Test]
  public void asCreatedWith_7_args_should_return_ISettingTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var target = new BuildingTuner<string>(_rootTuner, createNode, unitPattern);

    // --act
    var result = target.AsCreatedWith<int, int, int, int, int, int, int>((a, b, c, d, e, f, g) => $"{a},{b},{c},{d},{e},{f},{g}");

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<ISettingTuner>();
  }

  [Test]
  public void asCreatedWith_buildSession_should_return_ISettingTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var target = new BuildingTuner<int>(_rootTuner, createNode, unitPattern);

    // --act
    var result = target.AsCreatedWith(_ => 42);

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<ISettingTuner>();
  }

  [Test]
  public void asCreatedWith_should_add_factory_build_action()
  {
    // --arrange
    var treeRoot = A.Fake<IBuildStackPattern>();
    var rootTuner = new RootTuner(treeRoot);
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var leafNode = A.Fake<IBuildStackPattern>();

    A.CallTo(() => createNode.Invoke(A<int>._)).Returns(leafNode);
    A.CallTo(() => treeRoot.GetOrAddNode(A<IBuildStackPattern>._)).Returns(leafNode);

    var target = new BuildingTuner<int>(rootTuner, createNode, unitPattern);

    // --act
    target.AsCreatedWith(() => 42);

    // --assert
    A.CallTo(() => leafNode.AddBuildAction(A<IBuildAction>.That.IsInstanceOf(typeof(CreateWithFactoryMethod<int>)), BuildStage.Create))
      .MustHaveHappenedOnceExactly();
  }

  #endregion

  #region CreatedByDefault Tests

  [Test]
  public void createdByDefault_should_return_ISettingTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var target = new BuildingTuner<string>(_rootTuner, createNode, unitPattern);

    // --act
    var result = ((ICreationTuner)target).CreatedByDefault();

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<ISettingTuner>();
  }

  [Test]
  public void createdByDefault_should_add_default_creation_build_action()
  {
    // --arrange
    var treeRoot = A.Fake<IBuildStackPattern>();
    var rootTuner = new RootTuner(treeRoot);
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var leafNode = A.Fake<IBuildStackPattern>();

    A.CallTo(() => createNode.Invoke(A<int>._)).Returns(leafNode);
    A.CallTo(() => treeRoot.GetOrAddNode(A<IBuildStackPattern>._)).Returns(leafNode);

    var target = new BuildingTuner<string>(rootTuner, createNode, unitPattern);

    // --act
    ((ICreationTuner)target).CreatedByDefault();

    // --assert
    A.CallTo(() => leafNode.AddBuildAction(Default.CreationBuildAction, BuildStage.Create))
      .MustHaveHappenedOnceExactly();
  }

  #endregion

  #region CreatedByReflection Tests

  [Test]
  public void createdByReflection_should_return_ISettingTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var target = new BuildingTuner<string>(_rootTuner, createNode, unitPattern);

    // --act
    var result = ((ICreationTuner)target).CreatedByReflection();

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<ISettingTuner>();
  }

  #endregion

  #region AsSingleton Tests

  [Test]
  public void asSingleton_should_return_IContextTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var target = new BuildingTuner<object>(_rootTuner, createNode, unitPattern);

    // --act
    var result = target.AsSingleton();

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<IContextTuner>();
  }

  [Test]
  public void asSingleton_should_return_same_instance()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var target = new BuildingTuner<object>(_rootTuner, createNode, unitPattern);

    // --act
    var result = target.AsSingleton();

    // --assert
    result.Should().BeSameAs(target);
  }

  [Test]
  public void asSingleton_should_add_singleton_build_action()
  {
    // --arrange
    var treeRoot = A.Fake<IBuildStackPattern>();
    var rootTuner = new RootTuner(treeRoot);
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var leafNode = A.Fake<IBuildStackPattern>();

    A.CallTo(() => createNode.Invoke(A<int>._)).Returns(leafNode);
    A.CallTo(() => treeRoot.GetOrAddNode(A<IBuildStackPattern>._)).Returns(leafNode);

    var target = new BuildingTuner<object>(rootTuner, createNode, unitPattern);

    // --act
    target.AsSingleton();

    // --assert
    A.CallTo(() => leafNode.AddBuildAction(A<IBuildAction>._, BuildStage.Cache))
      .MustHaveHappenedOnceExactly();
  }

  #endregion

  #region BuildingIt Tests

  [Test]
  public void buildingIt_should_return_ISubjectTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var target = new BuildingTuner<int>(_rootTuner, createNode, unitPattern);

    // --act
    var result = target.BuildingIt();

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<ISubjectTuner>();
  }

  [Test]
  public void buildingIt_should_return_new_instance()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var target = new BuildingTuner<int>(_rootTuner, createNode, unitPattern);

    // --act
    var result = target.BuildingIt();

    // --assert
    result.Should().NotBeSameAs(target);
  }

  #endregion

  #region Dependency Methods Tests

  [Test]
  public void using_should_return_ISettingTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    ISettingTuner target = new BuildingTuner<int>(_rootTuner, createNode, unitPattern);
    var sideTuner = A.Fake<ISideTuner>();

    // --act
    var result = target.Using(sideTuner);

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<ISettingTuner>();
  }

  [Test]
  public void usingArguments_should_return_ISettingTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    ISettingTuner target = new BuildingTuner<int>(_rootTuner, createNode, unitPattern);

    // --act
    var result = target.UsingArguments(1, 2, 3);

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<ISettingTuner>();
  }

  [Test]
  public void usingInjectionPoints_should_return_ISettingTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    ISettingTuner target = new BuildingTuner<int>(_rootTuner, createNode, unitPattern);
    var injectionPoint = A.Fake<IInjectionPointSideTuner>();

    // --act
    var result = target.UsingInjectionPoints(injectionPoint);

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<ISettingTuner>();
  }

  #endregion

  #region BuildStackPatternSubtree Tests

  [Test]
  public void buildStackPatternSubtree_should_return_leafNode()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var target = new BuildingTuner<int>(_rootTuner, createNode, unitPattern);

    // --act
    var internalTuner = (IInternal<IUnitPattern, IBuildStackPattern>)target;
    var result = internalTuner.Member2;

    // --assert
    result.Should().NotBeNull();
  }

  [Test]
  public void buildStackPatternSubtree_should_cache_leafNode()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var unitPattern = A.Fake<IUnitPattern>();
    var target = new BuildingTuner<int>(_rootTuner, createNode, unitPattern);

    // --act
    var internalTuner = (IInternal<IUnitPattern, IBuildStackPattern>)target;
    var result1 = internalTuner.Member2;
    var result2 = internalTuner.Member2;

    // --assert
    result1.Should().BeSameAs(result2);
  }

  #endregion
}
