using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static ExpressionBuilder.DynamicQuerying;

namespace ExpressionBuilder.Tests
{
    public class UnitTest1
    {
        public List<TestObject> context { get; set; } = new List<TestObject>() {
            new TestObject{ Id=1,LastName="AAAA",Money=200,Name="BBBB"},
            new TestObject{ Id=2,LastName="AAAA",Money=300,Name="DDDD"},
            new TestObject{ Id=3,LastName="VVVV",Money=200,Name="DDDD"}
            };
        [Fact]
        public void ExpressionBuilderShouldReturnCollectionIfNoQueries()
        {
            var result= context.AsQueryable().RunQueries(new List<Query>());
            Assert.True(result.Count()==3);
        }

        [Fact]
        public void ExpressionGreaterThanQuery()
        {
            var queries = new List<Query>() { 
                new Query {Field=nameof(TestObject.Id),Value="1",Type=QueryType.GreaterThan}
            };
            var result = context.AsQueryable().RunQueries(queries);
            Assert.True(result.Count() == 2);
            Assert.True(!result.Any(x => x.Id <= 1));
        }
        [Fact]
        public void ExpressionGreaterThanOrEqualQuery()
        {
            var queries = new List<Query>() {
                new Query {Field=nameof(TestObject.Id),Value="3",Type=QueryType.GreaterThanOrEqual }
            };
            var result = context.AsQueryable().RunQueries(queries);
            Assert.True(result.Count() == 1);
            Assert.True(result.First().Id == 3);
        }
        [Fact]
        public void ExpressionLessThanOrEqualQuery()
        {
            var queries = new List<Query>() {
                new Query {Field=nameof(TestObject.Id),Value="1",Type=QueryType.LessThanOrEqual }
            };
            var result = context.AsQueryable().RunQueries(queries);
            Assert.True(result.Count() == 1);
            Assert.True(result.First().Id == 1);
        }
        [Fact]
        public void ExpressionLessThanQuery()
        {
            var queries = new List<Query>() {
                new Query {Field=nameof(TestObject.Id),Value="1",Type=QueryType.LessThan }
            };
            var result = context.AsQueryable().RunQueries(queries);
            Assert.True(result.Count() == 0);
        }

        [Fact]
        public void ExpressionEqualQuery()
        {
            var queries = new List<Query>() {
                new Query {Field=nameof(TestObject.Money),Value="200",Type=QueryType.Equal }
            };
            var result = context.AsQueryable().RunQueries(queries);
            Assert.True(result.Count() == 2);
        }
        [Fact]
        public void ExpressionNotEqualQuery()
        {
            var queries = new List<Query>() {
                new Query {Field=nameof(TestObject.Money),Value="200",Type=QueryType.NotEqual }
            };
            var result = context.AsQueryable().RunQueries(queries);
            Assert.True(result.Count() == 1);
        }
        [Fact]
        public void ExpressionContainsQuery()
        {
            var queries = new List<Query>() {
                new Query {Field=nameof(TestObject.LastName),Value="A",Type=QueryType.Contains }
            };
            var result = context.AsQueryable().RunQueries(queries);
            Assert.True(result.Count() == 2);
        }
        [Fact]
        public void ExpressionOrderBy()
        {
            var queries = new List<Query>()
            {
                new Query {Field=nameof(TestObject.Name),Value=null,Type=QueryType.OrderBy},
                new Query {Field=nameof(TestObject.Money),Value=null,Type=QueryType.OrderBy}
            };
            var result = context.AsQueryable().RunQueries(queries);
            Assert.True(result.Last().Money == 300);
        }
        [Fact]
        public void ExpressionOrderByDescending()
        {
            var queries = new List<Query>()
            {
                new Query {Field=nameof(TestObject.Name),Value=null,Type=QueryType.OrderByDescending},
                new Query {Field=nameof(TestObject.Money),Value=null,Type=QueryType.OrderByDescending}
            };
            var result = context.AsQueryable().RunQueries(queries);
            Assert.True(result.First().Money == 300);
        }
    }
}
