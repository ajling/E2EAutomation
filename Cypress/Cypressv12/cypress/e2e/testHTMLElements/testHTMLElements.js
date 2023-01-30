/// <reference types="Cypress" />
const { Given, Then, When } = require("@badeball/cypress-cucumber-preprocessor");

import htmlTagPage from "../../framework/pages/htmlTagPage";
import htmlExamplePage from "../../framework/pages/htmlExamplePage";
import genericHelpers from "./../../framework/genericHelpers";

beforeEach(() => {
    // run these tests in mobile view depending on setting
    genericHelpers.setMobileView();
});

Given(`I navigate to Quakit HTML page`, () => {
    cy.visit("html/examples/");
});

Then(`I verify the HTML page has loaded`, () => {
    htmlExamplePage.verifyHtmlExamplePageLoaded();
});

When("I click the HTML Tag {string}", (htmlTag) => {
    htmlExamplePage.clickHtmlTagLink(htmlTag);
});

Then("I verify the html element example page has loaded", () => {
    htmlTagPage.verifyHtmlTagPageLoaded();
});

When("I interact with the element using {string}", (interaction) => {
    if (interaction == "link") {
        htmlTagPage.clickLink();
    } else if (interaction == "text") {
        htmlTagPage.enterText("Automated Testing Text");
    } else {
        // TODO
    }
});

Then("I verify the interaction occurred {string}", (interaction) => {
    if (interaction == "link") {
        htmlTagPage.verifyLinkHref();
    } else if (interaction == "text") {
        htmlTagPage.verifyEnteredText();
    } else {
        // TODO
    }
})