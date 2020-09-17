require('dotenv').config();
const isDev = process.env.IS_DEV === "true";

const app = require('express');
const http = require('http').createServer(app);
const io = require('socket.io')(http);

const database = require('./database/index.js');

database.start(() => {
    io.on('connection', (socket) => {
        socket.on("user:register", function (name, password) {
            database.createUser(name, password, status => {
                socket.emit("user:register:result", status);
            });
        });
        socket.on("character:skills", function (username, charId, json) {
            database.updateSkills(username, charId, json);
        });
        socket.on("character:update", function (username, charId, key, json) {
            database.updateCharacter(username, charId, key, JSON.parse(json));
        });
        socket.on("character:save", function (username, character, fn) {
            database.saveCharacter(username, character, fn);
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
        });
        console.log("User Connected!");
    });

    const port = process.env.PORT || 1339;
    http.listen(port, function () {
        console.log("Master Server running on *:" + port);
    });
});