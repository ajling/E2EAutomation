Feature: Verify Microsoft.com Menu

I navigate to Microsoft.com and verify the menu items

@Microsoft
Scenario Outline: Verify Microsoft.com Menu
	Given I navigate to Microsoft.com
	When I accept cookies
	Then I verify the menu item <menuItem>

Examples: 
| menuItem      |
| Microsoft 365 |
| Teams         |
| Windows       |
| Surface       |
