using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

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
        [TestCase(null)]
        [TestCase(" ")]
        [TestCase("      ")]
        public void BeginScope_NameIsNullEmptyOrWhiteSpace_ThrowsException(string name)
        {
            Action act = () => TimeTrace.BeginScope(name);

            act.Should().Throw<ArgumentException>();
        }
    }
}
