Object.assign = Object.assign || function () {
    var argLength = arguments.length;
    if (argLength === 0 || typeof (arguments[0]) !== 'object') {
        return {};
    }
    var target = arguments[0];
    for (var i = 1; i < argLength; ++i) {
        var source = arguments[i];
        if (typeof (source) === 'object') {
            for (var key in source) {
                target[key] = source[key];
            }
        }
    }
    return target;
  }