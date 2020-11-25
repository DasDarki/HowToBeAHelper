function makeid(length) {
    let result = '';
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    const charactersLength = characters.length;
    for (let i = 0; i < length; i++) {
        result += characters.charAt(Math.floor(Math.random() * charactersLength));
    }
    return result;
}

const EXPLORER = makeid(10);
const SECRET = makeid(100);
const jwt = require('jsonwebtoken');

class ATK {

    static verify(atk) {
        try {
            let decoded = jwt.verify(atk, SECRET);
            return decoded != null;
        } catch (e) {
            return false;
        }
    }

    static generate() {
        return jwt.sign({exp: Math.floor(Date.now() / 1000) + (60 * 60), data: EXPLORER}, SECRET);
    }
}

module.exports = ATK;