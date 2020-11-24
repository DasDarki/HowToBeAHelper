const express = require('express');
const morgan = require('morgan');
const helmet = require('helmet');
const csp = require('express-csp');
const http = require('http');
const fs = require("fs");

const verifyTemplate = fs.readFileSync(__dirname + "/VerifySuccess.html").toString();

const app = express();
app.use(morgan('common'));
app.use(helmet());
app.use(express.json());
csp.extend(app, {
    policy: {
        directives: {
            'default-src': ['*'],
            'img-src': ['*', 'self', 'data:', 'https:'],
            'script-src': ['self', 'unsafe-inline', 'unsafe-eval', '*'],
            'style-src': ['self', 'unsafe-inline', '*']
        }
    },
    reportPolicy: {
        useScriptNonce: true,
        useStyleNonce: true,
        directives: {
            'default-src': ['*'],
            'img-src': ['*', 'self', 'data:', 'https:'],
            'script-src': ['self', 'unsafe-inline', 'unsafe-eval', '*'],
            'style-src': ['self', 'unsafe-inline', '*']
        }
    }
});

module.exports = (db) => {
    app.get('/verify', (req, res) => {
        try {
            if (req.query.val) {
                db.verifyUser(req.query.val, success => {
                    res.status(200);
                    res.send(verifyTemplate.split("%state%").join(success ? "war erfolgreich!" : "ist fehlgeschlagen!"));
                });
            } else {
                res.status(200);
                res.send("");
            }
        } catch (e) {
            console.error(e);
        }
    });

    const srv = http.createServer(app);
    srv.listen(8087);
    console.log(`Listening at http://localhost:8087`);
}