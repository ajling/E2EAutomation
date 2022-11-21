using TechTalk.SpecFlow;
using Xunit;

namespace SeleniumNetCore.StepDefinitions
{
    [Binding]
    public sealed class CalculatorStepDefinitions
    {
        private ScenarioContext _scenarioContext;
        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        public CalculatorStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given("the first number is (.*)")]
        public void GivenTheFirstNumberIs(int number)
        {
            _scenarioContext.Set(number, "firstNumber");
        }

        [Given("the second number is (.*)")]
        public void GivenTheSecondNumberIs(int number)
        {
            _scenarioContext.Set(number, "secondNumber");
        }

        [When("the two numbers are added")]
        public void WhenTheTwoNumbersAreAdded()
        {
            var first = _scenarioContext.Get<int>("firstNumber");
            var second = _scenarioContext.Get<int>("secondNumber");
            var result = first + second;
            _scenarioContext.Set(result, "result");
        }

        [Then("the result should be (.*)")]
        public void ThenTheResultShouldBe(int result)
        {
            var actualResult = _scenarioContext.Get<int>("result");
            Assert.Equal(result, actualResult);
        }
    }
}