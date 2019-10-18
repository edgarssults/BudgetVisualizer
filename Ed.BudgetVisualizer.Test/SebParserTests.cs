using Ed.BudgetVisualizer.Logic.Parsers;
using Ed.BudgetVisualizer.Models;
using FluentAssertions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Ed.BudgetVisualizer.Test
{
    public class SebParserTests
    {
        private readonly SebParser _target;

        public SebParserTests()
        {
            _target = new SebParser();
        }
    }
}
