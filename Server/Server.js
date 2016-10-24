var io = require('socket.io')({
	transports: ['websocket'],
});
var shortId 		= require('shortid');
var clients			= [];
var sockets = {};
var port = 4567;
io.attach(port);

//open the connection for everyone
io.on('connection', function(socket){
  console.log('A user ready for connection!');

    var currentUser;
	socket.on('beep', function(){
	    console.log('test beep received! ');
		
		socket.emit('boop');
	});
	
	//when client click's play, then begin, get data from json that we sent from the client
	socket.on('LOGIN', function (player)
	{

	    console.log('[INFO] ' + player.name + ' Connected!');
		sockets[player.id] = socket;
		 //create the new users data
		 currentUser = {
			name:player.name,
			tipe:player.tipe,
			id:shortId.generate(),
			position:player.position,
			rotation:player.rotation,
			data:""
		}

		//add currentUser in clients
		clients.push(currentUser);
		console.log(" currentUser "+currentUser);
		console.log('Total players: ' + clients.length);
	    socket.emit('LOGIN_SUCESS',currentUser);
		
	    //list of clients
		for (var i = 0; i < clients.length; i++) {		
			//every client in this server
		  if(clients[i].id != currentUser.id )
		  {
		  	//locally spawn player (INSTANTIATE_PLAYER)
			socket.emit('SPAW_PLAYER',{

				name:clients[i].name,
				tipe:clients[i].tipe,
				id:clients[i].id,
				position:clients[i].position

			});
			console.log(clients[i].name + ' Has Connected to the Server..');
		 }

		};
		//Spawn player (INSTANTIATE_PLAYER)
		socket.broadcast.emit('SPAW_PLAYER',currentUser);
	});
	
	
	//update players positon over the network
	socket.on('MOVE', function (data)
	{
	
      currentUser.position = data.position;
      socket.broadcast.emit('UPDATE_MOVE', currentUser);
      console.log('[POSITION] ' + currentUser.name+" Move to "+currentUser.position);
	  
	});
	
	//update jump
	socket.on('JUMP', function (data)
	{
      currentUser.position = data.position;
      socket.broadcast.emit('UPDATE_JUMP', currentUser);
      console.log('[JUMP] ' + currentUser.name+" Move Up "+currentUser.position);
	  
	});
	
	//update rotate
	socket.on('ROTATE', function (data)
	{
	  currentUser.rotation = data.rotation;
      socket.broadcast.emit('UPDATE_ROTATE', currentUser);
      console.log('[ROTATION] ' + currentUser.name+" Rotate to "+currentUser.rotation);
	 
	});
	
	//disconnect user
	socket.on('disconnect', function ()
	{

		socket.broadcast.emit('USER_DISCONNECTED',currentUser);
		for (var i = 0; i < clients.length; i++)
		{
			if (clients[i].name == currentUser.name && clients[i].id == currentUser.id) 
			{

				console.log("User "+clients[i].name+" has disconnected");
				clients.splice(i,1);

			};
		};
	});
});

//credits
console.log("------ server is running ------- " + port);
console.log("------  RennTek Studios ------ ");
console.log("---- Founder Lewis Comstive ---- ");
console.log("---- CO_Founder Jake Saxton ---- ");
console.log("- Server Developer Jake Saxton - ");
console.log("--Client Developer Jake Saxton -- ");
console.log("---------- ENJOY ---------- ");