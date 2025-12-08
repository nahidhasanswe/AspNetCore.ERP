using Swashbuckle.AspNetCore.Filters;

namespace ERP.Api.Examples.Test;

public class TestModelExample : IExamplesProvider<TestModel>
{
    public TestModel GetExamples()
    {
        return new TestModel() { Name = "Test working fine", Mobile = "89673498673" };
    }
}