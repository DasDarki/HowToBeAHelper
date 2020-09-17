require('dotenv').config();
const isDev = process.env.IS_DEV === "true";

const app = require('express');
const http = require('http').createServer(app);
const io = require('socket.io')(http);

const database = require('./database/index.js');

database.start(() => {
    function multicastLogin(exceptionId, username, callback) {
        for (let socketId in io.sockets.sockets) {
            let socket = io.sockets.sockets[socketId];
            if (exceptionId == socket.id) continue;
            if (socket.loginName == username) {
                callback(socket);
            }
        }
    }

    io.on('connection', (socket) => {
        socket.on("user:register", function (name, password) {
            database.createUser(name, password, status => {
                socket.emit("user:register:result", status);
            });
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
        socket.on("user:login", function (name, password, fn) {
            database.loginUser(name, password, data => {
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