const { defineConfig } = require('cypress');
const createBundler = require("@bahmutov/cypress-esbuild-preprocessor");
const preprocessor = require("@badeball/cypress-cucumber-preprocessor");
const createEsbuildPlugin = require("@badeball/cypress-cucumber-preprocessor/esbuild");
require("events").EventEmitter.defaultMaxListeners = 55;

async function setupNodeEvents(on, config) {
  const bundler = createBundler({
    plugins: [createEsbuildPlugin.createEsbuildPlugin(config)],
  });
  on("file:preprocessor", bundler);
  await preprocessor.addCucumberPreprocessorPlugin(on, config);
  require('cypress-mochawesome-reporter/plugin')(on);
  return config;
}

module.exports = defineConfig({
  reporter: 'cypress-mochawesome-reporter',
  reporterOptions: {
    charts: true,
    reportTitle: 'Test HTML Elements Report',
    reportPageTitle: 'Test HTML Elements Report',
    reportDir: 'cypress/reports',
    reportFilename: 'TestReport',
    embeddedScreenshots: true,
    inlineAssets: true,
    saveAllAttempts: false,
    code: false,
    showPassed: true,
    showFailed: true,
    showPending: false,
    showSkipped: false,
    timestamp: 'mmddyyyy_HHMMss'
  },
  e2e: {
    baseUrl: 'https://www.quackit.com/',
    specPattern: '**/*.feature',
    excludeSpecPattern: '*.js',
    supportFile: 'cypress/support/e2e.{js,jsx,ts,tsx}',
    watchForFileChanges: true,
    screenshotOnRunFailure: true,
    chromeWebSecurity: false,
    experimentalOriginDependencies: false,
    experimentalSourceRewriting: false,
    modifyObstructiveCode: true,
    videoUploadOnPasses: false,
    video: false,
    failOnStatusCode: false,
    screenshotsFolder: 'cypress/reports/screenshots',
    viewportWidth: 1200,
    viewportHeight: 768,
    runInMobileView: false,
    setupNodeEvents,    
    retries: {
        runMode: 0,
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
