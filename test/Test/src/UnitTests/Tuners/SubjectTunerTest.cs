using System;
using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;

namespace Armature.Test.UnitTests;

public class SubjectTunerTest
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
  public void constructor_should_throw_when_createNode_is_null()
  {
    // --act
    var act = () => new SubjectTuner(_rootTuner, null!);

    // --assert
    act.Should().Throw<ArgumentNullException>().WithParameterName("createNode");
  }

  [Test]
  public void constructor_should_initialize_weight_from_parent()
  {
    // --arrange
    var parentTuner = SubjectTuner.Treat<int>(_rootTuner, null).AmendWeight(42);
    var createNode = A.Fake<CreateNode>();

    // --act
    var target = new SubjectTuner((ITuner)parentTuner, createNode);

    // --assert
    ((ITuner)target).Weight.Should().Be(42);
  }

  [Test]
  public void constructor_should_initialize_treeRoot_from_parent()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();

    // --act
    var target = new SubjectTuner(_rootTuner, createNode);

    // --assert
    ((ITuner)target).TreeRoot.Should().BeSameAs(_treeRoot);
  }

  [Test]
  public void constructor_should_store_parent_reference()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();

    // --act
    var target = new SubjectTuner(_rootTuner, createNode);

    // --assert
    ((ITuner)target).Parent.Should().BeSameAs(_rootTuner);
  }

  #endregion

  #region AmendWeight Tests

  [Test]
  public void amendWeight_as_ISubjectTuner_should_change_weight()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    ISubjectTuner target = new SubjectTuner(_rootTuner, createNode);

    // --act
    var result = target.AmendWeight(10);

    // --assert
    ((ITuner)result).Weight.Should().Be(10);
  }

  [Test]
  public void amendWeight_as_ISubjectTuner_should_return_same_instance()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    ISubjectTuner target = new SubjectTuner(_rootTuner, createNode);

    // --act
    var result = target.AmendWeight(10);

    // --assert
    result.Should().BeSameAs(target);
  }

  [Test]
  public void amendWeight_as_IAllTuner_should_change_weight()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    IAllTuner target = new SubjectTuner(_rootTuner, createNode);

    // --act
    var result = target.AmendWeight(15);

    // --assert
    ((ITuner)result).Weight.Should().Be(15);
  }

  [Test]
  public void amendWeight_should_accumulate_multiple_calls()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    ISubjectTuner target = new SubjectTuner(_rootTuner, createNode);

    // --act
    target.AmendWeight(10).AmendWeight(5).AmendWeight(-3);

    // --assert
    ((ITuner)target).Weight.Should().Be(12);
  }

  [Test]
  public void amendWeight_should_support_negative_delta()
  {
    // --arrange
    var parentTuner = SubjectTuner.Treat<int>(_rootTuner, null).AmendWeight(100);
    var createNode = A.Fake<CreateNode>();
    ISubjectTuner target = new SubjectTuner((ITuner)parentTuner, createNode);

    // --act
    target.AmendWeight(-30);

    // --assert
    ((ITuner)target).Weight.Should().Be(70);
  }

  #endregion

  #region ITuner Interface Tests

  [Test]
  public void iTuner_parent_should_return_parent()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var target = new SubjectTuner(_rootTuner, createNode);

    // --act
    var result = ((ITuner)target).Parent;

    // --assert
    result.Should().BeSameAs(_rootTuner);
  }

  [Test]
  public void iTuner_treeRoot_should_return_treeRoot()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var target = new SubjectTuner(_rootTuner, createNode);

    // --act
    var result = ((ITuner)target).TreeRoot;

    // --assert
    result.Should().BeSameAs(_treeRoot);
  }

  [Test]
  public void iTuner_weight_should_return_current_weight()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    ISubjectTuner target = new SubjectTuner(_rootTuner, createNode);
    target.AmendWeight(25);

    // --act
    var result = ((ITuner)target).Weight;

    // --assert
    result.Should().Be(25);
  }

  [Test]
  public void iTuner_getOrAddNodeTo_should_call_createNode_with_current_weight()
  {
    // --arrange
    var expectedNode = A.Fake<IBuildStackPattern>();
    var createNode = A.Fake<CreateNode>();
    A.CallTo(() => createNode.Invoke(A<int>._)).Returns(expectedNode);

    ISubjectTuner target = new SubjectTuner(_rootTuner, createNode);
    target.AmendWeight(33);

    var parentNode = A.Fake<IBuildStackPattern>();

    // --act
    ((ITuner)target).GetOrAddNodeTo(parentNode);

    // --assert
    A.CallTo(() => createNode.Invoke(33)).MustHaveHappenedOnceExactly();
  }

  [Test]
  public void iTuner_getOrAddNodeTo_should_call_node_getOrAddNode_with_created_node()
  {
    // --arrange
    var expectedNode = A.Fake<IBuildStackPattern>();
    var createNode = A.Fake<CreateNode>();
    A.CallTo(() => createNode.Invoke(A<int>._)).Returns(expectedNode);

    var target = new SubjectTuner(_rootTuner, createNode);
    var parentNode = A.Fake<IBuildStackPattern>();

    // --act
    ((ITuner)target).GetOrAddNodeTo(parentNode);

    // --assert
    A.CallTo(() => parentNode.GetOrAddNode(expectedNode)).MustHaveHappenedOnceExactly();
  }

  [Test]
  public void iTuner_getOrAddNodeTo_should_return_result_from_getOrAddNode()
  {
    // --arrange
    var createdNode = A.Fake<IBuildStackPattern>();
    var returnedNode = A.Fake<IBuildStackPattern>();

    var createNode = A.Fake<CreateNode>();
    A.CallTo(() => createNode.Invoke(A<int>._)).Returns(createdNode);

    var parentNode = A.Fake<IBuildStackPattern>();
    A.CallTo(() => parentNode.GetOrAddNode(createdNode)).Returns(returnedNode);

    var target = new SubjectTuner(_rootTuner, createNode);

    // --act
    var result = ((ITuner)target).GetOrAddNodeTo(parentNode);

    // --assert
    result.Should().BeSameAs(returnedNode);
  }

  #endregion

  #region Building Method Tests

  [Test]
  public void building_with_type_should_return_ISubjectTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var target = new SubjectTuner(_rootTuner, createNode);

    // --act
    var result = target.Building(typeof(string));

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<ISubjectTuner>();
  }

  [Test]
  public void building_with_type_should_accept_tag()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var target = new SubjectTuner(_rootTuner, createNode);

    // --act
    var act = () => target.Building(typeof(string), "myTag");

    // --assert
    act.Should().NotThrow();
  }

  [Test]
  public void building_generic_should_return_ISubjectTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var target = new SubjectTuner(_rootTuner, createNode);

    // --act
    var result = target.Building<int>();

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<ISubjectTuner>();
  }

  #endregion

  #region Treat Method Tests

  [Test]
  public void treat_with_type_should_return_IBuildingTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var target = new SubjectTuner(_rootTuner, createNode);

    // --act
    var result = target.Treat(typeof(string));

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<IBuildingTuner<object?>>();
  }

  [Test]
  public void treat_generic_should_return_typed_IBuildingTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var target = new SubjectTuner(_rootTuner, createNode);

    // --act
    var result = target.Treat<int>();

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<IBuildingTuner<int>>();
  }

  [Test]
  public void treat_with_tag_should_accept_tag()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var target = new SubjectTuner(_rootTuner, createNode);

    // --act
    var act = () => target.Treat<string>("myTag");

    // --assert
    act.Should().NotThrow();
  }

  #endregion

  #region TreatOpenGeneric Tests

  [Test]
  public void treatOpenGeneric_should_return_IBuildingTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var target = new SubjectTuner(_rootTuner, createNode);

    // --act
    var result = target.TreatOpenGeneric(typeof(IComparable<>));

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<IBuildingTuner<object?>>();
  }

  #endregion

  #region TreatInheritorsOf Tests

  [Test]
  public void treatInheritorsOf_with_type_should_return_IBuildingTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var target = new SubjectTuner(_rootTuner, createNode);

    // --act
    var result = target.TreatInheritorsOf(typeof(IComparable));

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<IBuildingTuner<object?>>();
  }

  [Test]
  public void treatInheritorsOf_generic_should_return_typed_IBuildingTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var target = new SubjectTuner(_rootTuner, createNode);

    // --act
    var result = target.TreatInheritorsOf<IComparable>();

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<IBuildingTuner<IComparable>>();
  }

  #endregion

  #region TreatAll Tests

  [Test]
  public void treatAll_should_return_IAllTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var target = new SubjectTuner(_rootTuner, createNode);

    // --act
    var result = target.TreatAll();

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<IAllTuner>();
  }

  [Test]
  public void treatAll_should_return_same_instance()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var target = new SubjectTuner(_rootTuner, createNode);

    // --act
    var result = target.TreatAll();

    // --assert
    result.Should().BeSameAs(target);
  }

  #endregion

  #region Dependency Methods Tests

  [Test]
  public void usingArguments_should_return_IAllTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var target = new SubjectTuner(_rootTuner, createNode);

    // --act
    var result = target.UsingArguments("arg1", 42);

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<IAllTuner>();
  }

  [Test]
  public void usingInjectionPoints_should_return_IAllTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var target = new SubjectTuner(_rootTuner, createNode);
    var injectionPoint = A.Fake<IInjectionPointSideTuner>();

    // --act
    var result = target.UsingInjectionPoints(injectionPoint);

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<IAllTuner>();
  }

  [Test]
  public void using_should_return_IAllTuner()
  {
    // --arrange
    var createNode = A.Fake<CreateNode>();
    var target = new SubjectTuner(_rootTuner, createNode);
    var sideTuner = A.Fake<ISideTuner>();

    // --act
    var result = target.Using(sideTuner);

    // --assert
    result.Should().NotBeNull();
    result.Should().BeAssignableTo<IAllTuner>();
  }
  #endregion
}
