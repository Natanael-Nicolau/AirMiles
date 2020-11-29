const http = require('http');
const mysql = require('mysql2');

const hostname = '10.147.17.1';
const port = 50000;


mysqlDb = mysql.createConnection({
  host: '127.0.0.1',
  user: 'ticketUpdateBot',
  password: 'P@ssw0rd',
  database: 'AirMiles'
});

let server = http.createServer((request, response) => {
    
  console.log('URL:', request.url);
  console.log('METHOD:', request.method);

  if(request.url == '/todayTickets') {
    let sqlCommand = `SELECT CLientId, Fullname, StartRegion, EndRegion, StartIATA, EndIATA, FlightClass, StartDate, EndDate
              FROM Flights
              WHERE EndDate BETWEEN (NOW() - INTERVAL 1 DAY) and NOW();`

    response.statusCode = 200;
    response.setHeader('Content-Type', 'application/json');

    mysqlDb.query(sqlCommand, function(error, results) {
      if (error) {
        console.log ('Error', error.message, error.stack);
        response.status(503).send('Can\'t connect to DataBase');
      }
      response.end(JSON.stringify({results}));
    });
  }
});

server.listen(port, hostname, ()=>{
    console.log(`Ticket Update Bot running at http://${hostname}:${port}/`);
});