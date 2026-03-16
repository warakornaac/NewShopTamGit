var webAddress = {};
var param_values = {};
var param_values_Product = {};
var protocol = '';
var resourceAddress = {
    fullAddress: function () {
        var addressBar = window.location.href;
        if (addressBar != '' && addressBar != 'undefined') {
            webAddress['href'] = addressBar;
        }
    },
    protocol_identifier: function () {
        resourceAddress.fullAddress();

        protocol = window.location.protocol.replace(':', '');
        if (protocol != '' && protocol != 'undefined') {
            webAddress['protocol'] = protocol;
        }
    },
    domain: function () {
        resourceAddress.protocol_identifier();

        var domain = window.location.hostname;
        if (domain != '' && domain != 'undefined' && typeOfVar(domain) === 'string') {
            webAddress['domain'] = domain;
            var port = window.location.port;
            if ((port == '' || port == 'undefined') && typeOfVar(port) === 'string') {
                if (protocol == 'http') port = '80';
                if (protocol == 'https') port = '443';
            }
            webAddress['port'] = port;
        }
    },
    pathname: function () {
        resourceAddress.domain();

        var resourcePath = window.location.pathname;
        if (resourcePath != '' && resourcePath != 'undefined') {
            webAddress['resourcePath'] = resourcePath;
        }
    },
    params: function () {
        resourceAddress.pathname();

        var v_args = location.search.substring(1).split("/");

        if (v_args != '' && v_args != 'undefined')
            for (var i = 0; i < v_args.length; i++) {
                var pair = v_args[i].split("=");

                if (typeOfVar(pair) === 'array') {
                    param_values[decodeURIComponent(pair[0])] = decodeURIComponent(pair[1]);
                  
                }
            }
        webAddress['params'] = param_values;
        
    },
    paramspro: function () {
        resourceAddress.pathname();
        var v_args = location.search.substring(1).split("/");

        if (v_args != '' && v_args != 'undefined')
            for (var i = 0; i < v_args.length; i++) {
                var pair = v_args[i].split("=");

                if (typeOfVar(pair) === 'array') {
                    param_values_Product[decodeURIComponent(pair[2])] = decodeURIComponent(pair[2]);
                }
            }
        
        webAddress['paramspro'] = param_values_Product;
    },
    hash: function () {
        resourceAddress.params();

        var fragment = window.location.hash.substring(1);
        if (fragment != '' && fragment != 'undefined')
            webAddress['hash'] = fragment;
    }
};
function typeOfVar(obj) {
    return {}.toString.call(obj).split(' ')[1].slice(0, -1).toLowerCase();
}