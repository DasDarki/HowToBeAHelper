const mongoose = require('mongoose');
const schemas = require('./schemas.js');
const {v4: uuidv4} = require('uuid');

function Database() {

    this.start = function (callback) {
        mongoose.connect(`mongodb://${process.env.MONGO_HOST}:27017/` + process.env.MONGO_DABA, {
            useNewUrlParser: true, useUnifiedTopology: true,
            user: process.env.MONGO_USER,
            pass: process.env.MONGO_PASS
        }).then(() => {
            console.log("Connection to Mongo established!");

            callback();
        }).catch(err => {
            console.error(err);
        });
    };

    this.saveCharacter = function (name, character, callback) {
        schemas.User.find({
            name: {$regex: new RegExp(name, "i")}
        }, null, {limit: 1}, function (err, result) {
            if (err) {
                console.error(err);
                callback(false);
                return;
            }
            if (result && result.length >= 1) {
                let user = result[0];
                user.characters.push(new schemas.Character(character));
                user.save(function (err, _) {
                    if (err) {
                        console.error(err);
                        callback(false);
                    } else {
                        callback(true);
                    }
                });
            } else {
                callback(false);
            }
        });
    };

    this.loginUser = function (name, password, callback) {
        schemas.User.find({
            name: {$regex: new RegExp(name, "i")},
            password: password
        }, null, {limit: 1}, function (err, result) {
            if (err) {
                console.error(err);
                callback("");
                return;
            }
            if (result && result.length >= 1) {
                callback(JSON.stringify(result[0].characters).split('"').join('\\"'));
            } else {
                callback("");
            }
        });
    };

    this.createUser = function (name, password, callback) {
        schemas.User.find({name: {$regex: new RegExp(name, "i")}}, function (err, result) {
            if (err) {
                console.error(err);
                callback(2); //internal err
                return;
            }
            if (result && result.length > 0) {
                callback(1); //username existing
                return;
            }
            let user = new schemas.User({name: name, password: password, characters: [], uuid: uuidv4()});
            user.save(function (err, _) {
                let status = 0; //success
                if (err) {
                    status = 2; //internal err
                    console.error(err);
                }
                callback(status);
            });
        });

    };
}

module.exports = new Database();