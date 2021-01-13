using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static DynamicQuerying.DynamicQuerying;

namespace DynamicQuerying.Tests
{
    public class ListOfQueriesTests
    {
        public List<TestObject> context { get; set; } = new List<TestObject>() {
            new TestObject{ Id=1,LastName="AAAA",Money=200,Name="BBBB"},
            new TestObject{ Id=2,LastName="AAAA",Money=300,Name="BBBB"},
            new TestObject{ Id=3,LastName="VVVV",Money=200,Name="DDDD"},
            new TestObject{ Id=4,LastName="VAVAVA",Money=100,Name="DBDBDB"},
            new TestObject{ Id=5,LastName="ZAVAZ",Money=200,Name="YYYZZZ"},
            new TestObject{ Id=6,LastName="VVVV",Money=500,Name="ZXCVB"},
            new TestObject{ Id=7,Money=200,LastName="",Name="XSDD"}
            };

        [Fact]
        public void ExpressionMultiQuery()
        {
            var queries = new List<Query>() {
                new Query {Field=nameof(TestObject.Id),Value="1",Type=QueryType.GreaterThan},
                new Query {Field=nameof(TestObject.Id),Value="7",Type=QueryType.LessThan},
                new Query {Field=nameof(TestObject.LastName),Value="VV",Type=QueryType.Contains},
                new Query {Field=nameof(TestObject.Money),Value="300",Type=QueryType.GreaterThanOrEqual}
            };
            var result = context.AsQueryable().RunQueries(queries);
            Assert.True(result.Count() == 1 && result.First().Id == 6);
        }

        [Fact]
        public void ExpressionMultiQueryWithOrdering()
        {
            var queries = new List<Query>() {
                new Query {Field=nameof(TestObject.Id),Value="1",Type=QueryType.GreaterThan},
                new Query {Field=nameof(TestObject.Id),Value="7",Type=QueryType.LessThan},
                new Query {Field=nameof(TestObject.LastName),Value="VV",Type=QueryType.Contains},
                new Query {Field=nameof(TestObject.Money),Type=QueryType.OrderByDescending}
            };
            var result = context.AsQueryable().RunQueries(queries);
            Assert.True(result.Count() == 2 && result.First().Id == 6);
        }
        [Fact]
        public void ExpressionMultiQueryOrdering()
        {
            var queries = new List<Query>() {
                new Query {Field=nameof(TestObject.Name),Type=QueryType.OrderByDescending},
                new Query {Field=nameof(TestObject.Money),Type=QueryType.OrderBy}
            };
            var result = context.AsQueryable().RunQueries(queries);
            Assert.True(result.Count() == 7 && result.Last().Id == 2);
        }

    }
}
