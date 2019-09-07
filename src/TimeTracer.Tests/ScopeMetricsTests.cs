using FluentAssertions;
using NUnit.Framework;
using System;

namespace TimeTracer.Tests
{
    [TestFixture(TestOf = typeof(ScopeMetrics))]
    public class ScopeMetricsTests
    {
        [Test]
        public void Add_AddsCorrectValues()
        {
            var metrics = new ScopeMetrics("Test");

            metrics.Add(500);
            metrics.Add(500);

            metrics.TotalNanoseconds.Should().Be(1000);
            metrics.Count.Should().Be(2);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_NameIsNullEmptyOrWhitespace_ThrowsException(string name)
        {
            Action act = () => new ScopeMetrics(name);

            var exception = act.Should().Throw<ArgumentException>();

            exception.Which.ParamName.Should().Be("name");
        }

        [Test]
        public void Constructor_NameIsSetCorrectly()
        {
            var expected = "Test";

            var metrics = new ScopeMetrics(expected);

            metrics.Name.Should().Be(expected);
        }
    }
}