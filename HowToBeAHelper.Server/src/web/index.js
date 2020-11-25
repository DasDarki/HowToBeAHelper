const express = require('express');
const morgan = require('morgan');
const helmet = require('helmet');
const csp = require('express-csp');
const http = require('http');
const fs = require("fs");
const ATK = require('./../ATK.js');

const verifyTemplate = fs.readFileSync(__dirname + "/VerifySuccess.html").toString();
const forgotPasswordTemplate = fs.readFileSync(__dirname + "/ForgotPassword.html").toString();
const forgotPasswordAppliedTemplate = fs.readFileSync(__dirname + "/ForgotPasswordApplied.html").toString();

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
    app.get('/forgotpassword', (req, res) => {
        try {
            if (req.query.atk) {
                if (ATK.verify(req.query.atk)) {
                    res.status(200);
                    res.send(forgotPasswordTemplate.split("%atk%").join(req.query.atk));
                } else {
                    res.status(200);
                    res.send("Ungültige Anfrage");
                }
            } else {
                res.status(200);
                res.send("Ungültige Anfrage");
            }
        } catch (e) {
            console.error(e);
        }
    });
    app.get('/forgotpassword/save', (req, res) => {
        try {
            if (req.query.atk && req.query.pw) {
                if (ATK.verify(req.query.atk)) {
                    db.saveUserPassword(req.query.atk, req.query.pw, (username, success) => {
                        res.status(200);
                        if (username) {
                            res.send(forgotPasswordAppliedTemplate.split("%username%").join(username).split("%state%").join(success ? "erfolgreich" : "nicht"));
                        } else {
                            res.send("Interner Fehler");
                        }
                    });
                } else {
                    res.status(200);
                    res.send("Ungültige Anfrage");
                }
            } else {
                res.status(200);
                res.send("Ungültige Anfrage");
            }
        } catch (e) {
            console.error(e);
        }
    });

    const srv = http.createServer(app);
    srv.listen(8087);
    console.log(`Listening at http://localhost:8087`);
}