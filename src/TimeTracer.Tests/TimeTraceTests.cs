using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TimeTracer.Tests
{
    [TestFixture(TestOf = (typeof(TimeTrace)))]
    public class TimeTraceTests
    {
        [Test]
        public void Current_IsSetOnCreation_AndRemovedOnDispose()
        {
            using (var trace = new TimeTrace())
            {
                TimeTrace.Current.Should().BeSameAs(trace);
            }

            TimeTrace.Current.Should().BeNull();
        }

        [Test]
        public void Current_NestedTraces()
        {
            using (var outerTrace = new TimeTrace())
            {
                using (var innerTrace = new TimeTrace())
                {
                    TimeTrace.Current.Should().BeSameAs(innerTrace);
                }

                TimeTrace.Current.Should().BeSameAs(outerTrace);
            }
        }

        [Test]
        public void BeginScope_NoTrace_ScopeIsNull()
        {
            using (var scope = TimeTrace.BeginScope("TestScope"))
            {
                scope.Should().BeNull();
            }
        }

        [Test]
        public void BeginScope_TraceExists_ScopeIsCreatedWithCorrectName()
        {
            using (var trace = new TimeTrace())
            {
                using (var scope = TimeTrace.BeginScope("Foo"))
                {
                    scope.Should().NotBeNull();
                    scope.Name.Should().Be("Foo");
                }
            }
        }

        [Test]
        [TestCase(null)]
        [TestCase(" ")]
        [TestCase("      ")]
        public void BeginScope_NameIsNullEmptyOrWhiteSpace_ThrowsException(string name)
        {
            Action act = () => TimeTrace.BeginScope(name);

            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Scope_Disposed_MetricsAddedToTrace()
        {
            using (var trace = new TimeTrace())
            {
                using (var scope = TimeTrace.BeginScope("Foo"))
                {
                    Thread.Sleep(500); 
                }

                var metric = trace.Metrics.Single();

                metric.Count.Should().Be(1);
                metric.TotalDuration.Should().BeGreaterOrEqualTo(TimeSpan.FromMilliseconds(500));
            }
        }

        [Test]
        public void Scope_NestedScope_NamedCorrectly()
        {
            using (var trace = new TimeTrace())
            {
                using (var scope1 = TimeTrace.BeginScope("Foo"))
                {
                    scope1.Name.Should().Be("Foo");

                    using (var scope2 = TimeTrace.BeginScope("Bar"))
                    using (var scope3 = TimeTrace.BeginScope("Baz"))
                    {
                        scope2.Name.Should().Be("Foo/Bar");
                        scope3.Name.Should().Be("Foo/Bar/Baz");
                    }

                    using (var scope4 = TimeTrace.BeginScope("Ping"))
                    {
                        scope4.Name.Should().Be("Foo/Ping");
                    }
                }
            }
        }
    }
}
