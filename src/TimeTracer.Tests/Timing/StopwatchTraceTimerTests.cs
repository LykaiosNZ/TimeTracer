using FluentAssertions;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Threading;
using TimeTracer.Timing;

namespace TimeTracer.Tests.Timing
{
    [TestFixture(TestOf = typeof(StopwatchTraceTimer))]
    public class StopwatchTraceTimerTests
    {
        [Test]
        public void Elapsed_And_ElapsedNanoseconds_ShouldBeEquivalent()
        {
            var stopwatchNanosecondsPerTick = 1_000_000_000L / Stopwatch.Frequency;
            var timeSpanNanosecondsPerTick = 1_000_000_000L / TimeSpan.TicksPerSecond;
            var timer = new StopwatchTraceTimer();

            timer.Start();

            Thread.Sleep(5);

            timer.Stop();

            var timeSpanNanoseconds = timer.Elapsed.TotalMilliseconds * 1_000_000D;

            var delta = stopwatchNanosecondsPerTick == timeSpanNanosecondsPerTick
                ? 0
                : (ulong)(Math.Max(stopwatchNanosecondsPerTick / timeSpanNanosecondsPerTick, timeSpanNanosecondsPerTick / stopwatchNanosecondsPerTick));

            timer.ElapsedNanoseconds.Should().BeCloseTo((long)(timeSpanNanoseconds), delta);
        }

        [Test]
        public void IncrementsWhileRunning()
        {
            var sleepTimeMs = 5;

            var timer = new StopwatchTraceTimer();

            Thread.Sleep(5);

            var initialTimeNs = timer.ElapsedNanoseconds;

            timer.Start();

            Thread.Sleep(sleepTimeMs);

            timer.Stop();

            var afterFirstStop1 = timer.ElapsedNanoseconds;

            Thread.Sleep(sleepTimeMs);

            var afterFirstStop2 = timer.ElapsedNanoseconds;

            timer.Start();

            Thread.Sleep(sleepTimeMs);

            timer.Stop();

            var afterSecondStop = timer.ElapsedNanoseconds;

            initialTimeNs.Should().Be(0, "the timer should remain at 0 until started.");
            afterFirstStop1.Should().BeGreaterThan(initialTimeNs, "the timer should increment while running");
            afterFirstStop2.Should().Be(afterFirstStop1, "the timer should not increment while stopped");
            afterSecondStop.Should().BeGreaterThan(afterFirstStop2, "the timer should increment while running");
        }
    }
}