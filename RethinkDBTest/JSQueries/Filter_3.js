r = require('rethinkdb');
file = require('./reports.json');
config = require('./Helper/Config.js');

let start_time = new Date();
r.connect({ host: 'localhost', port: 28015 }, function (err, conn) {
    if (err)
        throw err;
    r.db(config.DB).table(config.TABLE)("_reports")("FBA Base")
        .concatMap(function (x) { return x; })
        .filter(r.row('Current').ge(35000) && r.row('OutstandingBal').ge(20000) && r.row('PropertyValue').ge(50000))
        //.avg('Current')
        .run(conn, function (err, cursor) {
            if (err)
                throw err;
            
            cursor.each(function (err, row) { }, function () {
                let end_time = new Date();
                let time_seconds = Number.parseInt((end_time - start_time).toLocaleString().replace(',', '')) / 1000;
                console.log(time_seconds + 's');
                process.exit();
            });
        });
});