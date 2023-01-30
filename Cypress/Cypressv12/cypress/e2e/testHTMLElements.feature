Feature: Test HTML Elements
	
	An example feature to test HTML Elements using Cypress

@example
Scenario Outline: Test HTML Elements
	Given I navigate to Quakit HTML page
	Then I verify the HTML page has loaded
	When I click the HTML Tag '<htmlTag>'
    Then I verify the html element example page has loaded
    When I interact with the element using '<interaction>'
	Then I verify the interaction occurred '<interaction>'

Examples:
    | htmlTag           | interaction |
    | Basic link        | link        |
    | input type="text" | text        |
