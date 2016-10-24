var io = require('socket.io')({
	transports: ['websocket'],
});
var shortId 		= require('shortid');
var clients			= [];
var sockets = {};
var port = 4567;
io.attach(port);


io.on('connection', function(socket){
  console.log('A user ready for connection!');

    var currentUser;
	socket.on('beep', function(){
	    console.log('test beep received! ');
		
		socket.emit('boop');
	});
	
	//funcao chamada pelo metodo OnClickPlayBtn() do script TestSocketIO3 quando o usuario aperta o botao de login no cliente
	socket.on('LOGIN', function (player)//recebe o pacote JSON player como parametro
	{

	    console.log('[INFO] Player ' + player.name + ' connected!');
		sockets[player.id] = socket;
		 
		 currentUser = {
			name:player.name,
			tipe:player.tipe,
			id:shortId.generate(),
			position:player.position,
			rotation:player.rotation,
			data:""
		}//instancia um novo player nesse servidor para ser adcionado a lista de clients
		
		clients.push(currentUser);//add currentUser in clients
		console.log(" currentUser "+currentUser);
		console.log('Total players: ' + clients.length);
	    socket.emit('LOGIN_SUCESS',currentUser);
		
		//para cada client on-line instancia na maquina do cliente que chamou esta funcao todos os prefabs player correspondentes aos outros clientes
		for (var i = 0; i < clients.length; i++) {		
	     //testa para ver se nao e o propio cliente		
		  if(clients[i].id != currentUser.id )
		  {
		    //envia para o cliente os outros players on line, A chamada INSTANTIATE_PALYER sera processada pela funcao OnInstantiatePlayer no script TestsocketIO no cliente
			socket.emit('SPAW_PLAYER',{

				name:clients[i].name,
				tipe:clients[i].tipe,
				id:clients[i].id,
				position:clients[i].position

			});
			console.log('User name '+clients[i].name+' is connected..');
		 }

		};
		//envia para todos os outros clientes on-line exceto o cliente que chamou esse socket o novo player que e o propio cliente
		socket.broadcast.emit('SPAW_PLAYER',currentUser);//o no broadcast o currentUser nao recebe , INSTANTIATE_PLAYER sera processada pela funcao OnInstantiatePlayer
	});
	
	
	//funcao para atualizar a movimentacao do cliente que chamou este socket para os demais clientes do game
	socket.on('MOVE', function (data)
	{
	
      currentUser.position = data.position;
      socket.broadcast.emit('UPDATE_MOVE', currentUser);//envia para todos os outros clientes a nova posicao do cliente que chamou este socket UPDATE_MOVE' sera
      console.log(currentUser.name+" Move to "+currentUser.position);
	  
	});
	
	//update jump
	socket.on('JUMP', function (data)
	{
	
      currentUser.position = data.position;
      socket.broadcast.emit('UPDATE_JUMP', currentUser);//envia para todos os outros clientes a nova posicao do cliente que chamou este socket UPDATE_MOVE' sera
      console.log(currentUser.name+" Move Up "+currentUser.position);
	  
	});
	
	//funcao para atualizar a movimentacao do cliente que chamou este socket para os demais clientes do game
	socket.on('ROTATE', function (data)
	{
    
	  currentUser.rotation = data.rotation;
      socket.broadcast.emit('UPDATE_ROTATE', currentUser);//envia para todos os outros clientes a nova posicao do cliente que chamou este socket UPDATE_MOVE' sera
      console.log(currentUser.name+" Rotate to "+currentUser.rotation); // processada pela funcao onUserMove em todos os clientes exceto o cliente que chamou este socket
	 
	});
	
	
	
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


console.log("------ server is running ------- " + port);
console.log("------  RennTek Studios ------ ");
console.log("---- Founder Lewis Comstive ---- ");
console.log("---- CO_Founder Jake Saxton ---- ");
console.log("- Server Developer Jake Saxton - ");
console.log("--Client Developer Jake Saxton -- ");
console.log("---------- ENJOY ---------- ");