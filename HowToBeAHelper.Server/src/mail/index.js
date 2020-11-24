const nodemailer = require("nodemailer");
const fs = require("fs");

const verifyTemplate = fs.readFileSync(__dirname + "/VerifyMail.html").toString();
const forgotPasswordTemplate = fs.readFileSync(__dirname + "/ForgotPasswordMail.html").toString();

const senderEmail = '"How to be a Helper" <noreply@eternitylife.net>';
const transporter = nodemailer.createTransport({
    host: 'smtp.ionos.de',
    port: 465,
    secure: true,
    auth: {
        user: 'noreply@eternitylife.net',
        pass: '5jY2zvZy5WNITKL5fWoj.'
    }
});

class Mail {

    static async verify(email, username, verifyLink) {
        let info = await transporter.sendMail({
            from: senderEmail,
            to: email,
            subject: "Nur noch ein Schritt...",
            text: verifyLink,
            html: verifyTemplate.split("%username%").join(username).split("%verify_link%").join(verifyLink)
        });
        console.log("Message sent: %s", info.messageId);
    }

    static async forgotPassword(email, username, resetLink) {
        let info = await transporter.sendMail({
            from: senderEmail,
            to: email,
            subject: "Kann ja mal passieren...",
            text: resetLink,
            html: forgotPasswordTemplate.split("%username%").join(username).split("%forgot_link%").join(resetLink)
        });
        console.log("Message sent: %s", info.messageId);
    }

}

module.exports = Mail;