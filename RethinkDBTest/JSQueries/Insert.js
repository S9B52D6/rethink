r = require('rethinkdb');
file = require(process.argv[2]);
config = require('./Helper/Config.js');

r.connect({ host: 'localhost', port: 28015 }, function (err, conn) {
    if (err)
        throw err;
    r.db(config.DB).table(config.TABLE).insert(file).run(conn, function (err, result) {
        process.exit();
    });
});