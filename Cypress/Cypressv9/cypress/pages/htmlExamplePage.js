export default {
    verifyHtmlExamplePageLoaded() {
        cy.findAllByRole("heading", { name: "HTML Examples"})
            .filter(":visible")
            .first()
            .should("be.visible");
        cy.findAllByRole("link", { name: "HTML Text Examples"})
            .filter(":visible")
            .first()
            .should("be.visible");
    },

    clickHtmlTagLink(htmlTag) {
        cy.findAllByRole("link", { name: htmlTag})
            .scrollIntoView()
            .first()
            .invoke('removeAttr', 'target')
            .click();
    },
}