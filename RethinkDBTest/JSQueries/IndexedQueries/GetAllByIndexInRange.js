r = require('rethinkdb');
indexQuery = require('../Helper/QueryIndexOnRange.js');
config = require('../Helper/Config');

let start_time = new Date();
r.connect({ host: 'localhost', port: 28015 }, function (err, conn) {
    if (err)
        throw err;
    indexQuery.filterIndexOnRange(0, 50, 'PortfolioId', config.DB, config.TABLE, conn, function (results) {
        let end_time = new Date();
        let time_seconds = Number.parseInt((end_time - start_time).toLocaleString().replace(',', '')) / 1000;
        console.log(results);
        console.log(time_seconds);
        process.exit();
    });
});