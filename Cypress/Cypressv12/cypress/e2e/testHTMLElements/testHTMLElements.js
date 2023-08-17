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
    cy.addContext("Navigated to Quakit HTML page");
});

Then(`I verify the HTML page has loaded`, () => {
    htmlExamplePage.verifyHtmlExamplePageLoaded();
    cy.addContext("Verified HTML page has loaded");
});

When("I click the HTML Tag {string}", (htmlTag) => {
    htmlExamplePage.clickHtmlTagLink(htmlTag);
    cy.addContext(`Clicked on the HTML Tag '${htmlTag}'`);
});

Then("I verify the html element example page has loaded", () => {
    htmlTagPage.verifyHtmlTagPageLoaded();
    cy.addContext("Verified HTML element page has loaded");
});

When("I interact with the element using {string}", (interaction) => {
    if (interaction == "link") {
        htmlTagPage.clickLink();
    } else if (interaction == "text") {
        htmlTagPage.enterText("Automated Testing Text");
    } else {
        // TODO
    }
    cy.addContext(`I begin interacting with a HTML element using '${interaction}'`);
});

Then("I verify the interaction occurred {string}", (interaction) => {
    if (interaction == "link") {
        htmlTagPage.verifyLinkHref();
    } else if (interaction == "text") {
        htmlTagPage.verifyEnteredText();
    } else {
        // TODO
    }
    cy.addContext(`Verified interaction with the HTML element using '${interaction}'`);
})