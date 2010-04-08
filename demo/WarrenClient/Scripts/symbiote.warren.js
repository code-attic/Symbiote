var warren = function () {

    var sock = null; // web socket
    var connected_callback = null;
    var disconnected_callback = null;
    var error_callback = null;
    var on_message = null;
    var username = '';
    var info = null;

    var log = function (msg) {
        if (info) info(msg);
    }

    var connect = function (url, user) {
        if (checkBrowser()) {
            try {
                username = user;
                log('Attempting connection...');
                sock = new WebSocket(url);
                sock.onopen = setup;
            } catch (Error) {
                if (error_callback) error_callback('Error occurred during connection attempt: ' + Error);
            }
        }
        else {
            if (error_callback) error_callback('oh shit');
        }
    }

    var checkBrowser = function () {
        if ("WebSocket" in window) {
            return true;
        }
        return false;
    }

    var send = function (json) {
        try {
            log('translate: ' + json);
            var message = $.toJSON(json);
            log('sending :' + message);
            sock.send(message);
        } catch (Error) {
            if (error_callback) error_callback(Error);
        }
    }

    var sendMessage = function (to, jsonMessage, key) {
        try {
            var json = {
                "To": to,
                "Body": jsonMessage,
                "From": username,
                "RoutingKey": ""
            };
            send(json);
        } catch (Error) {
            if (error_callback) error_callback(Error);
        }
    }

    var messageReceived = function (msg) {
        try {
            log('message recieved');
            var json = $.evalJSON(msg.data);
            log('processed message to: ' + json);
            if (on_message) on_message(json);
        } catch (Error) {
            if (error_callback) error_callback(Error);
        }
    }

    var setup = function () {
        try {
            setTimeout(send(userJson()), 200);
            setTimeout(send(all_clients()), 200);
            setTimeout(send(clients()), 200);
            setTimeout(send(node()), 200);
            if (connected_callback) connected_callback();
            sock.onmessage = messageReceived;
            sock.onclose = disconnected_callback;
        } catch (Error) {
            if (error_callback) error_callback(Error);
        }
    }

    var all_clients = function () {
        return {
            "$type": "Subscribe",
            "Exchange": "allclients",
            "RoutingKeys": ""
        };
    }

    var clients = function () {
        return {
            "$type": "Subscribe",
            "Exchange": "clients",
            "RoutingKeys": username
        };
    }

    var node = function () {
        return {
            "$type": "Subscribe",
            "Exchange": "node",
            "RoutingKeys": ""
        };
    }

    var userJson = function () {
        return { "Name": username };
    }

    var onConnect = function (callback) {
        connected_callback = callback;
    }

    var onDisconnect = function (callback) {
        disconnected_callback = callback;
    }

    var onError = function (callback) {
        error_callback = callback;
    }

    var onMessage = function (callback) {
        on_message = callback;
    }

    var debug = function (callback) {
        info = callback;
    }

    var test = function () {
        alert('hi');
    }

    return {
        connect: connect,
        sendmessage: sendMessage,
        test: test,
        onerror: onError,
        onconnect: onConnect,
        ondisconnect: onDisconnect,
        onmessage: onMessage,
        debug: debug
    };
} ();