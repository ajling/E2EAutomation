const { defineConfig } = require('cypress');
const createBundler = require("@bahmutov/cypress-esbuild-preprocessor");
const preprocessor = require("@badeball/cypress-cucumber-preprocessor");
const createEsbuildPlugin = require("@badeball/cypress-cucumber-preprocessor/esbuild");
require("events").EventEmitter.defaultMaxListeners = 55;

async function setupNodeEvents(on, config) {
  
  await preprocessor.addCucumberPreprocessorPlugin(on, config);
  on(
    "file:preprocessor",
    createBundler({
      plugins: [createEsbuildPlugin.default(config)],
    })
  )

  require('cypress-mochawesome-reporter/plugin')(on);

  // Make sure to return the config object as it might have been modified by the plugin.
  return config;
}

module.exports = defineConfig({
  reporter: 'cypress-multi-reporters',
  reporterOptions: {
    reporterEnabled: "cypress-mochawesome-reporter, spec, mocha-junit-reporter",
    cypressMochawesomeReporterReporterOptions: {
      reportDir: "cypress/reports/html",
      charts: false,
      reportPageTitle: "Cypress-E2E-Tests",
      embeddedScreenshots: true,
      inlineAssets: true
    },
    mochaJunitReporterReporterOptions: {
      mochaFile: "test-report-xml/results-[hash].xml"
    }
  },
  screenshotOnRunFailure: true,
  e2e: {
    baseUrl: 'https://www.quackit.com/',
    specPattern: '**/*.feature',
    excludeSpecPattern: '*.js',
    supportFile: 'cypress/support/e2e.{js,jsx,ts,tsx}',
    watchForFileChanges: true,
    setupNodeEvents,
    runInMobileView: false,
    videoUploadOnPasses: false,
    video: false,
    experimentalSourceRewriting: false,
    chromeWebSecurity: false,
    modifyObstructiveCode: true,
    pageLoadTimeout: 120000,
    defaultCommandTimeout: 30000,
    viewportWidth: 1200,
    viewportHeight: 768,
    responseTimeout: 30000,
    waitAfterEachCommand: 500,
    screenshotsFolder: 'cypress/reports/html/screenshots',
    retries: {
        runMode: 1,
        openMode: 0,
    },
    blockHosts: [
      "www.googletagmanager.com",
      "*doubleclick.net",
      "*youtube.com",
      "*ph-126.net",
    ],
  },
  
})
