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
        public void FromNanoseconds()
        {
            var timeSpan = TimeSpanUtil.FromNanoseconds(1_000_000);

            timeSpan.TotalMilliseconds.Should().Be(1);
        }

        [Test]
        public void FromNanoseconds_ValueIsZero()
        {
            var timeSpan = TimeSpanUtil.FromNanoseconds(0);

            timeSpan.Should().Be(TimeSpan.Zero);
        }
    }
}