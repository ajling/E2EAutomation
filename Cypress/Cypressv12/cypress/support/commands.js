import "@testing-library/cypress/add-commands";
const addContext = require("mochawesome/addContext");

Cypress.on("uncaught:exception", (err, runnable) => {
  // returning false here prevents Cypress from
  // failing the test
  return false;
});

Cypress.Commands.add("addContext", (context) => {
  cy.on("test:after:run", (attributes) => {
    addContext({ test: attributes }, context);
  });
});
