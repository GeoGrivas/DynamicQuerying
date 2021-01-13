# DynamicQuerying
DynamicQuerying is an extension function to Queryable that runs queries dynamically from string values.

With the magic of reflection and expression trees, you can run lambda expressions that are built dynamically on runtime!
DynamicQuerying is a take on that, in an attempt to run queries on collections dynamically.

Example:

```
            public List<TestObject> context { get; set; } = new List<TestObject>() {
              new TestObject{ Id=1,LastName="AAAA",Money=200,Name="BBBB"},
              new TestObject{ Id=2,LastName="AAAA",Money=300,Name="DDDD"},
              new TestObject{ Id=3,LastName="VVVV",Money=200,Name="DDDD"}
            };
            
            var queries = new List<Query>() {
                new Query {Field=nameof(TestObject.Id),Value="1",Type=QueryType.LessThanOrEqual }
            };
            var result = context.AsQueryable().RunQueries(queries);
```
This method was created originally for my [Blazor.DataGrid](https://github.com/GeoGrivas/Blazor.DataGrid) project, in order to have dynamic filtering and ordering.
