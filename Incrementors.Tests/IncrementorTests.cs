using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Incrementors.Tests
{
    public class IncrementorTests
    {
        private static readonly Random _random = new Random();

        [Fact]
        public void TestIncrementBy1()
        {
            var inc = new Incrementor(0, 10);
            inc.Value.Should().Be(0);
            for (int i=1; i<=10; i++)
            {
                inc = inc.IncrementValue();
                inc.Value.Should().Be(i);
            }
        }

        [Fact]
        public void TestIncrementBy2()
        {
            var inc = new Incrementor(10, 20, 2);
            inc.Value.Should().Be(10);
            for (int i = 1; i <= 5; i++)
            {
                inc = inc.IncrementValue();
                inc.Value.Should().Be(10 + 2*i);
            }
        }

        [Fact]
        public void TestToString()
        {
            var inc = new Incrementor(0, 10);
            inc.ToString().Should().Be("0");
            for (int i = 1; i <= 10; i++)
            {
                inc = inc.IncrementValue();
                inc.ToString().Should().Be(i.ToString());
            }
        }

        [Fact]
        public void TestCombined()
        {
            var inc1 = new Incrementor(0, 3);
            var inc2 = new Incrementor(5, 7);
            var combined = Incrementor.CreateCombined(new Incrementor[] { inc1, inc2 });
            string[] expected = { "05", "06", "07", "15", "16", "17", "25", "26", "27", "35", "36", "37" };
            for (int i=0; i<expected.Length; i++)
            {
                combined.ToString().Should().Be(expected[i]);
                if (combined.CanIncrement)
                    combined = combined.IncrementValue();
            }
        }

        [Fact]
        public void TestLongCombined()
        {
            int incrementorCount = _random.Next(3, 10);
            Incrementor[] incrementors = Enumerable.Range(1, incrementorCount).Select(i => CreateRandomIncrementor()).ToArray();
            Incrementor combined = Incrementor.CreateCombined(incrementors);
            int count = 0;
            while (combined.CanIncrement)
            {
                count++;
                combined = combined.IncrementValue();
            }

            int expectedPossibilities = (incrementors.Select(i => i.Maximum).Aggregate(1, (a, i) => a * (i+1)) - 1);
            count.Should().Be(expectedPossibilities);

            string expectedFinalString = string.Join("", incrementors.Select(i => i.Maximum.ToString()));
            combined.ToString().Should().Be(expectedFinalString);
        }

        private Incrementor CreateRandomIncrementor()
        {
            return new Incrementor(0, _random.Next(1, 10));
        }

        [Fact]
        public void TestNoParentIncrementor()
        {
            var inc1 = new Incrementor(0, 3);
            inc1.CanIncrement.Should().BeTrue();
            var maxInc = inc1.IncrementValue().IncrementValue().IncrementValue();
            maxInc.CanIncrement.Should().BeFalse();
            Action act = () => maxInc.IncrementValue();
            act.ShouldThrow<InvalidOperationException>();
        }
    }
}
