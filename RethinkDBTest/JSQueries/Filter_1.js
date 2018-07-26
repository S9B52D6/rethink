r = require('rethinkdb');
file = require('./reports.json');
config = require('./Helper/Config.js');

let start_time = new Date();
r.connect({ host: 'localhost', port: 28015 }, function (err, conn) {
    if (err)
        throw err;
    r.db(config.DB).table(config.TABLE)
        .concatMap(function (x) { return x; })
        .filter(r.row('Current').ge(35000))
        .avg('Current')
        .run(conn, function (err, cursor) {
            if (err)
                throw err;
            let end_time = new Date();
            let time_seconds = Number.parseInt((end_time - start_time).toLocaleString().replace(',', '')) / 1000;
            console.log(cursor, time_seconds + 's');
            process.exit();
        });
});