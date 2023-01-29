export default {
    verifyHtmlTagPageLoaded(htmlTitle) {
        cy.findAllByRole("link", { name: "next →"})
            .filter(":visible")
            .first()
            .should("be.visible");
        cy.findAllByRole("link", { name: "← prev"})
            .filter(":visible")
            .first()
            .should("be.visible");
        cy.findAllByRole("heading", { name: htmlTitle })
            .first()
            .should("be.visible");
    },
}