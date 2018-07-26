exports.pluckIndexOnRange = function (start, end, index, pluckArg, db, table, conn, callback) {
    let results = [];
    let finishedQueries = 0;
    for (let i = start; i < end + 1; i++) {
        r.db(db).table(table).getAll(i, { index: index }).pluck(pluckArg).run(conn, function (err, cursor) {
            if (err)
                throw err;
            cursor.toArray(function (err, result) {
                if (err)
                    throw err;
                finishedQueries++;
                results.push(result);
                if (finishedQueries == end - start) {
                    callback(results);
                }
            });
        });
    }
};

exports.filterIndexOnRange = function (start, end, index, db, table, conn, callback) {
    let results = [];
    let finishedQueries = 0;
    for (let i = start; i < end + 1; i++) {
        r.db(db).table(table).getAll(i, { index: index }).run(conn, function (err, cursor) {
            if (err)
                throw err;
            cursor.toArray(function (err, result) {
                if (err)
                    throw err;
                finishedQueries++;
                results = result;
                if (finishedQueries == end - start) {
                    callback(results);
                }
            });
        });
    }
};