r = require('rethinkdb');
indexQuery = require('./Helper/QueryIndexOnRange.js');
config = require('./Helper/Config');

let start_time = new Date();
r.connect({ host: 'localhost', port: 28015 }, function (err, conn) {
    if (err)
        throw err;
    r.db(config.DB).table(config.TABLE).getAll(66, { index: 'PortfolioId' }).run(conn, function (err, cursor) {
        if (err)
            throw err;
        cursor.toArray(function (err, results) {
            let end_time = new Date();
            let time_seconds = Number.parseInt((end_time - start_time).toLocaleString().replace(',', '')) / 1000;
            console.log(results);
            console.log(time_seconds);
            process.exit();
        })
    });
});