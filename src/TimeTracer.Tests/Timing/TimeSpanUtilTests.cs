using FluentAssertions;
using NUnit.Framework;
using System;
using TimeTracer.Timing;

namespace TimeTracer.Tests.Timing
{
    [TestFixture(TestOf = typeof(TimeSpanUtil))]
    public class TimeSpanUtilTests
    {
        [Test]
        [TestCase(0)]
        [TestCase(long.MaxValue)]
        [TestCase(long.MinValue)]
        public void FromNanoseconds(long nanoseconds)
        {
            var milliseconds = nanoseconds / 1_000_000D;

            var expected = TimeSpan.FromMilliseconds(milliseconds);
            var timeSpan = TimeSpanUtil.FromNanoseconds(nanoseconds);

            timeSpan.Should().Be(expected);
        }
    }
}