require('dotenv').config();
const isDev = process.env.IS_DEV === "true";

const morgan = require('morgan');
const cors = require('cors');
const helmet = require('helmet');
const express = require('express');
const app = express();

app.use((req, res, next) => {
    let auth = req.get('Authorization');
    if (auth) {
        if (auth.startsWith('Bearer ')) {
            let token = auth.replace('Bearer ', '');
            //check if the token is correct
            req.token = token;
            return;
        }
    }
    const error = new Error(`Auth failed - Permission denied for ${req.originalUrl}`);
    res.status(401);
    next(error);
});

app.use('/api/v1', require('./routes/v1/index.js')());

app.use(morgan('common'));
app.use(cors());
app.use(helmet());
app.use(express.json());
app.use((req, res, next) => {
    const error = new Error(`No route found for ${req.originalUrl}`);
    res.status(404);
    next(error);
});
app.use((error, req, res, next) => {
    const statusCode = res.statusCode === 200 ? 500 : res.statusCode;
    res.status(statusCode);
    let result = {
        message: error.message
    };
    if (isDev) {
        result.stack = error.stack;
    }
    res.json(result);
});

const port = process.env.PORT || 1337;
app.listen(port, () => {
    console.log(`Listening at http://localhost:${port}`);
});