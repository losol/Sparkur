"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/initializerHub").build();
var progress = 0;

function addListItem(text, list) {
    var li = document.createElement("li");
    li.textContent = text;
    document.getElementById(list).appendChild(li);
}

connection.on("UpdateProgress", function (message) {

    // var output = $("#resultmessage");
    var n = parseInt(message.progress);

    addListItem(message.progress + '%: ' + message.message, "messagesList");

    //var percetage = $("#percentage");
    //percetage.html(message.Progress + "%");

    //$("#pbar").css("width", n + "%").attr('aria-valuenow', n);

    if (n === 100)
    {
        // find progress bar and .removeClass("progress-bar-striped");
    }

}); 


connection.on("Importing", function (message) {
    addListItem(message, "messagesList");
}); 

connection.on("Pong", function (message) {
    var encodedMsg = "Server says " + message;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
    addListItem("test", "messagesList");
});

connection.start().then(function(){
    document.getElementById("initButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("initButton").addEventListener("click", function (event) {
    var message = "test";
    connection.invoke("LoadExamplesToStore").catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("clearButton").addEventListener("click", function (event) {
    var message = "test";
    connection.invoke("ClearStore").catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
