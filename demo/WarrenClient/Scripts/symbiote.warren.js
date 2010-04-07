var warren = function () {

    var sock = null; // web socket
    var connected_callback = null;
    var disconnected_callback = null;
    var error_callback = null;
    var on_message = null;
    var username = '';

    var connect = function (url, user) {
        if (checkBrowser()) {
            try {
                username = user;
                sock = new WebSocket(url);
            } catch (Error) {
                if (error_callback) error_callback('Error occurred during connection attempt: ' + Error);
            }

            sock.onopen = setup;
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

    var send = function (jsonMessage) {
        try {
            var message = $.toJSON(jsonMessage);
        } catch (Error) {
            if (error_callback) error_callback(Error);
        }
    }

    var messageReceived = function (msg) {
        var json = $.evalJSON(msg);
        if (on_message) on_message(json);
    }

    var setup = function () {
        try {
            setTimeout(send(userJson), 200);
            setTimeout(send(all_clients), 200);
            setTimeout(send(clients), 200);
            setTimeout(send(node), 200);
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

    var test = function () {
        alert('hi');
    }

    return {
        connect: connect,
        sendMessage: send,
        test: test,
        onerror: onError,
        onconnect: onConnect,
        ondisconnect: onDisconnect,
        onmessage: onMessage
    };
} ();