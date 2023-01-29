export default {
    setMobileView() {
        const runInMobileView = Cypress.config("runInMobileView");
        cy.log(`runInMobileView: ${runInMobileView}`);
        if (runInMobileView === true) {
          Cypress.env('deviceName', "iphone");
          cy.viewport('iphone-x');
        }
        else {
          Cypress.env('deviceName', "desktop");
        }
      },
      
      getMobileView() {
        const runInMobileView = Cypress.config("runInMobileView");
        cy.log(`runInMobileView: ${runInMobileView}`);
        return cy.wrap(runInMobileView);
      },

      sync() {
        let stateCheck = setInterval(() => {
          if (document.readyState === "complete") {
            clearInterval(stateCheck);
            // document ready
          }
        }, 100);
      },

      acceptCookies() {
        cy.wait(2000);
        cy.get("body").then(($body) => {
          if ($body.text().includes("Manage cookies")) {
            cy.findAllByRole("button", { name: "Accept"})
              .filter(":visible")
              .first()
              .click();
          }
        });
        cy.wait(2000);
      }
}