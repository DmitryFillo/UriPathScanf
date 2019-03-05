using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;
using UriPathScanf.Utils;

namespace UriPathScanf.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal class QueryStringParserTest
    {
        [Test]
        [TestCaseSource(typeof(QueryStringParserTestData), nameof(QueryStringParserTestData.TestCases))]
        public IDictionary<string, IEnumerable<string>> ParseTest(string qs)
        {
            return QueryStringParser.Parse(qs);
        }

    }

    [ExcludeFromCodeCoverage]
    internal class QueryStringParserTestData
    {
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData("?a=3").Returns(new Dictionary<string, string[]>
                {
                    {"a", new[] {"3"}},
                }).SetName("One parameter with leading ?");

                yield return new TestCaseData("a=3").Returns(new Dictionary<string, string[]>
                {
                    {"a", new[] {"3"}},
                }).SetName("One parameter without leading ?");

                yield return new TestCaseData("a=3&a=4").Returns(new Dictionary<string, string[]>
                {
                    {"a", new[] {"3","4"}},
                }).SetName("One parameter (dup values) without leading ?");

                yield return new TestCaseData("?a=3&b=4&x=5").Returns(new Dictionary<string, string[]>
                {
                    {"a", new[] {"3"}},
                    {"b", new[] {"4"}},
                    {"x", new[] {"5"}},
                }).SetName("Multiple parameter with leading ?");

                yield return new TestCaseData("a=3&b=4&x=5").Returns(new Dictionary<string, string[]>
                {
                    {"a", new[] {"3"}},
                    {"b", new[] {"4"}},
                    {"x", new[] {"5"}},
                }).SetName("Multiple parameter without leading ?");

                yield return new TestCaseData("?a=3&b=4&x=5&a=43d&x=x").Returns(new Dictionary<string, string[]>
                {
                    {"a", new[] {"3","43d"}},
                    {"b", new[] {"4"}},
                    {"x", new[] {"5","x"}},
                }).SetName("Multiple parameters (dup values) with leading ?");

                yield return new TestCaseData("???a=3&&&&b=4&x=5&a=43d&&x=x").Returns(new Dictionary<string, string[]>
                {
                    {"a", new[] {"3","43d"}},
                    {"b", new[] {"4"}},
                    {"x", new[] {"5","x"}},
                }).SetName("Multiple parameters (dup values) with duplicated both ? and &");

                yield return new TestCaseData("???a=3;;b=4;x=5&a=43d;;;;;;;x=xxx").Returns(new Dictionary<string, string[]>
                {
                    {"a", new[] {"3","43d"}},
                    {"b", new[] {"4"}},
                    {"x", new[] {"5","xxx"}},
                }).SetName("Multiple parameters (dup values) with duplicated both ? and &/;");

                yield return new TestCaseData("a=3;;b=4;x=5&a=43d;;;;;;;x=xxx").Returns(new Dictionary<string, string[]>
                {
                    {"a", new[] {"3","43d"}},
                    {"b", new[] {"4"}},
                    {"x", new[] {"5","xxx"}},
                }).SetName("Multiple parameters (dup values) without leading ?, but duplicated &/;");
            }
        }
    }
}
