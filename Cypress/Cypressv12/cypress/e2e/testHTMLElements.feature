Feature: Test HTML Elements
	
	An example feature to test HTML Elements using Cypress

@example
Scenario Outline: Test HTML Elements
	Given I navigate to JavaTPoint HTML page
	Then I verify the HTML page has loaded
	When I click the HTML Tag '<htmlTag>'
    Then I verify the '<htmlTitle>' page has loaded
	#And TODO
	#Then TODO
	#And TODO

Examples:
    | htmlTag          | htmlTitle      |
    | HTML <a> Tag     | HTML Anchor    |
    | HTML <input> Tag | HTML Input Tag |
