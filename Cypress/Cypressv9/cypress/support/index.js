// Import commands.js using ES2015 syntax:
import "./commands";
import addContext from 'mochawesome/addContext';
require("cypress-xpath");

Cypress.Server.defaults({
  delay: 1000
});

Cypress.on('test:after:run', (test, runnable) => {
  if (test.state === 'failed') {
      if (Cypress.env('imageBase64')) {
          addContext({ test }, Cypress.env('imageBase64'));
      }
  }
});