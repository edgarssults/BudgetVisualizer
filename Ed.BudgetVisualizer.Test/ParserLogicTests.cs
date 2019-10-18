using Ed.BudgetVisualizer.Logic;
using Ed.BudgetVisualizer.Models;
using FluentAssertions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Ed.BudgetVisualizer.Test
{
    public class ParserLogicTests
    {
        private readonly ParserLogic _target;

        public ParserLogicTests()
        {
            _target = new ParserLogic();
        }

        [Theory]
        [CsvFileData(@"Resources\TestSebData.csv")]
        [CsvFileData(@"Resources\TestSwedbankData.csv")]
        public async Task ParseFile_Works(Stream stream)
        {
            List<Transaction> results = await _target.ParseFile(stream);

            results.Should().NotBeNull();
            results.Count.Should().Be(5);
            results.Count(t => t.IsDebit).Should().Be(4);
            results.Count(t => t.IsCredit).Should().Be(1);
        }
    }
}
