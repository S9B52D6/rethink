r = require('rethinkdb');
file = require('./reports.json');
config = require('./Helper/Config.js');

r.connect({ host: 'localhost', port: 28015 }, function (err, conn) {
    if (err)
        throw err;

    let start_time = new Date();
    r.db(config.DB).table(config.TABLE)("_reports")("FBA Base")
        .concatMap(function (x) { return x; })
        .pluck('Current', 'OutstandingBal')
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