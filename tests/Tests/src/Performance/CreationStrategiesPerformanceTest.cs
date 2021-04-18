using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Armature.Core;
using NUnit.Framework;

namespace Tests.Performance
{
  [Ignore("Run manually only")]
  public class CreationStrategiesPerformanceTest
  {
    private const    int             Count                 = 100000;
    private readonly ConstructorInfo _constructorInfo      = typeof(Subject).GetConstructors().Single(_ => _.GetParameters().Length > 0);
    private readonly ConstructorInfo _emptyConstructorInfo = typeof(Subject).GetConstructors().Single(_ => _.GetParameters().Length == 0);
    private readonly object[]        _values               = {"1", "2", "3", "4", "5", "6", "7"};

    [Test]
    public void CreateByEmptyConstructorInfo()
    {
      var sw = new Stopwatch();
      sw.Start();

      for(var i = 0; i < Count; i++)
      {
        var value = _emptyConstructorInfo.Invoke(Empty<object>.Array);
        GC.KeepAlive(value);
      }

      sw.Stop();
      Console.WriteLine(sw.Elapsed);
    }

    [Test]
    public void CreateByEmtpyActivator()
    {
      var sw = new Stopwatch();
      sw.Start();

      for(var i = 0; i < Count; i++)
      {
        var value = Activator.CreateInstance(typeof(Subject), Empty<object>.Array);
        GC.KeepAlive(value);
      }

      sw.Stop();
      Console.WriteLine(sw.Elapsed);
    }

    [Test]
    public void CreateByConstructorInfo()
    {
      var sw = new Stopwatch();
      sw.Start();

      for(var i = 0; i < Count; i++)
      {
        var value = _constructorInfo.Invoke(_values);
        GC.KeepAlive(value);
      }

      sw.Stop();
      Console.WriteLine(sw.Elapsed);
    }

    [Test]
    public void CreateByActivator()
    {
      var sw = new Stopwatch();
      sw.Start();

      for(var i = 0; i < Count; i++)
      {
        var value = Activator.CreateInstance(typeof(Subject), _values);
        GC.KeepAlive(value);
      }

      sw.Stop();
      Console.WriteLine(sw.Elapsed);
    }

    [Test]
    public void CreateByCompiledExpression()
    {
      var sw = new Stopwatch();
      sw.Start();
      var factory = BuildFactoryExpression(_constructorInfo);

      for(var i = 0; i < Count; i++)
      {
        var value = factory(_values);
        GC.KeepAlive(value);
      }

      sw.Stop();
      Console.WriteLine(sw.Elapsed);
    }

    [Test]
    public void CreateByEmptyCompiledExpression()
    {
      var sw = new Stopwatch();
      sw.Start();
      var factory = BuildFactoryExpression(_emptyConstructorInfo);

      for(var i = 0; i < Count; i++)
      {
        var value = factory(Empty<object>.Array);
        GC.KeepAlive(value);
      }

      sw.Stop();
      Console.WriteLine(sw.Elapsed);
    }

    private static Func<object[], object> BuildFactoryExpression(ConstructorInfo ctor)
    {
      var parameters = ctor.GetParameters(); // Get the parameters of the constructor
      var args       = new Expression[parameters.Length];
      var param      = Expression.Parameter(typeof(object[])); // The object[] parameter to the Func

      // get the item from the array in the parameter and cast it to the correct type for the constructor
      for(var i = 0; i != parameters.Length; ++i)
        args[i] = Expression.Convert(Expression.ArrayIndex(param, Expression.Constant(i)), parameters[i].ParameterType);

      return Expression
            .Lambda<Func<object[], object>>(Expression.New(ctor, args), param)
            .Compile();
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    private class Subject
    {
      public Subject() { }

      public Subject(string s1, string s2, string s3, string s4, string s5, string s6, string s7) { }
    }
  }
}
