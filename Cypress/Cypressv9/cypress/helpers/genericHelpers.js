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

      getIframeDocument() {
        return cy.get("#preview").its('0.contentDocument').should('exist');
      },

      getIFrameBody() {
        return this.getIframeDocument().its('body').should('not.be.undefined').then(cy.wrap);
      },
}