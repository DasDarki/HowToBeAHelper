require('dotenv').config();

const app = require('express');
const http = require('http').createServer(app);
const io = require('socket.io')(http);

const database = require('./database/index.js');

database.start(() => {
    function multicastLogin(exceptionId, username, callback) {
        for (let socketId in io.sockets.sockets) {
            let socket = io.sockets.sockets[socketId];
            if (exceptionId && exceptionId == socket.id) continue;
            if (socket.loginName == username) {
                callback(socket);
            }
        }
    }

    io.on('connection', (socket) => {
        socket.on("user:mute", function (name, flag) {
            multicastLogin(socket.id, name, other => {
                other.emit("user:mute", flag);
            });
        });
        socket.on("user:register", function (name, email, password) {
            database.createUser(name, email, password, status => {
                socket.emit("user:register:result", status);
            });
        });
        socket.on("user:set-email", function (name, email) {
            database.saveUserMail(name, email);
        });
        socket.on("session:request", function (sessionId, fn) {
            database.requestSessionData(sessionId, fn);
        });
        socket.on("session:close", function (sessionId, fn) {
            database.closeSession(sessionId, (success, userName) => {
                if (success) {
                    multicastLogin(socket.id, userName, other => {
                        other.emit("session:closed", sessionId);
                    });
                    fn(true);
                } else {
                    fn(false);
                }
            });
        });
        socket.on("session:kick-player", function (sessionId, playerId, fn) {
            database.kickPlayer(sessionId, playerId, (success, userName) => {
                if (success) {
                    multicastLogin(socket.id, userName, other => {
                        other.emit("session:kicked", sessionId);
                    });
                    fn(true);
                } else {
                    fn(false);
                }
            });
        });
        socket.on("session:sync-skills", function (sessionId, charId, json) {
            database.updateSessionSkills(sessionId, charId, json, (hostName, playerName) => {
                multicastLogin(socket.id, hostName, other => {
                    other.emit("session:sync-skills", sessionId, json, charId);
                });
                multicastLogin(socket.id, playerName, other => {
                    other.emit("session:sync-skills", sessionId, json, charId);
                });
            });
        });
        socket.on("session:sync-data", function (sessionId, username, charId, key, json) {
            database.updateSessionCharacter(sessionId, username, charId, key, JSON.parse(json), (hostName, playerName) => {
                multicastLogin(socket.id, hostName, other => {
                    other.emit("session:sync-data", sessionId, key, json, charId);
                });
                multicastLogin(socket.id, playerName, other => {
                    other.emit("session:sync-data", sessionId, key, json, charId);
                });
            });
        });
        socket.on("session:join", function (sessionId, sessionPassword, charId, fn) {
            database.joinSession(socket.loginName, sessionId, sessionPassword, charId, fn, (hostName, sessionUid, json) => {
                multicastLogin(null, hostName, other => {
                    other.emit("session:sync-join", sessionUid, json);
                });
            });
        });
        socket.on("session:create", function (name, password, fn) {
            database.createSession(socket.loginName, name, password, fn);
        });
        socket.on("character:skills", function (username, charId, json) {
            database.updateSkills(username, charId, json);
            multicastLogin(socket.id, username, other => {
                other.emit("character:sync-skills", charId, json);
            });
        });
        socket.on("character:update", function (username, charId, key, json) {
            database.updateCharacter(username, charId, key, JSON.parse(json));
            multicastLogin(socket.id, username, other => {
                other.emit("character:sync-data", charId, key, json);
            });
        });
        socket.on("user:char-refresh", function (username, fn) {
            database.refreshCharacters(username, fn);
        });
        socket.on("user:forgot-pw", function (user, fn) {
            database.forgotUserPw(user, fn);
        });
        socket.on("character:save", function (username, character, fn) {
            database.saveCharacter(username, character, fn);
            let json = JSON.stringify(character);
            multicastLogin(socket.id, username, other => {
                other.emit("character:sync-creation", json);
            });
        });
        socket.on("user:logout", function () {
            socket.loginName = null;
        });
        socket.on("user:login", async function (name, password, fn) {
            await database.loginUser(name, password, data => {
                if (data != "") {
                    socket.loginName = name;
                }
                fn(data);
            });
        });
        socket.on("character:delete", function (username, charId, fn) {
            database.deleteCharacter(username, charId, fn);
            multicastLogin(socket.id, username, other => {
                other.emit("character:sync-delete", charId);
            });
        });
        console.log("User Connected!");
    });

    const port = process.env.PORT || 1339;
    http.listen(port, function () {
        console.log("Master Server running on *:" + port);
    });
});

require("./web/index.js")(database);