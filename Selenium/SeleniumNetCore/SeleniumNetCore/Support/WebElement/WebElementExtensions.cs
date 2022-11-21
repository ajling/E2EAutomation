using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumNetCore.Support.Helpers;
using SeleniumNetCore.Support.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;

namespace SeleniumNetCore.Support.WebElement
{
    public static class WebElementExtensions
    {
        /// <summary>
        /// Function to click on buttons, checkboxes, options etc, but doesn't check if element is displayed
        /// </summary>
        /// <param name="element"></param>
        public static void ClickElement(this IWebElement element)
        {
            try
            {
                element.Click();
            }
            catch (Exception ex)
            {
                Reporter.WriteFailureEventLog(TestBase.GetDriver, $"Failed to click Element '{element}'. Exception: '{ex.GetType()}'. Message: '{ex.Message}'");
            }
        }

        /// <summary>
        /// Function to click on buttons, checkboxes, options etc, but checks if the element is displayed
        /// </summary>
        /// <param name="element"></param>
        public static void ClickDisplayedElement(this IWebElement element)
        {
            if (!element.IsDisplayed())
            {
                Reporter.WriteFailureEventLog(TestBase.GetDriver, $"Element you request to click is not displayed - {element}");
            }
            element.ClickElement();
        }

        // TODO:
        /// <summary>
        /// Will do a while loop for 120 seconds waiting for element to be displayed before performing any
        /// further action and if not found an error is thrown.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool IsDisplayed(this IWebElement element, int numberOfTimesToKeepChecking = 3)
        {
            var isElementDisplayed = element.Displayed;
            var i = 0;
            while (!isElementDisplayed)
            {
                i++;
                if (i >= numberOfTimesToKeepChecking)
                {
                    return false;
                }
                isElementDisplayed = element.Displayed;
            }
            return isElementDisplayed;
        }

        /// <summary>
        /// Function to assert if an element is displayed; if not true will fail.
        /// </summary>
        /// <param name="assertThisElementIsDisplayed"></param>
        public static void AssertElementIsDisplayed(this IWebElement assertThisElementIsDisplayed)
        {
            try
            {
                Assert.True(IsDisplayed(assertThisElementIsDisplayed), "Element is not displayed!");
            }
            catch (Exception)
            {
                Reporter.WriteFailureEventLog(TestBase.GetDriver, "Element is not displayed!");
            }
            Reporter.WriteSuccessfulEventLog("Element is displayed");
        }

        /// <summary>
        /// Function to enter text into input fields, but doesn't check if displayed.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        public static void EnterText(this IWebElement element, string value)
        {
            try
            {
                element.SendKeys(value);
            }
            catch (Exception ex)
            {
                Reporter.WriteFailureEventLog(TestBase.GetDriver, $"Failed to send keys value '{value}', to Element '{element}'. Exception: '{ex.GetType()}'. Message: '{ex.Message}'");
            }
            Reporter.WriteSuccessfulEventLog($"Successfully entered the text '{value}'");
        }

        /// <summary>
        /// Function to clear an input field before entering a new value into the field.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        public static void ClearAndEnterText(this IWebElement element, string value, int retryCount = 6)
        {
            try
            {
                int i = 0;
                element.Clear();
                while (!string.IsNullOrWhiteSpace(element.Text) && i < retryCount)
                {
                    Reporter.WriteTestInfo($"'{i} - {element.Text}'");
                    Reporter.WriteTestInfo($"Clearing '{element}'");
                    element.Clear();
                    i++;
                }
                element.SendKeys(value);
                Reporter.WriteTestInfo($"Cleared and entered '{value}' into field.");
            }
            catch (Exception ex)
            {
                Reporter.WriteFailureEventLog(TestBase.GetDriver, $"Failed to clear text from Element '{element}' and enter new text '{value}'. Exception: '{ex.Message}'");
            }
            Thread.Sleep(1000);
        }

        /// <summary>
        /// Function to check element is displayed and return text from said element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string GetText(this IWebElement element)
        {
            var textToReturn = "";
            var isElementDisplayed = element.IsDisplayed(10);
            if (isElementDisplayed)
            {
                textToReturn = element.Text;
            }
            else
            {
                Reporter.WriteFailureEventLog(TestBase.GetDriver, $"The element is not displayed, unable to retrieve text - {element}.");
            }
            Reporter.WriteTestInfo($"Text retrieved from element '{textToReturn}'");
            return textToReturn;
        }

        /// <summary>
        /// Function to select item from dropdown and return selected text, checks element is displayed.
        /// </summary>
        /// <param name="element"></param>
        public static string SelectFromDropDown(this IWebElement element, int numberOfTimesToKeepChecking = 3)
        {
            if (!element.IsDisplayed(numberOfTimesToKeepChecking))
            {
                Reporter.WriteFailureEventLog(TestBase.GetDriver, $"Dropdown is not displayed after checking {numberOfTimesToKeepChecking} times.");
            }
            
            SelectElement selector = new SelectElement(element);
            int indexValue = General.ReturnRandomIndex(selector.Options.Count);
            var selectedText = selector.Options[indexValue].Text;
            selector.SelectByIndex(indexValue);
            Reporter.WriteSuccessfulEventLog($"Dropdown is displayed, selected index '{indexValue}' which has value '{selectedText}'.");
            return selectedText;
        }

        /// <summary>
        /// Assert Texts between actual UI texts and supplied expected Texts.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="expectedText"></param>
        public static void AssertTexts(this IWebElement element, string expectedText)
        {
            var actualTexts = element.GetText();
            try
            {
                Assert.Equal(expectedText.Trim(), actualTexts.Trim().Replace("\\r\\n", ""));
                Reporter.WriteSuccessfulEventLog($" Text Matches : {actualTexts}");
            }
            catch (Exception)
            {
                Reporter.WriteFailureEventLog(TestBase.GetDriver, $"Text doesn't match: Expected:{expectedText} - Actual:{actualTexts}");
            }
        }

        /// <summary>
        /// Assert Text between actual web element attribute specified text content and supplied expected texts.
        /// i.e. attribute = value (when retrieving text from an input)
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attribute"></param>
        /// <param name="expectedText"></param>
        public static void AssertAttributeTexts(this IWebElement element, string attribute, string expectedText)
        {
            var actualAttributeTexts = element.GetAttribute(attribute);
            try
            {
                Assert.Equal(expectedText.Trim(), actualAttributeTexts.Trim().Replace("\\r\\n", ""));
                Reporter.WriteSuccessfulEventLog($"Attribute text matches: '{actualAttributeTexts}'");
            }
            catch (Exception)
            {
                Reporter.WriteFailureEventLog(TestBase.GetDriver, $"Attribute text doesn't match: Expected: '{expectedText}' - Actual: '{actualAttributeTexts}'");
            }
        }

        /// <summary>
        /// Assert Contains Text between actual UI texts and supplied expected Texts.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="expectedText"></param>
        public static void AssertContainsText(this IWebElement element, string expectedText)
        {
            string actualTexts = element.GetText();
            try
            {

                Assert.Contains(expectedText.Trim(), actualTexts.Trim().Replace("\\r\\n", ""));
                Reporter.WriteSuccessfulEventLog($"'{actualTexts}' Text Does Contain '{expectedText}'");
            }
            catch (Exception)
            {
                Reporter.WriteFailureEventLog(TestBase.GetDriver, $"Text doesn't contain the right values : Actual: '{actualTexts}' should contain the expected : '{expectedText}'");
            }
        }

        /// <summary>
        /// Assert Does Not Contain Text between actual UI texts and supplied expected Texts.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="textNotExpected"></param>
        public static void AssertDoesNotContainText(this IWebElement element, string textNotExpected)
        {
            string actualTexts = element.GetText();
            try
            {
                Assert.DoesNotContain(textNotExpected.Trim(), actualTexts.Trim().Replace("\\r\\n", ""));
                Reporter.WriteSuccessfulEventLog($"'{actualTexts}' Text Does Not Contain '{textNotExpected}'");
            }
            catch (Exception)
            {
                Reporter.WriteFailureEventLog(TestBase.GetDriver, $"Text does contain the incorrect values : Actual: '{actualTexts}' should not contain the expected : '{textNotExpected}'");
            }
        }

        /// <summary>
        /// Assert Contains Text between actual web element attribute specified text content and supplied expected texts.
        /// i.e. attribute = value (when retrieving text from an input)
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attribute"></param>
        /// <param name="expectedText"></param>
        public static void AssertAttributeContainsText(this IWebElement element, string attribute, string expectedText)
        {
            string attributeActualText = element.GetAttribute(attribute);
            try
            {
                Assert.Contains(expectedText.Trim(), attributeActualText.Trim().Replace("\\r\\n", ""));
                Reporter.WriteSuccessfulEventLog($"The attribute '{attribute}' text value is '{attributeActualText}'. Actual text does contain '{expectedText}'");
            }
            catch (Exception)
            {
                Reporter.WriteFailureEventLog(TestBase.GetDriver, $"Text doesn't contain the right values: Actual: '{attributeActualText}' should contain Expected: '{expectedText}'");
            }
        }

        /// <summary>
        /// Assert Class value between actual element class and supplied expected class.
        /// <param name="element"></param>
        /// <param name="expectedClass"></param>
        public static void AssertClassValue(this IWebElement element, string expectedClass)
        {
            try
            {
                string actualClass = element.GetAttribute("class");
                Assert.Equal(actualClass, expectedClass);
            }
            catch (Exception)
            {
                Reporter.WriteFailureEventLog(TestBase.GetDriver, $"Expected:{expectedClass} - Actual:{element.GetAttribute("class")}");
            }
        }

        /// <summary>
        /// This will select time from appointment time slot, this needs improvement just for demo purposes.
        /// </summary>
        /// <param name="timeTileContainer"></param>
        /// <param name="tagName"></param>
        public static void SelectTimeFromAvailableTimeSlot(this IList<IWebElement> timeTileContainer, IWebElement upArrowButton, IWebElement dwArrowButton, string timeTileAttributeToRetrieve = "class", string expectedAttributeValue = "instant")
        {
            var totalTimeTile = timeTileContainer.Count;
            Reporter.WriteSuccessfulEventLog($"I have a total of '{totalTimeTile}' time tiles.");
            var x = General.ReturnRandomIndex(totalTimeTile);
            for (int i = x; i < totalTimeTile; i++)
            {
                var returnTimeTileElementBasedOnIndex = timeTileContainer.ElementAtOrDefault<IWebElement>(i);
                string timeTileAttribute = returnTimeTileElementBasedOnIndex.GetAttribute(timeTileAttributeToRetrieve);
                if (returnTimeTileElementBasedOnIndex.Displayed && timeTileAttribute.Equals(expectedAttributeValue))
                {
                    Reporter.WriteSuccessfulEventLog($"Index of element to be selected from list of elements :'{i}'");
                    returnTimeTileElementBasedOnIndex.ClickElement();
                    break;
                }
                else if (!returnTimeTileElementBasedOnIndex.Displayed && timeTileAttribute.Equals(expectedAttributeValue))
                {
                    if (upArrowButton.Displayed)
                    {
                        upArrowButton.ClickElement();
                        if (returnTimeTileElementBasedOnIndex.Displayed)
                        {
                            Reporter.WriteSuccessfulEventLog($"Index of element to be selected from list of elements :'{i}'");
                            returnTimeTileElementBasedOnIndex.ClickElement();
                            break;
                        }
                        else
                        {
                            dwArrowButton.ClickElement();
                            if (returnTimeTileElementBasedOnIndex.Displayed)
                            {
                                Reporter.WriteSuccessfulEventLog($"Index of element to be selected from list of elements :'{i}'");
                                returnTimeTileElementBasedOnIndex.ClickElement();
                                break;
                            }
                        }
                    }
                }
                else if (i == totalTimeTile - 1)
                {
                    i = General.ReturnRandomIndex(totalTimeTile);
                    if (i == 9)
                    {
                        i -= 2;
                    }
                    Reporter.WriteSuccessfulEventLog($"I have regenerated the random index for the time tile to be selected as.' {i}'");
                }
            }
        }

        /// <summary>
        /// This selects available date from calendar for valuation appointment.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="tagname"></param>
        public static void SelectDateFromAvailableDates(this IList<IWebElement> daysInContainer, IWebElement frontArrowButton, IWebElement backArrowButton, string dayAttributeToRetrieve = "class", string expectedAttributeValue = "instant")
        {
            //Will implement later.
            var totalDayOptionInCalendar = daysInContainer.Count;
            Reporter.WriteSuccessfulEventLog($"I have a total of '{totalDayOptionInCalendar}' day's option in the calendar.");
            var x = General.ReturnRandomIndex(totalDayOptionInCalendar);
            for (int i = x; i < totalDayOptionInCalendar; i++)
            {
                var returnDayElementBasedOnIndex = daysInContainer.ElementAtOrDefault<IWebElement>(i);
                string returnedDayElementAttribute = returnDayElementBasedOnIndex.GetAttribute(dayAttributeToRetrieve);
                if (returnDayElementBasedOnIndex.Displayed && returnedDayElementAttribute.Equals(expectedAttributeValue))
                {
                    Reporter.WriteSuccessfulEventLog($"Index of day element to be selected from list of elements :'{i}'");
                    returnDayElementBasedOnIndex.Clicked();
                    break;
                }
                else if (!returnDayElementBasedOnIndex.Displayed && returnedDayElementAttribute.Equals(expectedAttributeValue))
                {
                    while (!returnDayElementBasedOnIndex.Displayed && returnedDayElementAttribute.Equals(expectedAttributeValue) && frontArrowButton.Displayed)
                    {
                        if (!returnDayElementBasedOnIndex.Displayed)
                        {
                            frontArrowButton.Clicked();
                            Thread.Sleep(1000);
                            Reporter.WriteSuccessfulEventLog("Clicking the front arrow in search of the selected day in the calendar.");
                        }
                        else if (returnDayElementBasedOnIndex.Displayed)
                        {
                            returnDayElementBasedOnIndex.Clicked();
                            Reporter.WriteSuccessfulEventLog("I have seen and clicked the randomly selected available day on the calendar, will break out of loop.");
                            break;
                        }
                        Reporter.WriteSuccessfulEventLog("After clicking the front arrow in search of the selected day in the calendar, the day is still not displayed - I will click the front arrow again.");
                    }
                    while (!returnDayElementBasedOnIndex.Displayed && returnedDayElementAttribute.Equals(expectedAttributeValue) && !frontArrowButton.Displayed)
                    {
                        if (!returnDayElementBasedOnIndex.Displayed)
                        {
                            backArrowButton.Clicked();
                            Thread.Sleep(1000);
                            Reporter.WriteSuccessfulEventLog("Clicking the back arrow in search of the selected day in the calendar.");
                        }
                        else if (returnDayElementBasedOnIndex.Displayed)
                        {
                            returnDayElementBasedOnIndex.Clicked();
                            Reporter.WriteSuccessfulEventLog("I have seen and clicked the randomly selected available day on the calendar, will break out of loop.");
                            break;
                        }
                        Reporter.WriteSuccessfulEventLog("After clicking the back arrow in search of the selected day in the calendar, the day is still not displayed - I will click the back arrow again.");
                    }
                }
                else if (i == totalDayOptionInCalendar - 1)
                {
                    i = General.ReturnRandomIndex(totalDayOptionInCalendar);
                    Reporter.WriteSuccessfulEventLog($"I have regenerated the random index for the time tile to be selected as.'{i}");
                }
            }
        }

        /// <summary>
        /// To identify the element name using its properties
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>Element details</returns>
        public static string GetReportName(this IWebElement element)
        {
            string[] elementId = { "id", "class", "name", "title", "value" };
            foreach (var findByAttribute in elementId)
            {
                var _element = element.GetAttribute(findByAttribute);
                if (!String.IsNullOrWhiteSpace(_element))
                    return _element;
            }
            return "untraceable element";
        }

        public static string GetValueOfSpecifiedAttributeForElement(this IWebElement elementToGetAttributeFrom, string attributeToGetTextFrom)
        {
            var attributeText = elementToGetAttributeFrom.GetAttribute(attributeToGetTextFrom);
            return attributeText;
        }

        /// <summary>
        /// Assert the element is not displayed.
        /// </summary>
        /// <param name="element"></param>
        public static void AssertNotDisplayed(this IWebElement element)
        {
            try
            {
                Assert.False(element.IsDisplayed());
                Reporter.WriteSuccessfulEventLog("Element is  not displayed");
            }
            catch (Exception)
            {
                Reporter.WriteFailureEventLog(TestBase.GetDriver, "Element is displayed");
            }
        }

        /// <summary>
        /// Assert the element is displayed.
        /// </summary>
        /// <param name="element"></param>
        public static void AssertDisplayed(this IWebElement element, string elementDescription = "")
        {
            try
            {
                Assert.True(element.IsDisplayed());
                var desc = string.IsNullOrEmpty(elementDescription) ? "Element is displayed" : $"{elementDescription} is displayed";
                Reporter.WriteSuccessfulEventLog(desc);
            }
            catch (Exception)
            {
                var desc = string.IsNullOrEmpty(elementDescription) ? "Element is not displayed" : $"{elementDescription} is not displayed";
                Reporter.WriteFailureEventLog(TestBase.GetDriver, desc);
            }
        }

        /// <summary>
        /// Assert the element does not exist.
        /// </summary>
        /// <param name="element"></param>
        public static void AssertNotExists(this IWebElement element)
        {
            try
            {
                var isElementPresent = element.Displayed;
                Assert.False(isElementPresent);
            }
            catch (Exception e)
            {
                Reporter.WriteFailureEventLog(TestBase.GetDriver, $"element is exists -{e.Message}");
            }
        }

        /// <summary>
        /// This will assert true if an element is selected or not - this is applicable to checkboxes and dropdown options.
        /// </summary>
        /// <param name="element"></param>
        public static void AssertTrueIfElementIsSelected(this IWebElement element)
        {
            try
            {
                Assert.True(element.Selected);
                Reporter.WriteSuccessfulEventLog($"Element '{element}' is selected as expected.");
            }
            catch (Exception)
            {
                Reporter.WriteFailureEventLog(TestBase.GetDriver, $"element ticked/selected - '{element}' to be '{element.Selected}'- Found :{element.Selected}");
            }
        }

        /// <summary>
        /// This will assert to check that an element is not selected - can be applicable to a checkbox/radio button and options in dropdown.
        /// </summary>
        /// <param name="element"></param>
        public static void AssertThatElementIsNotSelected(this IWebElement element)
        {
            try
            {
                Assert.False(element.Selected);
                Reporter.WriteSuccessfulEventLog($"As expected element - '{element}' is not selected.");
            }
            catch (Exception)
            {
                Reporter.WriteFailureEventLog(TestBase.GetDriver, $"element -'{element}' not to be selected/ticked by returning '{element.Selected}' - Found :{element.Selected}");
            }
        }

        public static void SelectElementByText(this IWebElement element, string text, bool partialMatch = false)
        {
            SelectElement select = new SelectElement(element);
            select.SelectByText(text, partialMatch);
            Reporter.WriteSuccessfulEventLog($"Select text is '{text}' and partial match is '{partialMatch}'");
        }

        public static void SelectElementByValue(this IWebElement element, string value)
        {
            SelectElement select = new SelectElement(element);
            select.SelectByValue(value);
            Reporter.WriteSuccessfulEventLog($"Select value is '{value}'");
        }

        public static void SelectElementByIndex(this IWebElement element, int index)
        {
            SelectElement select = new SelectElement(element);
            select.SelectByIndex(index);
            Reporter.WriteSuccessfulEventLog($"Selected element in index - {index} from dropdown with value - '{select.SelectedOption.Text}' ");
        }

        /// <summary>
        /// This retrieves property address
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string RetrievedPropertyAddressValuationIsBookedAgainst(this IWebElement element)
        {
            var elementText = element.Text;
            Reporter.WriteSuccessfulEventLog($"Text retrieved before splitting - {elementText}.");
            string[] splitText = elementText.Split(',');
            var splittedTextToReturn = splitText[0];
            Reporter.WriteSuccessfulEventLog($"Text returned after splitting retrieved text - {splittedTextToReturn}.");
            return splittedTextToReturn;
        }

        public static bool IsElementChecked(this IWebElement element)
        {
            var isSelected = element.Selected;
            return isSelected;
        }

        public static bool IsElementDisplayedAndChecked(this IWebElement element)
        {
            if (!element.IsDisplayed(5))
            {
                Reporter.WriteFailureEventLog(TestBase.GetDriver, "Element is not displayed, so can't check if its selected or not");
            }
            var isSelected = element.Selected;
            return isSelected;
        }

        public static void SelectLabelInGroup(this IWebElement containerElement, string itemValue, string otherFieldXpathLocator = "")
        {
            string labelText = "";
            IReadOnlyCollection<IWebElement> labelElements = containerElement.FindElements(By.TagName("label"));
            Reporter.WriteSuccessfulEventLog($"Found the 'label' elements in the group container provided. There were {labelElements.Count} elements found.");

            foreach (var label in labelElements)
            {
                labelText = label.Text;
                if (labelText.ToUpper().Equals(itemValue.ToUpper()))
                {
                    label.ClickElement();
                    Reporter.WriteSuccessfulEventLog($"Selected the label with the text '{labelText}' located in the specified group container.");
                    break;
                }
            }

            if ("OTHER".Equals(itemValue.ToUpper()))
            {
                IWebElement otherField = containerElement.FindElement(By.XPath(otherFieldXpathLocator));
                otherField.ClickElement();
                Reporter.WriteSuccessfulEventLog($"Clicked on the other field input.");
                otherField.EnterText("9");
                Reporter.WriteSuccessfulEventLog("Set the value of the 'other' fields input to '9'.");
            }
        }

        public static void EnterDateOfBirth(this IWebElement dateOfBirthInput, string requiredDateOfBirth)
        {
            dateOfBirthInput.EnterText(requiredDateOfBirth);
            dateOfBirthInput.EnterText(Keys.Return);
            Thread.Sleep(1000);
        }

        public static void SelectFirstMatchingItemFromTable(this IWebElement customerSearchResultTable, string requiredItem)
        {
            var rowElements = customerSearchResultTable.FindElements(By.TagName("tr"));
            Reporter.WriteSuccessfulEventLog($"Found the 'tr' elements in the group container provided. There were {rowElements.Count} elements found.");

            bool itemMatched = false;
            if (rowElements.Count == 1)
            {
                IWebElement cellToClick = rowElements[0].FindElement(By.XPath(".//td[2]/a"));
                itemMatched = true;
                cellToClick.ClickElement();
                Reporter.WriteSuccessfulEventLog($"Only one row found so I have clicked on it.");
            }
            else
            {
                foreach (IWebElement rowElement in rowElements)
                {
                    var columnElements = rowElement.FindElements(By.TagName("td"));
                    Reporter.WriteSuccessfulEventLog($"Found the 'td' elements in the row container provided. There were {columnElements.Count} elements found.");

                    foreach (IWebElement columnElement in columnElements)
                    {
                        var cellElements = columnElement.FindElements(By.TagName("a"));
                        if (cellElements.Count > 0)
                        {
                            IWebElement cellElement = cellElements[0];
                            if (cellElement.Text.ToLower().Equals(requiredItem.ToLower()))
                            {
                                itemMatched = true;
                                cellElement.ClickElement();
                                Reporter.WriteSuccessfulEventLog($"Found the cell element matching text '{requiredItem}' and clicked on it.");
                                break;
                            }
                        }

                    }

                    if (itemMatched)
                    {
                        break;
                    }
                }
            }

            if (!itemMatched)
            {
                Reporter.WriteFailureEventLog(TestBase.GetDriver, $"I was unable to find the cell element matching text '{requiredItem}'.");
            }
        }

        public static void SelectGivenTextFromList(this IWebElement givenElement, string text)
        {
            var elementList = givenElement.FindElements(By.TagName("li"));
            foreach (IWebElement element in elementList)
            {
                if (element.GetText().Equals(text))
                {
                    element.Clicked();
                    break;
                }
            }

        }

        public static Boolean IsElementEnabledOrDisabled(this IWebElement elementToCheckIfEnabledOrNot)
        {
            var isElementEnabled = elementToCheckIfEnabledOrNot.Enabled;
            if (isElementEnabled == true)
            {
                Reporter.WriteTestInfo("Element is enabled.");
                return isElementEnabled;
            }
            Reporter.WriteTestInfo("Element is disabled.");
            return isElementEnabled;
        }

        public static void AssertItemCount(this IList<IWebElement> totalItems, int itemCountExpected)
        {
            try
            {
                Assert.Equal(totalItems.Count, itemCountExpected);
                Reporter.WriteSuccessfulEventLog($"AssertItemCount - Ilist item count matches : {itemCountExpected}");
            }
            catch (Exception)
            {
                Reporter.WriteFailureEventLog(TestBase.GetDriver, $"AssertItemCount - Ilist item count doesn't match. Expected: {itemCountExpected} - Actual: {totalItems.Count}");
            }
        }

        public static string GetSelectedDropdownOptionText(this IWebElement element)
        {
            var selectedOption = new SelectElement(element).SelectedOption.Text;
            Reporter.WriteSuccessfulEventLog($"select option is {selectedOption}");
            return selectedOption;
        }

        public static void AssertThatValuesArePresent<T>(this List<T> list, T text)
        {
            try
            {
                Assert.Contains(text, list);
                Reporter.WriteSuccessfulEventLog($"As expected all the  values  are present.");
            }
            catch (Exception)
            {
                Reporter.WriteFailureEventLog(TestBase.GetDriver, $"All the values are not present.");
            }
        }

        public static void AssertCompressedTexts(this IWebElement element, string expectedText)
        {
            string actualTexts = element.GetText();
            try
            {
                Assert.Equal(expectedText.Trim().Replace(" ", "").Replace(Environment.NewLine, ""), actualTexts.Trim().Replace(" ", "").Replace(Environment.NewLine, ""));
                Reporter.WriteSuccessfulEventLog($" Text Matches : {actualTexts}");
            }
            catch (Exception)
            {
                Reporter.WriteFailureEventLog(TestBase.GetDriver, $"Text doesn't match: Expected:{expectedText} - Actual:{actualTexts}");
            }
        }
    }
}

