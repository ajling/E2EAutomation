export default {
    verifyHtmlTutorialPageLoaded() {
        cy.findAllByRole("heading", { name: "HTML Tutorial"})
            .filter(":visible")
            .first()
            .should("be.visible");
        cy.findAllByRole("link", { name: "next →"})
            .filter(":visible")
            .first()
            .should("be.visible");
    },

    clickHtmlTagLink(htmlTag) {
        cy.findAllByRole("link", { name: htmlTag})
            .scrollIntoView()
            .first()
            .click();
    },
}