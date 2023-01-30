import genericHelpers from "../genericHelpers";

export default {
    verifyHtmlTagPageLoaded() {
        cy.get("[onclick=\"runPreview('preview');\"]").should("be.visible");
        cy.get("[onclick=\"runPreview('_blank');\"]").should("be.visible");
    },

    clickLink() {
        genericHelpers.getIFrameBody()
            .findAllByRole("link", { name: "Quackit homepage"})
            .scrollIntoView()
            .first()
            .should('not.be.disabled')
    },

    verifyLinkHref() {
        genericHelpers.getIFrameBody()
            .findAllByRole("link", { name: "Quackit homepage"})
            .scrollIntoView()
            .first()
            .then((el) => {
                let hrefValue = el.attr("href");
                expect(hrefValue).to.include("quackit.com");
            });
    },

    enterText(isInput = true) {
        if (isInput) {
            genericHelpers.getIFrameBody()
                .findByPlaceholderText("Enter your first name")
                .clear()
                .type("Example First Name");
        } else {
            genericHelpers.getIFrameBody()
                .findAllByRole("textarea")
                .first()
                .clear()
                .type("Example Text Area");
        }
    },

    verifyEnteredText(isInput = true) {
        if (isInput) {
            genericHelpers.getIFrameBody()
                .findByPlaceholderText("Enter your first name")
                .then((el) => {
                    let elValue = el.val();
                    expect(elValue).to.equal("Example First Name");
                });
        } else {
            genericHelpers.getIFrameBody()
                .findAllByRole("textarea")
                .first()
                .then((el) => {
                    let elValue = el.val();
                    expect(elValue).to.equal("Example Text Area");
                });
        }
    },
}