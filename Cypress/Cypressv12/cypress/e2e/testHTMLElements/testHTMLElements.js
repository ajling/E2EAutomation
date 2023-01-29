/// <reference types="Cypress" />
const { Given, Then, When } = require("@badeball/cypress-cucumber-preprocessor");

import htmlTagPage from "../../framework/pages/htmlTagPage";
import htmlTutorialPage from "../../framework/pages/htmlTutorialPage";
import genericHelpers from "./../../framework/genericHelpers";

beforeEach(() => {
    // run these tests in mobile view depending on setting
    genericHelpers.setMobileView();
});

Given(`I navigate to JavaTPoint HTML page`, () => {
    cy.visit("/html-tutorial");
});

Then(`I verify the HTML page has loaded`, () => {
    htmlTutorialPage.verifyHtmlTutorialPageLoaded();
});

When("I click the HTML Tag {string}", (htmlTag) => {
    htmlTutorialPage.clickHtmlTagLink(htmlTag);
});

Then("I verify the {string} page has loaded", (htmlTitle) => {
    htmlTagPage.verifyHtmlTagPageLoaded(htmlTitle);
});

When("I select a date time", () => {
    
});