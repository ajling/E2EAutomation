const cucumber = require("cypress-cucumber-preprocessor").default;
const fs = require('fs')
const imageToBase64 = require('image-to-base64')
require("events").EventEmitter.defaultMaxListeners = 55;

module.exports = (on, config) => {
        on('file:preprocessor', cucumber())
        on('after:screenshot', (details) => {
                const newPath = details.path.replace("#", "");
                return new Promise((resolve, reject) => {
                        fs.rename(details.path, newPath, (err) => {
                                if (err) return reject(err)
                                resolve({ path: newPath })
                        })
                })
        })
        on('task', {
            convertImgToBase64(fileName) {
              if (fs.existsSync(fileName)) {
                      return imageToBase64(fileName);
              }
              return null;
            }
        })
};